import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {StrandAddFormComponent} from './strand-add-form/strand-add-form.component';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
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
import {ThemeService} from '../../services/theme.service';

@Component({
    selector: 'app-strands',
    templateUrl: './strands.component.html',
    styleUrl: './strands.component.scss'
})
export class StrandsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(StrandAddFormComponent)
    strandFormComp: StrandAddFormComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFFormComp: SchoolSoftFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

    tableModel: string = 'strand';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/cbe/assessments/strands'], title: 'Strands'}
    ];
    dashboardTitle = 'CBE Assessments: Strands';
    tableTitle: string = ' Strands list';
    tableHeaders: string[] = [
        'Ref#',
        'Curriculum',
        'Learning Level',
        'Subject',
        'Theme',
        'Code',
        'Strand Name',
        'Description',
        'Rank',
        'Action'
    ];

    strand: Strand;
    strands: Strand[] = [];
    subjects: Subject[] = [];
    themes: any[] = [];

    curricula: Curriculum[] = [];
    academicYears: AcademicYear[] = [];
    learningLevels: LearningLevel[] = [];
    educationLevels: EducationLevel[] = [];

    selectedCurriculum: Curriculum;

    firstLoad: boolean = true;

    constructor(
        private toastr: ToastrService,
        private strandSvc: StrandService,
        private subjectsSvc: SubjectsService,
        private curriculaSvc: CurriculumService,
        private learninglevelSvc: LearningLevelsService,
        private educationlevelSvc: EducationLevelService,
        private educationlevelSubjectsSvc: EducationLevelSubjectService,
        private academicYearSvc: AcademicYearsService,
        private themeSvc: ThemeService
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
        this.strandFormComp.editMode = false;
    };

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

                this.academicYears = academicYears.sort(
                    (a, b) => a.rank - b.rank
                );
                this.educationLevels = educationLevels.sort(
                    (a, b) => a.rank - b.rank
                );

                let topAcademicYear = this.academicYears.find(
                    (y) => y.status == true
                );
                let topCurriculum = this.curricula[0];

                this.learninglevelSvc
                    .getLearningLevelsByCurriculum(parseInt(topCurriculum.id))
                    .subscribe({
                        next: (learnLvls) => {
                            this.learningLevels = learnLvls.sort(
                                (a, b) => a.rank - b.rank
                            );
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

    academicYearAddFormChanged = (acadYearId: number) => {
        this.subjects = this.learningLevels = [];
        if (!acadYearId) {
            return;
        }
        let curId = this.strandFormComp.strandForm.get('curriculumId')?.value;
        if (curId) {
            this.curriculumAcademicYearChanged(curId, acadYearId);
        }
    };

    academicYearFilterFormChanged = (acadYearId: number) => {
        this.subjects = this.learningLevels = this.strands = [];
        this.ssFFormComp.schoolSoftFilterForm.get('learningLevelId')?.reset();
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId')?.reset();
        if (!acadYearId) {
            return;
        }
        let curId =
            this.ssFFormComp.schoolSoftFilterForm.get('curriculumId')?.value;
        if (curId) {
            this.curriculumAcademicYearChanged(curId, acadYearId);
        }
    };

    curriculumAddFormChanged = (currId: number) => {
        this.subjects = this.learningLevels = [];
        let acadYearId =
            this.strandFormComp.strandForm.get('academicYearId')?.value;

        if (!acadYearId) {
            this.toastr.info('Please select academic year.');
            return;
        }
        this.curriculumAcademicYearChanged(currId, acadYearId);
    };

    curriculumFilterFormChanged = (currId: number) => {
        this.subjects = this.learningLevels = this.strands = [];
        this.ssFFormComp.schoolSoftFilterForm.get('learningLevelId')?.reset();
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId')?.reset();
        let acadYearId =
            this.ssFFormComp.schoolSoftFilterForm.get('academicYearId')?.value;

        if (!acadYearId) {
            this.toastr.info('Please select academic year.');
            return;
        }
        this.curriculumAcademicYearChanged(currId, acadYearId);
    };

    curriculumAcademicYearChanged = (currId: number, acadId: number) => {
        let learnLvlsReq =
            this.learninglevelSvc.getLearningLevelsByCurriculum(currId);

        forkJoin([learnLvlsReq]).subscribe({
            next: ([learnLvls]) => {
                this.learningLevels = learnLvls.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    learningLevelAddFormChanged = (learnLvlId: number) => {
        this.subjects = [];
        let acadYearId =
            this.strandFormComp.strandForm.get('academicYearId')?.value;
        if (!learnLvlId || !acadYearId) return;
        let learnLvl = this.learningLevels.find(
            (l) => parseInt(l.id) == learnLvlId
        );
        this.educationlevelSubjectsSvc
            .getByEducationLevelAndAcademicYear(
                learnLvl.educationLevelId,
                acadYearId
            )
            .subscribe({
                next: (elSubjects) => {
                    this.subjects = elSubjects
                        .map((es) => es.subject)
                        .sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
    };

    learningLevelFilterFormChanged = (learnLvlId: number) => {
        this.subjects = this.strands = [];
        let learnLvl = this.learningLevels.find(
            (l) => parseInt(l.id) == learnLvlId
        );
        this.ssFFormComp.schoolSoftFilterForm.get('subjectId').setValue(null);
        let acadYearId =
            this.ssFFormComp.schoolSoftFilterForm.get('academicYearId').value;
        this.educationlevelSubjectsSvc
            .getByEducationLevelAndAcademicYear(
                learnLvl.educationLevelId,
                acadYearId
            )
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
        this.strands = [];
    };

    searchClicked = (cys: SchoolSoftFilter) => {
        let searchStr = `/strands/bySubjectId?academicYearId=${cys.academicYearId ?? ''}&learningLvlId=${cys.learningLevelId ?? ''}&subjectId=${cys.subjectId ?? ''}`;
        this.strandSvc.get(searchStr).subscribe({
            next: (strands) => {
                this.strands = strands.sort((a, b) => a.rank - b.rank);
                if (this.strands.length <= 0 && !this.firstLoad) {
                    this.toastr.info(
                        'No record found for the selected search items!'
                    );
                }
                this.firstLoad = false;
                this.isAuthLoading = false;
            },
            error: (err) => this.toastr.error(err.error)
        });
        // Load themes for the form dropdown
        if (cys.subjectId && cys.learningLevelId) {
            this.themeSvc.get(`/themes/bySubjectId?subjectId=${cys.subjectId}&learningLvlId=${cys.learningLevelId}`).subscribe({
                next: (themes) => { this.themes = themes.sort((a, b) => a.rank - b.rank); },
                error: () => { this.themes = []; }
            });
        }
    };

    editItem(id: number) {
        this.strandSvc.getById(id, '/strands').subscribe(
            (res) => {
                let strandId = res.id;
                this.strand = new Strand(res);
                this.strand.id = strandId;

                let learningLevelreq = this.learninglevelSvc.getById(
                    this.strand.learningLevelId,
                    '/learningLevels'
                );
                let subjectreq = this.subjectsSvc.getById(
                    this.strand.subjectId,
                    '/subjects'
                );

                forkJoin([learningLevelreq, subjectreq]).subscribe({
                    next: ([learningLevel, subject]) => {
                        this.learningLevels = [];
                        this.learningLevels.push(learningLevel);
                        this.strand.learningLevel = learningLevel;
                        this.subjects = [];
                        this.subjects.push(subject);
                        this.strand.subject = subject;
                        if (this.strand.curriculum) {
                            this.curricula = [];
                            this.curricula.push(this.strand.curriculum);
                        }
                        this.strandFormComp.setFormControls(this.strand);
                        this.strandFormComp.editMode = true;
                        this.strandFormComp.strand = this.strand;
                        this.tableButton.onClick();
                    },
                    error: (err) => this.toastr.error(err.error)
                });
            },
            (err) => {
                this.toastr.error(err.error?.message || err.message || 'An error occurred.');
            }
        );
    }

    deleteItem(strand: Strand) {
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
                this.strandSvc
                    .delete('/strands', parseInt(strand.id))
                    .subscribe(
                        (res) => {
                            let acadYearId =
                                this.ssFFormComp.schoolSoftFilterForm.get(
                                    'academicYearId'
                                ).value;
                            let currId =
                                this.ssFFormComp.schoolSoftFilterForm.get(
                                    'curriculumId'
                                ).value;
                            let llId =
                                this.ssFFormComp.schoolSoftFilterForm.get(
                                    'learningLevelId'
                                ).value;
                            let subjId =
                                this.ssFFormComp.schoolSoftFilterForm.get(
                                    'subjectId'
                                ).value;

                            this.searchStrands(
                                acadYearId,
                                currId,
                                llId,
                                subjId
                            );
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
        this.strandFormComp.editMode = false;
        this.strandFormComp.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addStrand = (strand: Strand) => {
        Swal.fire({
            title: `${this.strandFormComp.editMode ? 'Update' : 'Add'} strand?`,
            text: `Confirm if you want to ${
                this.strandFormComp.editMode ? 'update' : 'add'
            } strand.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.strandFormComp.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new Strand(strand);
                if (this.strandFormComp.editMode) app.id = strand.id;
                let reqToProcess = this.strandFormComp.editMode
                    ? this.strandSvc.update('/strands', app)
                    : this.strandSvc.create('/strands', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.strandFormComp.editMode = false;
                        this.strandFormComp.refreshItems();
                        this.toastr.success('Strand saved successfully');
                        this.searchStrands(
                            strand.academicYearId,
                            strand.curriculumId,
                            strand.learningLevelId,
                            strand.subjectId
                        );
                        this.closeButton.nativeElement.click();
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
