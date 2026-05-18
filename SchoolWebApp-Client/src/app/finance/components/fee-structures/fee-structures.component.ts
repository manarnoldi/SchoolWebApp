import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {FeeStructure, FeeStructureItem, FeeCategory} from '@/finance/models/finance-models';
import {FeeStructureService, FeeCategoryService} from '@/finance/services/finance-services';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {EducationLevelService} from '@/school/services/education-level.service';

@Component({
    selector: 'app-finance-fee-structures',
    templateUrl: './fee-structures.component.html'
})
export class FeeStructuresComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/fee-structures'], title: 'Fee Structures'}
    ];
    dashboardTitle = 'Finance: Fee Structures';

    structures: FeeStructure[] = [];
    categories: FeeCategory[] = [];
    academicYears: any[] = [];
    allSessions: any[] = [];
    sessions: any[] = [];
    educationLevels: any[] = [];
    allLearningLevels: any[] = [];
    learningLevels: any[] = [];

    item: FeeStructure = new FeeStructure({isActive: true, items: []});
    editMode = false;
    showForm = false;
    viewDetail: FeeStructure | null = null;
    applyToAllLevels = false;
    applyToAllSessions = false;
    formEducationLevelId: any = null;
    formLearningLevels: any[] = [];
    formSessions: any[] = [];

    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterEducationLevelId: any = null;
    filterLearningLevelId: any = null;
    filterActive: any = null;

    page = 1;
    pageSize = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(
        private toastr: ToastrService,
        private svc: FeeStructureService,
        private catSvc: FeeCategoryService,
        private yearSvc: AcademicYearsService,
        private sessionSvc: SessionsService,
        private llSvc: LearningLevelsService,
        private elSvc: EducationLevelService
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.svc.get('/feeStructures'),
            this.catSvc.get('/feeCategories'),
            this.yearSvc.get('/academicYears'),
            this.sessionSvc.get('/sessions'),
            this.llSvc.get('/learningLevels'),
            this.elSvc.get('/educationLevels')
        ]).subscribe({
            next: ([structures, cats, years, sess, levels, edLevels]: any) => {
                this.structures = structures || [];
                this.categories = (cats || []).filter((c: any) => c.isActive).sort((a: any, b: any) => (a.rank || 0) - (b.rank || 0));
                this.academicYears = (years || []).sort((a: any, b: any) => (b.rank || 0) - (a.rank || 0));
                this.allSessions = sess || [];
                this.allLearningLevels = (levels || []).sort((a: any, b: any) => (a.rank || 0) - (b.rank || 0));
                this.learningLevels = this.allLearningLevels;
                this.educationLevels = (edLevels || []).sort((a: any, b: any) => (a.rank || 0) - (b.rank || 0));
                let activeYear = this.academicYears.find((y: any) => y.status === true);
                this.filterAcademicYearId = activeYear ? activeYear.id : (this.academicYears[0]?.id || null);
                this.onAcademicYearChange();
                // Default to active session within the year
                let activeSession = this.sessions.find((s: any) => s.status === true);
                if (activeSession) this.filterSessionId = activeSession.id;
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    load = () => {
        this.svc.get('/feeStructures').subscribe({
            next: (r) => this.structures = r || [],
            error: (err) => this.toastr.error(err.error)
        });
    };

    filtered = (): FeeStructure[] => {
        return (this.structures || []).filter((s) => {
            if (this.filterAcademicYearId != null && s.academicYearId != this.filterAcademicYearId) return false;
            if (this.filterSessionId != null && s.sessionId != this.filterSessionId) return false;
            if (this.filterEducationLevelId != null) {
                let ll = this.allLearningLevels.find((l: any) => l.id == s.learningLevelId);
                if (!ll || ll.educationLevelId != this.filterEducationLevelId) return false;
            }
            if (this.filterLearningLevelId != null && s.learningLevelId != this.filterLearningLevelId) return false;
            if (this.filterActive != null && s.isActive !== this.filterActive) return false;
            return true;
        }).sort((a, b) => {
            let llA = this.allLearningLevels.find((l: any) => l.id == a.learningLevelId);
            let llB = this.allLearningLevels.find((l: any) => l.id == b.learningLevelId);
            return (llA?.rank || 0) - (llB?.rank || 0);
        });
    };

    onAcademicYearChange = () => {
        this.sessions = this.filterAcademicYearId
            ? this.allSessions.filter((s: any) => s.academicYearId == this.filterAcademicYearId)
            : this.allSessions;
        this.filterSessionId = null;
        this.page = 1;
    };

    onFilterEducationLevelChange = () => {
        this.learningLevels = this.filterEducationLevelId
            ? this.allLearningLevels.filter((l: any) => l.educationLevelId == this.filterEducationLevelId)
            : this.allLearningLevels;
        this.filterLearningLevelId = null;
        this.page = 1;
    };

    onFormAcademicYearChange = () => {
        this.formSessions = this.item.academicYearId
            ? this.allSessions.filter((s: any) => s.academicYearId == this.item.academicYearId)
            : this.allSessions;
        this.item.sessionId = undefined;
        this.applyToAllSessions = false;
    };

    onFormSessionChange = () => {
    };

    onApplyToAllSessionsChange = () => {
        if (this.applyToAllSessions) {
            this.item.sessionId = undefined;
        }
    };

    onFormEducationLevelChange = () => {
        this.formLearningLevels = this.formEducationLevelId
            ? this.allLearningLevels.filter((l: any) => l.educationLevelId == this.formEducationLevelId)
            : this.allLearningLevels;
        this.item.learningLevelId = undefined;
        this.applyToAllLevels = false;
        this.autoGenerateName();
    };

    onFormLearningLevelChange = () => {
        this.autoGenerateName();
    };

    onApplyToAllChange = () => {
        if (this.applyToAllLevels) {
            this.item.learningLevelId = undefined;
        }
        this.autoGenerateName();
    };

    autoGenerateName = () => {
        // Name is always left blank in the form (shows "Auto" placeholder).
        // Actual name is built at save time per learning level.
    };

    buildName = (levelId: any, sessionId: any): string => {
        let parts: string[] = [];
        let year = this.academicYears.find((y: any) => y.id == this.item.academicYearId);
        if (year) parts.push(year.name);
        let sess = this.allSessions.find((s: any) => s.id == sessionId);
        if (sess) parts.push(sess.sessionName);
        let ll = this.allLearningLevels.find((l: any) => l.id == levelId);
        if (ll) parts.push(ll.name);
        parts.push('Fee Structure');
        return parts.join(' ');
    };

    clearFilters = () => {
        let activeYear = this.academicYears.find((y: any) => y.status === true);
        this.filterAcademicYearId = activeYear ? activeYear.id : (this.academicYears[0]?.id || null);
        this.onAcademicYearChange();
        let activeSession = this.sessions.find((s: any) => s.status === true);
        this.filterSessionId = activeSession ? activeSession.id : null;
        this.filterEducationLevelId = null;
        this.onFilterEducationLevelChange();
        this.filterActive = null;
        this.page = 1;
    };

    addNew = () => {
        this.item = new FeeStructure({isActive: true, items: []});
        this.formEducationLevelId = null;
        this.formLearningLevels = this.allLearningLevels;
        this.formSessions = [];
        this.applyToAllLevels = false;
        this.applyToAllSessions = false;
        this.editMode = false;
        this.showForm = true;
    };

    edit = (s: FeeStructure) => {
        this.svc.getById(parseInt(s.id), '/feeStructures').subscribe({
            next: (raw: any) => {
                let full: any = Array.isArray(raw) ? raw[0] : raw;
                this.item = new FeeStructure(full);
                this.item.items = (full?.items || []).map((i: any) => Object.assign(new FeeStructureItem(), i));
                // Set education level from the learning level
                let ll = this.allLearningLevels.find((l: any) => l.id == this.item.learningLevelId);
                this.formEducationLevelId = ll ? ll.educationLevelId : null;
                this.formLearningLevels = this.formEducationLevelId
                    ? this.allLearningLevels.filter((l: any) => l.educationLevelId == this.formEducationLevelId)
                    : this.allLearningLevels;
                this.applyToAllLevels = false;
                this.applyToAllSessions = false;
                // Load sessions for the academic year
                this.formSessions = this.item.academicYearId
                    ? this.allSessions.filter((ss: any) => ss.academicYearId == this.item.academicYearId)
                    : this.allSessions;
                this.editMode = true;
                this.showForm = true;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    cancel = () => { this.showForm = false; };

    viewItems = (s: FeeStructure) => {
        this.svc.getById(parseInt(s.id), '/feeStructures').subscribe({
            next: (raw: any) => {
                let full: any = Array.isArray(raw) ? raw[0] : raw;
                this.viewDetail = new FeeStructure(full);
                this.viewDetail.items = (full?.items || []).map((i: any) => Object.assign(new FeeStructureItem(), i));
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    closeDetail = () => { this.viewDetail = null; };

    addItem = () => {
        if (!this.item.items) this.item.items = [];
        this.item.items.push(Object.assign(new FeeStructureItem(), {amount: 0, isMandatory: true}));
    };

    removeItem = (i: FeeStructureItem) => {
        let idx = (this.item.items || []).indexOf(i);
        if (idx >= 0) this.item.items!.splice(idx, 1);
    };

    getItemTotal = (): number => {
        return (this.item.items || []).reduce((s, i) => s + (+i.amount! || 0), 0);
    };

    getCategoryLabel = (categoryId: any): string => {
        let c = this.categories.find((x) => x.id == categoryId);
        return c ? c.name! : '';
    };

    getEducationLevelName = (learningLevelId: any): string => {
        let ll = this.allLearningLevels.find((l: any) => l.id == learningLevelId);
        if (!ll) return '';
        let el = this.educationLevels.find((e: any) => e.id == ll.educationLevelId);
        return el ? el.name : '';
    };

    save = () => {
        if (!this.item.academicYearId) {
            this.toastr.info('Academic year is required.');
            return;
        }
        if (!this.formEducationLevelId) {
            this.toastr.info('Education level is required.');
            return;
        }
        if (!this.applyToAllLevels && !this.item.learningLevelId) {
            this.toastr.info('Select a learning level or check "Apply to all learning levels".');
            return;
        }
        if (!this.item.items || this.item.items.length === 0) {
            this.toastr.info('Add at least one fee item.');
            return;
        }
        let dupAcct = new Set<number>();
        for (let it of this.item.items) {
            if (!it.feeCategoryId) { this.toastr.info('Each item must have a fee category.'); return; }
            if (dupAcct.has(+it.feeCategoryId)) { this.toastr.info('Duplicate fee category in items.'); return; }
            dupAcct.add(+it.feeCategoryId);
        }

        let targetLevels = this.applyToAllLevels
            ? this.formLearningLevels
            : [{id: this.item.learningLevelId}];

        let targetSessions = this.applyToAllSessions
            ? this.formSessions
            : [{id: this.item.sessionId}];

        if (this.editMode) {
            // Single update — rebuild name from current level + session
            let payload: any = {
                Name: this.buildName(this.item.learningLevelId, this.item.sessionId),
                AcademicYearId: this.item.academicYearId,
                SessionId: this.item.sessionId,
                LearningLevelId: this.item.learningLevelId,
                Description: this.item.description,
                IsActive: this.item.isActive,
                Items: (this.item.items || []).map((i) => ({
                    Id: i.id || null,
                    FeeCategoryId: i.feeCategoryId,
                    Amount: +i.amount! || 0,
                    IsMandatory: !!i.isMandatory
                }))
            };
            this.svc.updateById(+this.item.id, payload).subscribe({
                next: () => { this.toastr.success('Fee structure saved.'); this.showForm = false; this.load(); },
                error: (err) => this.toastr.error(err.error?.message || 'Error saving.')
            });
        } else {
            // Create — cartesian product of sessions × levels
            let combos: {sessionId: any, levelId: any}[] = [];
            for (let sess of targetSessions) {
                for (let ll of targetLevels) {
                    combos.push({sessionId: sess.id, levelId: ll.id});
                }
            }
            let created = 0;
            let errors = 0;
            let total = combos.length;
            for (let combo of combos) {
                let name = this.buildName(combo.levelId, combo.sessionId);
                let payload = {
                    Name: name,
                    AcademicYearId: this.item.academicYearId,
                    SessionId: combo.sessionId,
                    LearningLevelId: combo.levelId,
                    Description: this.item.description,
                    IsActive: this.item.isActive,
                    Items: (this.item.items || []).map((i) => ({
                        FeeCategoryId: i.feeCategoryId,
                        Amount: +i.amount! || 0,
                        IsMandatory: !!i.isMandatory
                    }))
                };
                this.svc.createFeeStructure(payload).subscribe({
                    next: () => {
                        created++;
                        if (created + errors === total) {
                            this.toastr.success(`${created} fee structure(s) created.`);
                            this.showForm = false;
                            this.load();
                        }
                    },
                    error: () => {
                        errors++;
                        if (created + errors === total) {
                            this.toastr.warning(`${created} created, ${errors} failed.`);
                            this.showForm = false;
                            this.load();
                        }
                    }
                });
            }
        }
    };

    delete = (s: FeeStructure) => {
        Swal.fire({title: 'Delete fee structure?', icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (r.value) {
                this.svc.delete('/feeStructures', parseInt(s.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    };
}
