import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {StudentInvoice, StudentInvoiceItem, InvoiceStatus, FeeStructure, FeeCategory} from '@/finance/models/finance-models';
import {FeeStructureService, StudentInvoiceService, FeeCategoryService, SponsorshipService} from '@/finance/services/finance-services';
import {Sponsorship, SponsorshipCoverageType} from '@/finance/models/sponsorship';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {Status} from '@/core/enums/status';
import {formatDate} from '@angular/common';

@Component({
    selector: 'app-finance-invoices',
    templateUrl: './invoices.component.html'
})
export class FinanceInvoicesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/invoices'], title: 'Student Invoices'}
    ];
    dashboardTitle = 'Finance: Student Invoices';

    invoices: StudentInvoice[] = [];
    allFeeStructures: FeeStructure[] = [];
    feeStructures: FeeStructure[] = [];
    categories: FeeCategory[] = [];
    academicYears: any[] = [];
    allSessions: any[] = [];
    bulkSessions: any[] = [];
    filterSessions: any[] = [];
    learningLevels: any[] = [];
    schoolClasses: any[] = [];

    showBulk = false;
    showEdit = false;
    editInvoice: StudentInvoice = new StudentInvoice();
    viewDetail: StudentInvoice | null = null;

    bulkAcademicYearId: any = null;
    bulkSessionId: any = null;
    bulkFeeStructureId: any = null;
    bulkClassId: any = null;
    bulkInvoiceDate: string = '';
    bulkDueDate: string = '';
    bulkDescription: string = '';

    filterYearId: any = null;
    filterSessionId: any = null;
    filterLearningLevelId: any = null;
    filterClassId: any = null;
    filterStatus: any = null;
    filterStudentSearch: string = '';
    filterClasses: any[] = [];
    filterClassStudentIds: Set<number> = new Set();
    studentClassMap: Map<number, string> = new Map();

    page = 1;
    pageSize = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    // Active sponsorships for the student whose invoice is being edited.
    activeSponsorships: Sponsorship[] = [];

    constructor(
        private toastr: ToastrService,
        private svc: StudentInvoiceService,
        private feeStructureSvc: FeeStructureService,
        private catSvc: FeeCategoryService,
        private academicYearSvc: AcademicYearsService,
        private sessionSvc: SessionsService,
        private llSvc: LearningLevelsService,
        private schoolClassesSvc: SchoolClassesService,
        private studentClassSvc: StudentClassService,
        private sponsorshipSvc: SponsorshipService
    ) {}

    ngOnInit(): void {
        this.bulkInvoiceDate = formatDate(new Date(), 'yyyy-MM-dd', 'en');
        forkJoin([
            this.svc.get('/studentInvoices'),
            this.feeStructureSvc.get('/feeStructures'),
            this.academicYearSvc.get('/academicYears'),
            this.sessionSvc.get('/sessions'),
            this.llSvc.get('/learningLevels'),
            this.catSvc.get('/feeCategories')
        ]).subscribe({
            next: ([invoices, fs, years, sessions, levels, cats]: any) => {
                this.invoices = invoices.sort((a: any, b: any) => new Date(b.invoiceDate).getTime() - new Date(a.invoiceDate).getTime());
                this.allFeeStructures = fs.filter((f: any) => f.isActive);
                this.academicYears = (years || []).sort((a: any, b: any) => (b.rank || 0) - (a.rank || 0));
                this.allSessions = sessions || [];
                this.learningLevels = (levels || []).sort((a: any, b: any) => (a.rank || 0) - (b.rank || 0));
                this.categories = (cats || []).filter((c: any) => c.isActive).sort((a: any, b: any) => (a.rank || 0) - (b.rank || 0));
                // Default filter to active year + active session
                let activeYear = this.academicYears.find((y: any) => y.status === true);
                if (activeYear) {
                    this.filterYearId = activeYear.id;
                    let activeSessionId: any = null;
                    let sessionsForYear = this.allSessions.filter((s: any) => s.academicYearId == activeYear.id);
                    let activeSession = sessionsForYear.find((s: any) => s.status === true);
                    if (activeSession) activeSessionId = activeSession.id;
                    // Trigger cascade loading of sessions, classes and student-class map
                    this.onFilterYearChange();
                    this.filterSessionId = activeSessionId;
                }
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    load = () => {
        this.svc.get('/studentInvoices').subscribe({
            next: (inv) => this.invoices = inv.sort((a: any, b: any) => new Date(b.invoiceDate).getTime() - new Date(a.invoiceDate).getTime()),
            error: (err) => this.toastr.error(err.error)
        });
    };

    // --- Filters ---
    onFilterYearChange = () => {
        this.filterSessions = this.filterYearId
            ? this.allSessions.filter((s: any) => s.academicYearId == this.filterYearId)
            : [];
        this.filterSessionId = null;
        this.filterClassId = null;
        this.filterClasses = [];
        this.filterClassStudentIds = new Set();
        this.studentClassMap = new Map();
        if (this.filterYearId) {
            this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${this.filterYearId}`).subscribe({
                next: (classes: any[]) => {
                    this.filterClasses = (classes || []).sort((a: any, b: any) =>
                        (a.learningLevel?.rank || 0) - (b.learningLevel?.rank || 0));
                    // Build student→class map for all classes in this year
                    this.filterClasses.forEach((c: any) => {
                        this.studentClassSvc.getBySchoolClassId(+c.id, Status.Active).subscribe({
                            next: (scs: any[]) => {
                                let label = `${c.learningLevel?.name || ''}${c.schoolStream?.name ? ' - ' + c.schoolStream.name : ''}`;
                                (scs || []).forEach((sc: any) => this.studentClassMap.set(sc.studentId, label));
                            },
                            error: () => {}
                        });
                    });
                },
                error: () => {}
            });
        }
        this.page = 1;
    };

    getStudentClass = (studentId: any): string => {
        if (!studentId) return '';
        return this.studentClassMap.get(+studentId) || '';
    };

    onFilterClassChange = () => {
        this.filterClassStudentIds = new Set();
        if (!this.filterClassId) { this.page = 1; return; }
        this.studentClassSvc.getBySchoolClassId(+this.filterClassId, Status.Active).subscribe({
            next: (scs: any[]) => {
                this.filterClassStudentIds = new Set((scs || []).map((sc: any) => sc.studentId));
                this.page = 1;
            },
            error: () => {}
        });
    };

    filtered = (): StudentInvoice[] => {
        return this.invoices.filter((i) => {
            if (this.filterYearId != null && i.academicYearId != this.filterYearId) return false;
            if (this.filterSessionId != null && i.sessionId != this.filterSessionId) return false;
            if (this.filterStatus != null && i.status != this.filterStatus) return false;
            if (this.filterClassId != null && !this.filterClassStudentIds.has(+i.studentId!)) return false;
            if (this.filterStudentSearch) {
                let q = this.filterStudentSearch.toLowerCase();
                let name = (i.studentName || '').toLowerCase();
                let upi = (i.studentUPI || '').toLowerCase();
                if (!name.includes(q) && !upi.includes(q)) return false;
            }
            return true;
        });
    };

    clearFilters = () => {
        this.filterYearId = null;
        this.filterSessionId = null;
        this.filterLearningLevelId = null;
        this.filterClassId = null;
        this.filterStatus = null;
        this.filterStudentSearch = '';
        this.filterSessions = [];
        this.filterClasses = [];
        this.filterClassStudentIds = new Set();
        this.page = 1;
    };

    // --- Bulk Generate ---
    onBulkYearChange = () => {
        this.bulkSessionId = null;
        this.bulkFeeStructureId = null;
        this.bulkClassId = null;
        this.feeStructures = [];
        this.schoolClasses = [];
        this.bulkSessions = this.bulkAcademicYearId
            ? this.allSessions.filter((s: any) => s.academicYearId == this.bulkAcademicYearId)
            : [];
    };

    onBulkSessionChange = () => {
        this.bulkFeeStructureId = null;
        this.bulkClassId = null;
        this.schoolClasses = [];
        this.feeStructures = this.allFeeStructures.filter((f: any) => {
            if (this.bulkAcademicYearId && f.academicYearId != this.bulkAcademicYearId) return false;
            if (this.bulkSessionId && f.sessionId != this.bulkSessionId) return false;
            return true;
        }).sort((a: any, b: any) => {
            let llA = this.learningLevels.find((l: any) => l.id == a.learningLevelId);
            let llB = this.learningLevels.find((l: any) => l.id == b.learningLevelId);
            return (llA?.rank || 0) - (llB?.rank || 0);
        });
    };

    onBulkFsChange = () => {
        let fs = this.feeStructures.find((f) => f.id == this.bulkFeeStructureId);
        if (!fs) { this.schoolClasses = []; this.bulkClassId = null; return; }
        this.bulkClassId = null;
        this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${fs.academicYearId}`).subscribe({
            next: (classes: any[]) => {
                this.schoolClasses = fs!.learningLevelId
                    ? classes.filter((c: any) => c.learningLevelId == fs!.learningLevelId)
                    : classes;
                if (this.schoolClasses.length === 1) this.bulkClassId = this.schoolClasses[0].id;
            },
            error: () => {}
        });
    };

    runBulk() {
        if (!this.bulkFeeStructureId || !this.bulkClassId || !this.bulkInvoiceDate) {
            this.toastr.warning('Fee structure, class and date required.'); return;
        }
        Swal.fire({
            title: 'Generate invoices?',
            text: 'One invoice per active student with mandatory fee items. Any active sponsorships will be auto-applied.',
            icon: 'question', showCancelButton: true, confirmButtonText: 'Generate'
        }).then((r) => {
            if (r.value) {
                this.svc.bulkCreate({
                    feeStructureId: +this.bulkFeeStructureId,
                    schoolClassId: +this.bulkClassId,
                    invoiceDate: this.bulkInvoiceDate,
                    dueDate: this.bulkDueDate || null,
                    description: this.bulkDescription
                }).subscribe({
                    next: (resp: any) => {
                        this.showBulk = false;
                        this.load();
                        this.showBulkSummary(resp);
                    },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    }

    private showBulkSummary(resp: any) {
        let studentsTotal = resp?.count || 0;
        let sponsored = resp?.studentsWithSponsorships || 0;
        let lines = resp?.sponsoredLineCount || 0;
        let total = resp?.totalSponsored || 0;
        let perSponsor: any[] = resp?.perSponsor || [];

        let perSponsorHtml = perSponsor.length > 0
            ? `<table class="table table-sm mt-2" style="font-size:0.85rem;">
                <thead class="table-light"><tr><th>Sponsor</th><th class="text-end">Amount</th></tr></thead>
                <tbody>${perSponsor.map((p: any) =>
                    `<tr><td>${p.sponsor}</td><td class="text-end">${(+p.amount || 0).toLocaleString(undefined, {minimumFractionDigits: 2, maximumFractionDigits: 2})}</td></tr>`).join('')}</tbody>
               </table>`
            : '';

        let html = `
            <div style="text-align:left; font-size:0.9rem;">
                <p><strong>${studentsTotal}</strong> invoice${studentsTotal === 1 ? '' : 's'} created.</p>
                ${sponsored > 0
                    ? `<p><strong>${sponsored}</strong> student${sponsored === 1 ? '' : 's'} matched an active sponsorship (${lines} line${lines === 1 ? '' : 's'} covered).</p>
                       <p>Total sponsored: <strong>${(+total || 0).toLocaleString(undefined, {minimumFractionDigits: 2, maximumFractionDigits: 2})}</strong></p>
                       ${perSponsorHtml}`
                    : '<p class="text-muted"><i class="fas fa-info-circle me-1"></i> No active sponsorships applied to these invoices.</p>'}
            </div>`;

        Swal.fire({
            title: 'Invoices generated',
            html, icon: 'success', width: 600, position: 'top', padding: '1em',
            confirmButtonText: 'Done'
        });
    }

    // --- View Detail (read-only) ---
    viewItems = (inv: StudentInvoice) => {
        this.svc.getById(parseInt(inv.id), '/studentInvoices').subscribe({
            next: (raw: any) => {
                let full = Array.isArray(raw) ? raw[0] : raw;
                this.viewDetail = new StudentInvoice(full);
                this.viewDetail.items = (full?.items || []).map((i: any) => Object.assign(new StudentInvoiceItem(), i));
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    closeDetail = () => { this.viewDetail = null; };

    getDetailTotal = (): number => {
        if (!this.viewDetail) return 0;
        return (this.viewDetail.items || []).reduce((s, i) => s + (+i.amount! || 0) - (+i.discount! || 0), 0);
    };

    getDetailPaidTotal = (): number => {
        if (!this.viewDetail) return 0;
        return (this.viewDetail.items || []).reduce((s, i) => s + (+i.paidAmount! || 0), 0);
    };

    // --- Edit Invoice Items ---
    editItems = (inv: StudentInvoice) => {
        this.svc.getById(parseInt(inv.id), '/studentInvoices').subscribe({
            next: (raw: any) => {
                let full = Array.isArray(raw) ? raw[0] : raw;
                this.editInvoice = new StudentInvoice(full);
                this.editInvoice.items = (full?.items || []).map((i: any) => Object.assign(new StudentInvoiceItem(), i));

                // Merge in any optional/mandatory items from the matching fee structure(s).
                // Amounts come from the structure and are locked; to change them the user must
                // edit the fee structure first.
                this.mergeFeeStructureItemsIntoInvoice();

                // Load the student's active sponsorships so the edit form can offer them per line.
                this.activeSponsorships = [];
                if (this.editInvoice.studentId) {
                    let onDate = this.editInvoice.invoiceDate
                        ? (this.editInvoice.invoiceDate as any).toString().substring(0, 10)
                        : undefined;
                    this.sponsorshipSvc.byStudent(+this.editInvoice.studentId,
                        +(this.editInvoice.academicYearId || 0) || undefined,
                        +(this.editInvoice.sessionId || 0) || undefined,
                        onDate).subscribe({
                        next: (sps) => { this.activeSponsorships = sps || []; },
                        error: () => { this.activeSponsorships = []; }
                    });
                }

                this.showEdit = true;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    // Sponsorships applicable to an item's fee category. Empty FeeCategoryIds on a sponsorship means
    // it covers every fee category.
    sponsorshipsForItem = (item: any): Sponsorship[] => {
        let catId = +item.feeCategoryId;
        if (!catId) return [];
        return this.activeSponsorships.filter(s =>
            !s.feeCategoryIds || s.feeCategoryIds.length === 0 || s.feeCategoryIds.includes(catId));
    };

    // When the user picks a sponsorship on a line, auto-compute the discount from the sponsorship's coverage.
    onSponsorshipChange = (item: any) => {
        if (!item.sponsorshipId) { item.discount = 0; return; }
        let sp = this.activeSponsorships.find(s => s.id === +item.sponsorshipId);
        if (!sp) return;
        let amt = +item.amount || 0;
        switch (sp.coverageType) {
            case SponsorshipCoverageType.FullCoverage:
                item.discount = amt;
                break;
            case SponsorshipCoverageType.Percentage:
                item.discount = Math.round((amt * (sp.percentage / 100)) * 100) / 100;
                break;
            case SponsorshipCoverageType.FixedAmount:
                // Per-line cap equals item amount (bulk's across-line allocation isn't replicated here — manual edit).
                item.discount = Math.min(sp.fixedAmount || 0, amt);
                break;
        }
    };

    sponsorshipLabel = (sp: Sponsorship): string => {
        let coverage = '';
        if (sp.coverageType === SponsorshipCoverageType.FullCoverage) coverage = 'Full';
        else if (sp.coverageType === SponsorshipCoverageType.Percentage) coverage = `${sp.percentage}%`;
        else coverage = `${sp.fixedAmount}`;
        return `${sp.sponsorName} (${coverage})`;
    };

    // Pull all fee structure items that match this invoice's academic year + session, and either:
    // - tag an existing invoice item as "from structure" (amount locked), or
    // - add it as a new line with `_selected = false` so the user can opt in.
    mergeFeeStructureItemsIntoInvoice = () => {
        let structures = (this.allFeeStructures || []).filter((fs: any) =>
            fs.academicYearId == this.editInvoice.academicYearId &&
            (this.editInvoice.sessionId == null || fs.sessionId == null || fs.sessionId == this.editInvoice.sessionId));

        // Map: feeCategoryId -> { amount, isMandatory }
        let structureByCategory = new Map<number, {amount: number; isMandatory: boolean}>();
        structures.forEach((fs: any) => {
            (fs.items || []).forEach((it: any) => {
                let existing = structureByCategory.get(+it.feeCategoryId);
                if (!existing) {
                    structureByCategory.set(+it.feeCategoryId, {amount: +it.amount || 0, isMandatory: !!it.isMandatory});
                } else {
                    // If duplicated across structures, prefer mandatory and the higher amount.
                    existing.isMandatory = existing.isMandatory || !!it.isMandatory;
                    existing.amount = Math.max(existing.amount, +it.amount || 0);
                }
            });
        });

        // Tag existing invoice items as _fromStructure when matching a structure entry.
        this.editInvoice.items.forEach((i: any) => {
            let match = structureByCategory.get(+i.feeCategoryId!);
            if (match) {
                i._fromStructure = true;
                i._isMandatory = match.isMandatory;
                i._selected = true; // already on the invoice
            } else {
                i._fromStructure = false;
                i._isMandatory = false;
                i._selected = true;
            }
        });

        // Append any structure items that aren't yet on the invoice — user can include them.
        let existingCategoryIds = new Set(this.editInvoice.items.map((i: any) => +i.feeCategoryId!));
        structureByCategory.forEach((info, feeCategoryId) => {
            if (existingCategoryIds.has(feeCategoryId)) return;
            let ni: any = Object.assign(new StudentInvoiceItem(), {
                feeCategoryId: feeCategoryId,
                amount: info.amount,
                discount: 0,
                paidAmount: 0
            });
            ni._fromStructure = true;
            ni._isMandatory = info.isMandatory;
            ni._selected = info.isMandatory; // mandatory items default to included
            this.editInvoice.items.push(ni);
        });
    };

    cancelEdit = () => { this.showEdit = false; };

    addInvoiceItem = () => {
        this.editInvoice.items.push(Object.assign(new StudentInvoiceItem(), {amount: 0, discount: 0}));
    };

    removeInvoiceItem = (item: StudentInvoiceItem) => {
        let idx = this.editInvoice.items.indexOf(item);
        if (idx >= 0) this.editInvoice.items.splice(idx, 1);
    };

    getEditTotal = (): number => {
        return (this.editInvoice.items || [])
            .filter((i: any) => i._selected !== false)
            .reduce((s, i) => s + (+i.amount! || 0) - (+i.discount! || 0), 0);
    };

    saveInvoiceItems = () => {
        // Only persist the items the user has selected.
        let selected = (this.editInvoice.items || []).filter((i: any) => i._selected !== false);
        if (selected.length === 0) {
            this.toastr.info('At least one item must be selected.'); return;
        }
        for (let it of selected) {
            if (!it.feeCategoryId) { this.toastr.info('Each item must have a fee category.'); return; }
        }
        let payload = {
            StudentId: this.editInvoice.studentId,
            AcademicYearId: this.editInvoice.academicYearId,
            SessionId: this.editInvoice.sessionId,
            InvoiceDate: this.editInvoice.invoiceDate,
            DueDate: this.editInvoice.dueDate,
            DiscountAmount: this.editInvoice.discountAmount || 0,
            Description: this.editInvoice.description,
            Status: this.editInvoice.status,
            Items: selected.map((i: any) => ({
                Id: i.id || null,
                FeeCategoryId: i.feeCategoryId,
                Amount: +i.amount! || 0,
                Discount: +i.discount! || 0,
                SponsorshipId: i.sponsorshipId ? +i.sponsorshipId : null,
                Description: i.description
            }))
        };
        this.svc.updateById(+this.editInvoice.id, payload).subscribe({
            next: () => {
                this.toastr.success('Invoice updated.');
                this.showEdit = false;
                this.load();
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error saving.')
        });
    };

    // --- Helpers ---
    statusLabel(s: any): string {
        const labels: any = {0: 'Unpaid', 1: 'Partially Paid', 2: 'Paid', 3: 'Overdue', 4: 'Cancelled'};
        return labels[s] || '';
    }
    statusBadge(s: any): string {
        const badges: any = {0: 'bg-danger', 1: 'bg-warning text-dark', 2: 'bg-success', 3: 'bg-danger', 4: 'bg-secondary'};
        return badges[s] || 'bg-secondary';
    }

    delete(i: StudentInvoice) {
        Swal.fire({title: 'Delete invoice?', icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (r.value) {
                this.svc.delete('/studentInvoices', parseInt(i.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    }
}
