import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {SubStrandAddFormComponent} from './sub-strand-add-form/sub-strand-add-form.component';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
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
    selector: 'app-sub-strands',
    templateUrl: './sub-strands.component.html',
    styleUrl: './sub-strands.component.scss'
})
export class SubStrandsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(SubStrandAddFormComponent)
    subStrandFormComp: SubStrandAddFormComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFFormComp: SchoolSoftFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

    tableModel: string = 'subStrand';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/cbe/assessments/sub-strands'], title: 'Sub-Strands'}
    ];
    dashboardTitle = 'CBE Assessments: Sub-Strands';
    tableTitle: string = ' Sub-Strands list';
    tableHeaders: string[] = [
        'Ref#',
        'Strand',
        'Code',
        'Sub-Strand Name',
        'Description',
        'Rank',
        'Action'
    ];

    subStrand: SubStrand;
    subStrands: SubStrand[] = [];
    strands: Strand[] = [];
    subjects: Subject[] = [];

    curricula: Curriculum[] = [];
    academicYears: AcademicYear[] = [];
    learningLevels: LearningLevel[] = [];
    educationLevels: EducationLevel[] = [];

    // Separate arrays for the add/edit form cascading dropdowns
    formStrands: Strand[] = [];
    formSubjects: Subject[] = [];
    formLearningLevels: LearningLevel[] = [];

    selectedStrandId: any;
    firstLoad: boolean = true;

    constructor(
        private toastr: ToastrService,
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

    searchStrands = (
        acadId: number,
        currId: number,
        llId: number,
        subjId: number
    ) => {
        let cysPass = new SchoolSoftFilter();
        cysPass.academicYearId = acadId;
        cysPass.curriculumId = currId;
        cysPass.learningLevelId = llId;
        cysPass.subjectId = subjId;
        this.firstLoad = true;
        this.ssFFormComp.setFormControls(cysPass);
        this.ssFFormComp.onSubmit();
        this.isAuthLoading = false;
        this.subStrandFormComp.editMode = false;
    };

    academicYearFilterFormChanged = (acadYearId: number) => {
        this.subjects = this.learningLevels = this.strands = this.subStrands = [];
        this.selectedStrandId = null;
        this.ssFFormComp.schoolSoftFilterForm.get('learningLevelId')?.reset();
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId')?.reset();
        if (!acadYearId) return;
        let curId = this.ssFFormComp.schoolSoftFilterForm.get('curriculumId')?.value;
        if (curId) {
            this.curriculumAcademicYearChanged(curId, acadYearId);
        }
    };

    curriculumFilterFormChanged = (currId: number) => {
        this.subjects = this.learningLevels = this.strands = this.subStrands = [];
        this.selectedStrandId = null;
        this.ssFFormComp.schoolSoftFilterForm.get('learningLevelId')?.reset();
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId')?.reset();
        let acadYearId = this.ssFFormComp.schoolSoftFilterForm.get('academicYearId')?.value;
        if (!acadYearId) {
            this.toastr.info('Please select academic year.');
            return;
        }
        this.curriculumAcademicYearChanged(currId, acadYearId);
    };

    curriculumAcademicYearChanged = (currId: number, acadId: number) => {
        this.learninglevelSvc.getLearningLevelsByCurriculum(currId).subscribe({
            next: (learnLvls) => {
                this.learningLevels = learnLvls.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    learningLevelFilterFormChanged = (learnLvlId: number) => {
        this.subjects = this.strands = this.subStrands = [];
        this.selectedStrandId = null;
        let learnLvl = this.learningLevels.find((l) => parseInt(l.id) == learnLvlId);
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId').setValue(null);
        let acadYearId = this.ssFFormComp.schoolSoftFilterForm.get('academicYearId').value;
        this.educationlevelSubjectsSvc
            .getByEducationLevelAndAcademicYear(learnLvl.educationLevelId, acadYearId)
            .subscribe({
                next: (elSubjects) => {
                    this.subjects = elSubjects
                        .map((es) => es.subject)
                        .sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
    };

    subjectFilterFormChanged = (id: number) => {
        this.strands = this.subStrands = [];
        this.selectedStrandId = null;
    };

    searchClicked = (cys: SchoolSoftFilter) => {
        let searchStr = `/strands/bySubjectId?academicYearId=${cys.academicYearId ?? ''}&learningLvlId=${cys.learningLevelId ?? ''}&subjectId=${cys.subjectId ?? ''}`;
        this.strandSvc.get(searchStr).subscribe({
            next: (strands) => {
                this.strands = strands.sort((a, b) => a.rank - b.rank);
                this.subStrands = [];
                this.selectedStrandId = null;
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
        if (this.selectedStrandId) {
            this.loadSubStrands();
        }
    };

    loadSubStrands = () => {
        if (this.selectedStrandId === 'all') {
            let requests = this.strands.map((s) =>
                this.subStrandSvc.get(`/subStrands/byStrandId/${s.id}`)
            );
            forkJoin(requests).subscribe({
                next: (results) => {
                    this.subStrands = results
                        .reduce((acc, arr) => acc.concat(arr), [])
                        .sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
        } else {
            this.subStrandSvc.get(`/subStrands/byStrandId/${this.selectedStrandId}`).subscribe({
                next: (subStrands) => {
                    this.subStrands = subStrands.sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
        }
    };

    // ---- Add-form cascading handlers ----

    academicYearAddFormChanged = (acadYearId: number) => {
        this.formSubjects = this.formLearningLevels = this.formStrands = [];
        if (!acadYearId) return;
        let curId = this.subStrandFormComp.subStrandForm.get('curriculumId')?.value;
        if (curId) {
            this.formCurriculumAcademicYearChanged(curId, acadYearId);
        }
    };

    curriculumAddFormChanged = (currId: number) => {
        this.formSubjects = this.formLearningLevels = this.formStrands = [];
        let acadYearId = this.subStrandFormComp.subStrandForm.get('academicYearId')?.value;
        if (!acadYearId) {
            this.toastr.info('Please select academic year.');
            return;
        }
        this.formCurriculumAcademicYearChanged(currId, acadYearId);
    };

    formCurriculumAcademicYearChanged = (currId: number, acadId: number) => {
        this.learninglevelSvc.getLearningLevelsByCurriculum(currId).subscribe({
            next: (learnLvls) => {
                this.formLearningLevels = learnLvls.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    learningLevelAddFormChanged = (learnLvlId: number) => {
        this.formSubjects = this.formStrands = [];
        let acadYearId = this.subStrandFormComp.subStrandForm.get('academicYearId')?.value;
        if (!learnLvlId || !acadYearId) return;
        let learnLvl = this.formLearningLevels.find((l) => parseInt(l.id) == learnLvlId);
        this.educationlevelSubjectsSvc
            .getByEducationLevelAndAcademicYear(learnLvl.educationLevelId, acadYearId)
            .subscribe({
                next: (elSubjects) => {
                    this.formSubjects = elSubjects
                        .map((es) => es.subject)
                        .sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
    };

    subjectAddFormChanged = (subjectId: number) => {
        this.formStrands = [];
        if (!subjectId) return;
        let learningLevelId = this.subStrandFormComp.subStrandForm.get('learningLevelId')?.value;
        let searchStr = `/strands/bySubjectId?learningLvlId=${learningLevelId ?? ''}&subjectId=${subjectId}`;
        this.strandSvc.get(searchStr).subscribe({
            next: (strands) => {
                this.formStrands = strands.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    // ---- Edit item ----

    editItem(id: number) {
        this.subStrandSvc.getById(id, '/subStrands').subscribe(
            (res) => {
                let subStrandId = res.id;
                this.subStrand = new SubStrand(res);
                this.subStrand.id = subStrandId;

                // Load the strand to get its related data for the form
                this.strandSvc.getById(this.subStrand.strandId, '/strands').subscribe(
                    (strand) => {
                        this.subStrand.academicYearId = this.ssFFormComp.schoolSoftFilterForm.get('academicYearId')?.value;
                        this.subStrand.curriculumId = strand.curriculumId;
                        this.subStrand.learningLevelId = strand.learningLevelId;
                        this.subStrand.subjectId = strand.subjectId;

                        // Populate form dropdowns for edit
                        this.formLearningLevels = this.learningLevels.length > 0
                            ? this.learningLevels
                            : [strand.learningLevel].filter(Boolean);
                        this.formSubjects = this.subjects.length > 0
                            ? this.subjects
                            : [strand.subject].filter(Boolean);
                        this.formStrands = this.strands.length > 0
                            ? this.strands
                            : [strand].filter(Boolean);

                        this.subStrandFormComp.setFormControls(this.subStrand);
                        this.subStrandFormComp.editMode = true;
                        this.subStrandFormComp.subStrand = this.subStrand;
                        this.tableButton.onClick();
                    },
                    (err) => this.toastr.error(err.error)
                );
            },
            (err) => {
                this.toastr.error(err.error?.message || err.message || 'An error occurred.');
            }
        );
    }

    deleteItem(subStrand: SubStrand) {
        Swal.fire({
            title: `Delete record?`,
            text: `Confirm if you want to delete record.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `Delete`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.subStrandSvc
                    .delete('/subStrands', parseInt(subStrand.id))
                    .subscribe(
                        (res) => {
                            this.loadSubStrands();
                            this.toastr.success('Record deleted successfully!');
                        },
                        (err) => {
                            this.toastr.error(err.error?.message || err.message || 'An error occurred.');
                        }
                    );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    resetForm = () => {
        this.subStrandFormComp.editMode = false;
        this.subStrandFormComp.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addSubStrand = (subStrand: SubStrand) => {
        Swal.fire({
            title: `${this.subStrandFormComp.editMode ? 'Update' : 'Add'} sub-strand?`,
            text: `Confirm if you want to ${
                this.subStrandFormComp.editMode ? 'update' : 'add'
            } sub-strand.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.subStrandFormComp.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new SubStrand(subStrand);
                if (this.subStrandFormComp.editMode) app.id = subStrand.id;
                let reqToProcess = this.subStrandFormComp.editMode
                    ? this.subStrandSvc.update('/subStrands', app)
                    : this.subStrandSvc.create('/subStrands', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.subStrandFormComp.editMode = false;
                        this.toastr.success('Sub-strand saved successfully');
                        this.closeButton.nativeElement.click();

                        // Set the main filter to match what was just saved
                        let acadYearId = subStrand.academicYearId;
                        let currId = subStrand.curriculumId;
                        let llId = subStrand.learningLevelId;
                        let subjId = subStrand.subjectId;
                        let strandId = subStrand.strandId;

                        // Update the filter form values
                        let cysPass = new SchoolSoftFilter();
                        cysPass.academicYearId = acadYearId;
                        cysPass.curriculumId = currId;
                        cysPass.learningLevelId = llId;
                        cysPass.subjectId = subjId;

                        // Reload learning levels and subjects for the filter
                        this.learninglevelSvc.getLearningLevelsByCurriculum(currId).subscribe({
                            next: (learnLvls) => {
                                this.learningLevels = learnLvls.sort((a, b) => a.rank - b.rank);
                                let learnLvl = this.learningLevels.find((l) => parseInt(l.id) == llId);
                                if (learnLvl) {
                                    this.educationlevelSubjectsSvc
                                        .getByEducationLevelAndAcademicYear(learnLvl.educationLevelId, acadYearId)
                                        .subscribe({
                                            next: (elSubjects) => {
                                                this.subjects = elSubjects
                                                    .map((es) => es.subject)
                                                    .sort((a, b) => a.rank - b.rank);

                                                // Set the filter form and trigger search
                                                this.ssFFormComp.setFormControls(cysPass);

                                                // Load strands for this subject and select the saved strand
                                                let searchStr = `/strands/bySubjectId?academicYearId=${acadYearId ?? ''}&learningLvlId=${llId ?? ''}&subjectId=${subjId ?? ''}`;
                                                this.strandSvc.get(searchStr).subscribe({
                                                    next: (strands) => {
                                                        this.strands = strands.sort((a, b) => a.rank - b.rank);
                                                        this.selectedStrandId = strandId;
                                                        this.loadSubStrands();
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

                        this.subStrandFormComp.refreshItems();
                    },
                    (err) => {
                        this.toastr.error(err.error);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };
}
