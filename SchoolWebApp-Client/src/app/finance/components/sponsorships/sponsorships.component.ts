import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {formatDate} from '@angular/common';
import {Sponsor, Sponsorship, SponsorshipCoverageType, SponsorshipStatus} from '@/finance/models/sponsorship';
import {SponsorService, SponsorshipService, FeeCategoryService} from '@/finance/services/finance-services';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {StudentDetailsService} from '@/students/services/student-details.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {Status} from '@/core/enums/status';

@Component({
    selector: 'app-finance-sponsorships',
    templateUrl: './sponsorships.component.html'
})
export class SponsorshipsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/sponsorships'], title: 'Sponsorships'}
    ];
    dashboardTitle = 'Finance: Sponsorships';

    sponsorships: Sponsorship[] = [];
    sponsors: Sponsor[] = [];
    academicYears: any[] = [];
    allSessions: any[] = [];
    formSessions: any[] = [];
    schoolClasses: any[] = [];
    formStudents: any[] = [];
    feeCategories: any[] = [];

    item: Sponsorship = {
        sponsorId: 0,
        academicYearId: 0,
        coverageType: SponsorshipCoverageType.FullCoverage,
        fixedAmount: 0,
        percentage: 100,
        startDate: formatDate(new Date(), 'yyyy-MM-dd', 'en'),
        status: SponsorshipStatus.Active,
        feeCategoryIds: []
    };
    editMode = false;
    showForm = false;

    targetMode: 'student' | 'class' = 'student';

    // Filters
    filterYearId: any = null;
    filterSponsorId: any = null;
    filterStatus: any = null;

    page = 1;
    pageSize = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    coverageTypes = [
        {value: SponsorshipCoverageType.FullCoverage, label: 'Full Coverage (100%)'},
        {value: SponsorshipCoverageType.Percentage, label: 'Percentage'},
        {value: SponsorshipCoverageType.FixedAmount, label: 'Fixed Amount'}
    ];

    statuses = [
        {value: SponsorshipStatus.Active, label: 'Active'},
        {value: SponsorshipStatus.Ended, label: 'Ended'},
        {value: SponsorshipStatus.Cancelled, label: 'Cancelled'}
    ];

    constructor(
        private toastr: ToastrService,
        private svc: SponsorshipService,
        private sponsorSvc: SponsorService,
        private yearSvc: AcademicYearsService,
        private sessionSvc: SessionsService,
        private classSvc: SchoolClassesService,
        private studentSvc: StudentDetailsService,
        private studentClassSvc: StudentClassService,
        private catSvc: FeeCategoryService
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.svc.getAll(),
            this.sponsorSvc.getAll(),
            this.yearSvc.get('/academicYears'),
            this.sessionSvc.get('/sessions'),
            this.catSvc.get('/feeCategories')
        ]).subscribe({
            next: ([sps, sponsors, years, sessions, cats]: any) => {
                this.sponsorships = sps || [];
                this.sponsors = (sponsors || []).filter((s: Sponsor) => s.isActive);
                this.academicYears = (years || []).sort((a: any, b: any) => (b.rank || 0) - (a.rank || 0));
                this.allSessions = sessions || [];
                this.feeCategories = (cats || []).filter((c: any) => c.isActive);
                let active = this.academicYears.find((y: any) => y.status === true);
                this.filterYearId = active ? active.id : null;
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading.')
        });
    }

    load = () => {
        this.svc.getAll().subscribe({
            next: (s) => this.sponsorships = s || [],
            error: () => {}
        });
    };

    blank = (): Sponsorship => ({
        sponsorId: 0,
        academicYearId: 0,
        coverageType: SponsorshipCoverageType.FullCoverage,
        fixedAmount: 0,
        percentage: 100,
        startDate: formatDate(new Date(), 'yyyy-MM-dd', 'en'),
        status: SponsorshipStatus.Active,
        feeCategoryIds: []
    });

    filtered = (): Sponsorship[] =>
        this.sponsorships.filter(s => {
            if (this.filterYearId && s.academicYearId != this.filterYearId) return false;
            if (this.filterSponsorId && s.sponsorId != this.filterSponsorId) return false;
            if (this.filterStatus != null && s.status !== +this.filterStatus) return false;
            return true;
        });

    clearFilters = () => {
        let active = this.academicYears.find((y: any) => y.status === true);
        this.filterYearId = active ? active.id : null;
        this.filterSponsorId = null;
        this.filterStatus = null;
    };

    addNew = () => {
        this.item = this.blank();
        this.targetMode = 'student';
        let active = this.academicYears.find((y: any) => y.status === true);
        if (active) { this.item.academicYearId = active.id; this.onFormYearChange(); }
        this.editMode = false;
        this.showForm = true;
    };

    edit = (s: Sponsorship) => {
        this.item = {...s, feeCategoryIds: [...(s.feeCategoryIds || [])]};
        this.item.startDate = s.startDate ? s.startDate.substring(0, 10) : '';
        this.item.endDate = s.endDate ? s.endDate.substring(0, 10) : null;
        this.targetMode = s.studentId ? 'student' : 'class';
        this.onFormYearChange();
        if (this.targetMode === 'student' && this.item.schoolClassId) {
            // Reload students for the existing class
            this.onFormClassChange();
        }
        this.editMode = true;
        this.showForm = true;
    };

    cancel = () => { this.showForm = false; };

    onFormYearChange = () => {
        this.formSessions = this.item.academicYearId
            ? this.allSessions.filter((s: any) => s.academicYearId == this.item.academicYearId)
            : [];
        if (this.item.academicYearId) {
            this.classSvc.get(`/schoolClasses/byAcademicYearId/${this.item.academicYearId}`).subscribe({
                next: (classes: any[]) => {
                    this.schoolClasses = (classes || []).sort((a: any, b: any) =>
                        (a.learningLevel?.rank || 0) - (b.learningLevel?.rank || 0));
                },
                error: () => {}
            });
        } else {
            this.schoolClasses = [];
        }
    };

    onFormClassChange = () => {
        this.formStudents = [];
        if (!this.item.schoolClassId) return;
        this.studentClassSvc.getBySchoolClassId(+this.item.schoolClassId, Status.Active).subscribe({
            next: (scs: any[]) => {
                this.formStudents = (scs || [])
                    .filter((sc: any) => sc.student)
                    .map((sc: any) => {
                        let s = sc.student;
                        s.displayName = `${s.upi || ''} - ${s.fullName || ''}`;
                        return s;
                    });
            },
            error: () => {}
        });
    };

    onTargetModeChange = () => {
        if (this.targetMode === 'student') {
            this.item.schoolClassId = null;
        } else {
            this.item.studentId = null;
        }
    };

    toggleFeeCategory = (id: number) => {
        let idx = this.item.feeCategoryIds.indexOf(id);
        if (idx >= 0) this.item.feeCategoryIds.splice(idx, 1);
        else this.item.feeCategoryIds.push(id);
    };

    isFeeCategoryChecked = (id: number): boolean => {
        return this.item.feeCategoryIds.includes(id);
    };

    save = () => {
        if (!this.item.sponsorId) { this.toastr.info('Select a sponsor.'); return; }
        if (!this.item.academicYearId) { this.toastr.info('Academic year is required.'); return; }
        if (this.targetMode === 'student' && !this.item.studentId) { this.toastr.info('Select a student.'); return; }
        if (this.targetMode === 'class' && !this.item.schoolClassId) { this.toastr.info('Select a class.'); return; }
        if (this.targetMode === 'student') this.item.schoolClassId = null;
        else this.item.studentId = null;
        if (this.item.coverageType === SponsorshipCoverageType.Percentage && (this.item.percentage <= 0 || this.item.percentage > 100)) {
            this.toastr.info('Percentage must be between 1 and 100.'); return;
        }
        if (this.item.coverageType === SponsorshipCoverageType.FixedAmount && this.item.fixedAmount <= 0) {
            this.toastr.info('Fixed amount must be positive.'); return;
        }

        let obs = this.editMode ? this.svc.update(+this.item.id!, this.item) : this.svc.create(this.item);
        obs.subscribe({
            next: () => {
                this.toastr.success(this.editMode ? 'Sponsorship updated.' : 'Sponsorship created.');
                this.showForm = false;
                this.load();
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error saving sponsorship.')
        });
    };

    applyToExisting = (s: Sponsorship) => {
        if (s.status !== SponsorshipStatus.Active) {
            this.toastr.info('Only active sponsorships can be applied.');
            return;
        }
        Swal.fire({
            title: `Apply '${s.sponsorName}' to existing invoices?`,
            html: `<div style="text-align:left; font-size:0.9rem;">
                <p>This will look at every <strong>open</strong> invoice in scope and:</p>
                <ul>
                    <li>Apply the sponsorship discount to lines that match a covered fee category</li>
                    <li>Skip lines already linked to another sponsorship</li>
                    <li>Skip fully-paid invoices</li>
                    <li>Post an adjustment journal: <strong>Dr Sponsor Receivable / Cr Student Debtors</strong></li>
                </ul>
                <p class="text-muted" style="font-size:0.8rem;">For partially-paid invoices, the discount is capped at the unpaid portion of each line.</p>
            </div>`,
            width: 560, position: 'top', padding: '1em',
            icon: 'question', showCancelButton: true, confirmButtonText: 'Apply'
        }).then(r => {
            if (!r.value) return;
            this.svc.applyToExisting(+s.id!).subscribe({
                next: (resp: any) => {
                    this.load();
                    this.showApplySummary(s, resp);
                },
                error: (err) => this.toastr.error(err.error?.message || 'Error applying sponsorship.')
            });
        });
    };

    private showApplySummary(s: Sponsorship, resp: any) {
        let html = `
            <div style="text-align:left; font-size:0.9rem;">
                <p><strong>${resp.appliedInvoices || 0}</strong> invoice${(resp.appliedInvoices === 1) ? '' : 's'} updated, <strong>${resp.appliedItems || 0}</strong> line${(resp.appliedItems === 1) ? '' : 's'} sponsored.</p>
                <p>Total applied: <strong>${(+resp.totalApplied || 0).toLocaleString(undefined, {minimumFractionDigits: 2, maximumFractionDigits: 2})}</strong></p>
                <hr/>
                <p class="text-muted" style="font-size:0.8rem;">
                    Skipped: ${resp.skippedAlreadySponsoredItems || 0} already-sponsored line${(resp.skippedAlreadySponsoredItems === 1) ? '' : 's'},
                    ${resp.skippedNoCoverageItems || 0} no-coverage line${(resp.skippedNoCoverageItems === 1) ? '' : 's'},
                    ${resp.skippedFullyPaidInvoices || 0} fully-paid invoice${(resp.skippedFullyPaidInvoices === 1) ? '' : 's'}.
                </p>
            </div>`;
        Swal.fire({
            title: `Applied: ${s.sponsorName}`,
            html, icon: 'success', width: 560, position: 'top', padding: '1em',
            confirmButtonText: 'Done'
        });
    }

    delete = (s: Sponsorship) => {
        Swal.fire({
            title: 'Delete sponsorship?', icon: 'warning',
            width: 400, position: 'top', padding: '1em',
            showCancelButton: true, confirmButtonColor: '#d33', confirmButtonText: 'Delete'
        }).then(r => {
            if (!r.value) return;
            this.svc.delete(+s.id!).subscribe({
                next: () => { this.toastr.success('Deleted.'); this.load(); },
                error: (err) => this.toastr.error(err.error?.message || 'Error deleting.')
            });
        });
    };

    coverageLabel = (t: any): string =>
        this.coverageTypes.find(x => x.value === t)?.label || '';

    statusBadge = (s: any): string => {
        switch (s) {
            case SponsorshipStatus.Active: return 'bg-success';
            case SponsorshipStatus.Ended: return 'bg-secondary';
            case SponsorshipStatus.Cancelled: return 'bg-danger';
            default: return 'bg-secondary';
        }
    };

    statusLabel = (s: any): string =>
        this.statuses.find(x => x.value === s)?.label || '';

    coverageSummary = (s: Sponsorship): string => {
        if (s.coverageType === SponsorshipCoverageType.FullCoverage) return 'Full';
        if (s.coverageType === SponsorshipCoverageType.Percentage) return `${s.percentage}%`;
        return (s.fixedAmount || 0).toLocaleString();
    };
}
