import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {PCIAddFormComponent} from './pci-add-form/pci-add-form.component';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {PCI} from '../../models/pci';
import {PCIService} from '../../services/pci.service';
import {SubStrand} from '../../models/sub-strand';
import {SubStrandService} from '../../services/sub-strand.service';
import {Strand} from '../../models/strand';
import {StrandService} from '../../services/strand.service';
import {Subject} from '@/academics/models/subject';
import {SubjectsService} from '@/academics/services/subjects.service';
import {AcademicYear} from '@/school/models/academic-year';
import {Curriculum} from '@/academics/models/curriculum';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {LearningLevel} from '@/class/models/learning-level';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {EducationLevel} from '@/school/models/educationLevel';
import {EducationLevelService} from '@/school/services/education-level.service';
import {EducationLevelSubjectService} from '@/academics/services/education-level-subject.service';

@Component({
    selector: 'app-pcis',
    templateUrl: './pcis.component.html',
    styleUrl: './pcis.component.scss'
})
export class PCIsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(PCIAddFormComponent)
    pciFormComp: PCIAddFormComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFFormComp: SchoolSoftFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

    tableModel: string = 'pci';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/cbe/assessments/pcis'], title: 'Pertinent & Contemporary Issues'}
    ];
    dashboardTitle = 'CBE Assessments: Pertinent & Contemporary Issues (PCIs)';
    tableTitle: string = ' PCIs list';
    tableHeaders: string[] = [
        'Ref#',
        'PCI',
        'Sub-Strand',
        'Rank',
        'Action'
    ];

    pci: PCI;
    pcis: PCI[] = [];
    strands: Strand[] = [];
    subStrands: SubStrand[] = [];
    subjects: Subject[] = [];

    curricula: Curriculum[] = [];
    academicYears: AcademicYear[] = [];
    learningLevels: LearningLevel[] = [];
    educationLevels: EducationLevel[] = [];

    formStrands: Strand[] = [];
    formSubStrands: SubStrand[] = [];
    formSubjects: Subject[] = [];
    formLearningLevels: LearningLevel[] = [];

    selectedStrandId: any;
    selectedSubStrandId: any;
    firstLoad: boolean = true;

    constructor(
        private toastr: ToastrService,
        private pciSvc: PCIService,
        private subStrandSvc: SubStrandService,
        private strandSvc: StrandService,
        private subjectsSvc: SubjectsService,
        private curriculaSvc: CurriculumService,
        private learninglevelSvc: LearningLevelsService,
        private educationlevelSvc: EducationLevelService,
        private educationlevelSubjectsSvc: EducationLevelSubjectService,
        private academicYearSvc: AcademicYearsService
    ) {}

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems() {
        let curriculaReq = this.curriculaSvc.get('/curricula');
        let academicYearsReq = this.academicYearSvc.get('/academicYears');
        let educationLevelsReq = this.educationlevelSvc.get('/educationLevels');

        forkJoin([
            curriculaReq,
            academicYearsReq,
            educationLevelsReq
        ]).subscribe({
            next: ([curricula, academicYears, educationLevels]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort((a, b) => a.rank - b.rank);
                this.educationLevels = educationLevels.sort((a, b) => a.rank - b.rank);

                let topAcademicYear = this.academicYears.find((y) => y.status == true);
                let topCurriculum = this.curricula[0];

                this.learninglevelSvc
                    .getLearningLevelsByCurriculum(parseInt(topCurriculum.id))
                    .subscribe({
                        next: (learnLvls) => {
                            this.learningLevels = learnLvls.sort((a, b) => a.rank - b.rank);
                            let topLearningLevel = this.learningLevels[0];
                            this.educationlevelSubjectsSvc
                                .getByEducationLevelAndAcademicYear(
                                    topLearningLevel.educationLevelId,
                                    parseInt(topAcademicYear.id)
                                )
                                .subscribe({
                                    next: (elSubjects) => {
                                        this.subjects = elSubjects
                                            .map((es) => es.subject)
                                            .sort((a, b) => a.rank - b.rank);
                                        let topSubject = this.subjects[0];
                                        this.searchStrands(
                                            parseInt(topAcademicYear.id),
                                            parseInt(topCurriculum.id),
                                            parseInt(topLearningLevel.id),
                                            parseInt(topSubject.id)
                                        );
                                    },
                                    error: (err) => this.toastr.error(err.error)
                                });
                        },
                        error: (err) => this.toastr.error(err.error)
                    });
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    }

    searchStrands = (acadId: number, currId: number, llId: number, subjId: number) => {
        let cysPass = new SchoolSoftFilter();
        cysPass.academicYearId = acadId;
        cysPass.curriculumId = currId;
        cysPass.learningLevelId = llId;
        cysPass.subjectId = subjId;
        this.firstLoad = true;
        this.ssFFormComp.setFormControls(cysPass);
        this.ssFFormComp.onSubmit();
        this.isAuthLoading = false;
        this.pciFormComp.editMode = false;
    };

    academicYearFilterFormChanged = (acadYearId: number) => {
        this.subjects = this.learningLevels = this.strands = [];
        this.subStrands = this.pcis = [];
        this.selectedStrandId = null;
        this.selectedSubStrandId = null;
        this.ssFFormComp.schoolSoftFilterForm.get('learningLevelId')?.reset();
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId')?.reset();
        if (!acadYearId) return;
        let curId = this.ssFFormComp.schoolSoftFilterForm.get('curriculumId')?.value;
        if (curId) { this.curriculumAcademicYearChanged(curId, acadYearId); }
    };

    curriculumFilterFormChanged = (currId: number) => {
        this.subjects = this.learningLevels = this.strands = [];
        this.subStrands = this.pcis = [];
        this.selectedStrandId = null;
        this.selectedSubStrandId = null;
        this.ssFFormComp.schoolSoftFilterForm.get('learningLevelId')?.reset();
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId')?.reset();
        let acadYearId = this.ssFFormComp.schoolSoftFilterForm.get('academicYearId')?.value;
        if (!acadYearId) { this.toastr.info('Please select academic year.'); return; }
        this.curriculumAcademicYearChanged(currId, acadYearId);
    };

    curriculumAcademicYearChanged = (currId: number, acadId: number) => {
        this.learninglevelSvc.getLearningLevelsByCurriculum(currId).subscribe({
            next: (learnLvls) => { this.learningLevels = learnLvls.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    learningLevelFilterFormChanged = (learnLvlId: number) => {
        this.subjects = this.strands = [];
        this.subStrands = this.pcis = [];
        this.selectedStrandId = null;
        this.selectedSubStrandId = null;
        let learnLvl = this.learningLevels.find((l) => parseInt(l.id) == learnLvlId);
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId').setValue(null);
        let acadYearId = this.ssFFormComp.schoolSoftFilterForm.get('academicYearId').value;
        this.educationlevelSubjectsSvc
            .getByEducationLevelAndAcademicYear(learnLvl.educationLevelId, acadYearId)
            .subscribe({
                next: (elSubjects) => {
                    this.subjects = elSubjects.map((es) => es.subject).sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
    };

    subjectFilterFormChanged = (id: number) => {
        this.strands = [];
        this.subStrands = this.pcis = [];
        this.selectedStrandId = null;
        this.selectedSubStrandId = null;
    };

    searchClicked = (cys: SchoolSoftFilter) => {
        let searchStr = `/strands/bySubjectId?academicYearId=${cys.academicYearId ?? ''}&learningLvlId=${cys.learningLevelId ?? ''}&subjectId=${cys.subjectId ?? ''}`;
        this.strandSvc.get(searchStr).subscribe({
            next: (strands) => {
                this.strands = strands.sort((a, b) => a.rank - b.rank);
                this.subStrands = [];
                this.pcis = [];
                this.selectedStrandId = null;
                this.selectedSubStrandId = null;
                if (this.strands.length > 0) {
                    this.selectedStrandId = 'all';
                    this.loadSubStrands();
                } else if (!this.firstLoad) {
                    this.toastr.info('No strands found for the selected search items!');
                }
                this.firstLoad = false;
                this.isAuthLoading = false;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    strandChanged = () => {
        this.subStrands = [];
        this.pcis = [];
        this.selectedSubStrandId = null;
        if (this.selectedStrandId) { this.loadSubStrands(); }
    };

    loadSubStrands = () => {
        if (this.selectedStrandId === 'all') {
            let requests = this.strands.map((s) => this.subStrandSvc.get(`/subStrands/byStrandId/${s.id}`));
            forkJoin(requests).subscribe({
                next: (results) => {
                    this.subStrands = results.reduce((acc, arr) => acc.concat(arr), []).sort((a, b) => a.rank - b.rank);
                    if (this.subStrands.length > 0) { this.selectedSubStrandId = 'all'; this.loadPCIs(); }
                },
                error: (err) => this.toastr.error(err.error)
            });
        } else {
            this.subStrandSvc.get(`/subStrands/byStrandId/${this.selectedStrandId}`).subscribe({
                next: (subStrands) => {
                    this.subStrands = subStrands.sort((a, b) => a.rank - b.rank);
                    if (this.subStrands.length > 0) { this.selectedSubStrandId = 'all'; this.loadPCIs(); }
                },
                error: (err) => this.toastr.error(err.error)
            });
        }
    };

    subStrandChanged = () => {
        this.pcis = [];
        if (this.selectedSubStrandId) { this.loadPCIs(); }
    };

    loadPCIs = () => {
        if (this.selectedSubStrandId === 'all') {
            let requests = this.subStrands.map((ss) => this.pciSvc.get(`/pcis/bySubStrandId/${ss.id}`));
            forkJoin(requests).subscribe({
                next: (results) => {
                    this.pcis = results.reduce((acc, arr) => acc.concat(arr), []).sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
        } else {
            this.pciSvc.get(`/pcis/bySubStrandId/${this.selectedSubStrandId}`).subscribe({
                next: (pcis) => { this.pcis = pcis.sort((a, b) => a.rank - b.rank); },
                error: (err) => this.toastr.error(err.error)
            });
        }
    };

    // ---- Add-form cascading handlers ----

    academicYearAddFormChanged = (acadYearId: number) => {
        this.formSubjects = this.formLearningLevels = this.formStrands = [];
        this.formSubStrands = [];
        if (!acadYearId) return;
        let curId = this.pciFormComp.pciForm.get('curriculumId')?.value;
        if (curId) { this.formCurriculumAcademicYearChanged(curId, acadYearId); }
    };

    curriculumAddFormChanged = (currId: number) => {
        this.formSubjects = this.formLearningLevels = this.formStrands = [];
        this.formSubStrands = [];
        let acadYearId = this.pciFormComp.pciForm.get('academicYearId')?.value;
        if (!acadYearId) { this.toastr.info('Please select academic year.'); return; }
        this.formCurriculumAcademicYearChanged(currId, acadYearId);
    };

    formCurriculumAcademicYearChanged = (currId: number, acadId: number) => {
        this.learninglevelSvc.getLearningLevelsByCurriculum(currId).subscribe({
            next: (learnLvls) => { this.formLearningLevels = learnLvls.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    learningLevelAddFormChanged = (learnLvlId: number) => {
        this.formSubjects = this.formStrands = [];
        this.formSubStrands = [];
        let acadYearId = this.pciFormComp.pciForm.get('academicYearId')?.value;
        if (!learnLvlId || !acadYearId) return;
        let learnLvl = this.formLearningLevels.find((l) => parseInt(l.id) == learnLvlId);
        this.educationlevelSubjectsSvc
            .getByEducationLevelAndAcademicYear(learnLvl.educationLevelId, acadYearId)
            .subscribe({
                next: (elSubjects) => {
                    this.formSubjects = elSubjects.map((es) => es.subject).sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
    };

    subjectAddFormChanged = (subjectId: number) => {
        this.formStrands = [];
        this.formSubStrands = [];
        if (!subjectId) return;
        let learningLevelId = this.pciFormComp.pciForm.get('learningLevelId')?.value;
        let searchStr = `/strands/bySubjectId?learningLvlId=${learningLevelId ?? ''}&subjectId=${subjectId}`;
        this.strandSvc.get(searchStr).subscribe({
            next: (strands) => { this.formStrands = strands.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    strandAddFormChanged = (strandId: number) => {
        this.formSubStrands = [];
        if (!strandId) return;
        this.subStrandSvc.get(`/subStrands/byStrandId/${strandId}`).subscribe({
            next: (subStrands) => { this.formSubStrands = subStrands.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    // ---- Edit item ----

    editItem(id: number) {
        this.pciSvc.getById(id, '/pcis').subscribe(
            (res) => {
                let pciId = res.id;
                this.pci = new PCI(res);
                this.pci.id = pciId;

                this.subStrandSvc.getById(this.pci.subStrandId, '/subStrands').subscribe(
                    (subStrand) => {
                        this.strandSvc.getById(subStrand.strandId, '/strands').subscribe(
                            (strand) => {
                                this.pci.academicYearId = this.ssFFormComp.schoolSoftFilterForm.get('academicYearId')?.value;
                                this.pci.curriculumId = strand.curriculumId;
                                this.pci.learningLevelId = strand.learningLevelId;
                                this.pci.subjectId = strand.subjectId;
                                this.pci.strandId = subStrand.strandId;

                                this.formLearningLevels = this.learningLevels.length > 0 ? this.learningLevels : [strand.learningLevel].filter(Boolean);
                                this.formSubjects = this.subjects.length > 0 ? this.subjects : [strand.subject].filter(Boolean);
                                this.formStrands = this.strands.length > 0 ? this.strands : [strand].filter(Boolean);
                                this.formSubStrands = this.subStrands.length > 0 ? this.subStrands : [subStrand].filter(Boolean);

                                this.pciFormComp.setFormControls(this.pci);
                                this.pciFormComp.editMode = true;
                                this.pciFormComp.pci = this.pci;
                                this.tableButton.onClick();
                            },
                            (err) => this.toastr.error(err.error)
                        );
                    },
                    (err) => this.toastr.error(err.error)
                );
            },
            (err) => { this.toastr.error(err); }
        );
    }

    deleteItem(pci: PCI) {
        Swal.fire({
            title: `Delete record?`,
            text: `Confirm if you want to delete record.`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: `Delete`, cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.pciSvc.delete('/pcis', parseInt(pci.id)).subscribe(
                    (res) => { this.loadPCIs(); this.toastr.success('Record deleted successfully!'); },
                    (err) => { this.toastr.error(err.error?.message || err.message || 'Error deleting record.'); }
                );
            }
        });
    }

    resetForm = () => {
        this.pciFormComp.editMode = false;
        this.pciFormComp.refreshItems();
    };

    errorEvent = (errorName: string) => { this.toastr.error(errorName); };

    addPCI = (pci: PCI) => {
        Swal.fire({
            title: `${this.pciFormComp.editMode ? 'Update' : 'Add'} PCI?`,
            text: `Confirm if you want to ${this.pciFormComp.editMode ? 'update' : 'add'} PCI.`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.pciFormComp.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new PCI(pci);
                if (this.pciFormComp.editMode) app.id = pci.id;
                let reqToProcess = this.pciFormComp.editMode
                    ? this.pciSvc.update('/pcis', app)
                    : this.pciSvc.create('/pcis', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.pciFormComp.editMode = false;
                        this.toastr.success('PCI saved successfully');
                        this.closeButton.nativeElement.click();

                        let acadYearId = pci.academicYearId;
                        let currId = pci.curriculumId;
                        let llId = pci.learningLevelId;
                        let subjId = pci.subjectId;
                        let strandId = pci.strandId;
                        let subStrandId = pci.subStrandId;

                        let cysPass = new SchoolSoftFilter();
                        cysPass.academicYearId = acadYearId;
                        cysPass.curriculumId = currId;
                        cysPass.learningLevelId = llId;
                        cysPass.subjectId = subjId;

                        this.learninglevelSvc.getLearningLevelsByCurriculum(currId).subscribe({
                            next: (learnLvls) => {
                                this.learningLevels = learnLvls.sort((a, b) => a.rank - b.rank);
                                let learnLvl = this.learningLevels.find((l) => parseInt(l.id) == llId);
                                if (learnLvl) {
                                    this.educationlevelSubjectsSvc
                                        .getByEducationLevelAndAcademicYear(learnLvl.educationLevelId, acadYearId)
                                        .subscribe({
                                            next: (elSubjects) => {
                                                this.subjects = elSubjects.map((es) => es.subject).sort((a, b) => a.rank - b.rank);
                                                this.ssFFormComp.setFormControls(cysPass);
                                                let searchStr = `/strands/bySubjectId?academicYearId=${acadYearId ?? ''}&learningLvlId=${llId ?? ''}&subjectId=${subjId ?? ''}`;
                                                this.strandSvc.get(searchStr).subscribe({
                                                    next: (strands) => {
                                                        this.strands = strands.sort((a, b) => a.rank - b.rank);
                                                        this.selectedStrandId = strandId;
                                                        this.subStrandSvc.get(`/subStrands/byStrandId/${strandId}`).subscribe({
                                                            next: (subStrands) => {
                                                                this.subStrands = subStrands.sort((a, b) => a.rank - b.rank);
                                                                this.selectedSubStrandId = subStrandId;
                                                                this.loadPCIs();
                                                            },
                                                            error: (err) => this.toastr.error(err.error)
                                                        });
                                                    },
                                                    error: (err) => this.toastr.error(err.error)
                                                });
                                            },
                                            error: (err) => this.toastr.error(err.error)
                                        });
                                }
                            },
                            error: (err) => this.toastr.error(err.error)
                        });

                        this.pciFormComp.refreshItems();
                    },
                    (err) => { this.toastr.error(err.error); }
                );
            }
        });
    };
}
