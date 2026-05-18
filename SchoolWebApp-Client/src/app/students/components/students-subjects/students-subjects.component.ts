import {Curriculum} from '@/academics/models/curriculum';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import { Status } from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {EducationLevelYear} from '@/shared/models/education-level-year';
import {StudentClass} from '@/students/models/student-class';
import {StudentSubject} from '@/students/models/student-subject';
import {StudentSubjectSearch} from '@/students/models/student-subject-search';
import {StudentClassService} from '@/students/services/student-class.service';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
import {StudentsSubjectsStateService} from '@/students/services/students-subjects-state.service';
import {StudentsSubjectsSearchFormComponent} from './students-subjects-search-form/students-subjects-search-form.component';
import {AfterViewInit, Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-students-subjects',
    templateUrl: './students-subjects.component.html',
    styleUrl: './students-subjects.component.scss'
})
export class StudentsSubjectsComponent implements OnInit, AfterViewInit {
    @ViewChild(StudentsSubjectsSearchFormComponent)
    searchForm: StudentsSubjectsSearchFormComponent;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/students/subjects'], title: 'Students: Subjects'}
    ];
    dashboardTitle = 'Students: Subjects';

    curricula: Curriculum[] = [];
    educationLevels: EducationLevel[] = [];
    academicYears: AcademicYear[] = [];
    schoolClasses: SchoolClass[] = [];
    studentClasses: StudentClass[] = [];

    studentSubjects: StudentSubject[] = [];

    // studentClassId -> subject count, pre-fetched after the SchoolClass filter
    // resolves so the left-hand student list can render a clickable count
    // badge per row.
    subjectCounts: Record<number, number> = {};
    selectedStudentClassId: number | null = null;

    studentsChanged = () => {
        this.studentSubjects = [];
    };

    doneLoading = false;

    constructor(
        private toastr: ToastrService,
        private curriculaSvc: CurriculumService,
        private academicYearsSvc: AcademicYearsService,
        private educationLevelSvc: EducationLevelService,
        private schoolClassSvc: SchoolClassesService,
        private studentClassesSvc: StudentClassService,
        private studentSubjectsSvc: StudentSubjectsService,
        private stateSvc: StudentsSubjectsStateService
    ) {}

    ngOnInit(): void {
        this.loadInitials();
    }

    ngAfterViewInit(): void {
        // searchForm is available after view init; defer restore so its
        // FormGroup is constructed before we try to patch values into it.
        // Wait for loadInitials() to finish populating curricula + academic
        // years too, otherwise the dropdowns can't render the saved selection.
        setTimeout(() => this.restoreStateIfAny(), 0);
    }

    loadInitials = () => {
        let curriculaReq = this.curriculaSvc.get('/curricula');
        let academicYearsReq = this.academicYearsSvc.get('/academicYears');

        forkJoin([curriculaReq, academicYearsReq]).subscribe({
            next: ([curricula, academicYears]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort(
                    (a, b) => b.rank - a.rank
                );
                this.doneLoading = true;
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    curriculumChanged = (curriculumId: number) => {
        this.studentSubjects = [];
        // Curriculum changed -> downstream selections invalidated.
        this.stateSvc.set({
            curriculumId,
            educationLevelId: null,
            schoolClassId: null,
            selectedStudentClassId: null
        });
        this.educationLevelSvc
            .educationLevelsByCurriculum(curriculumId)
            .subscribe({
                next: (educationLevels) => {
                    this.educationLevels = educationLevels.sort(
                        (a, b) => a.rank - b.rank
                    );
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    educationLevelYearChanged = (ely: EducationLevelYear) => {
        this.studentSubjects = [];
        this.stateSvc.set({
            academicYearId: ely.academicYearId,
            educationLevelId: ely.educationLevelId,
            schoolClassId: null,
            selectedStudentClassId: null
        });
        this.schoolClassSvc
            .getByEducationLevelandYear(
                ely.educationLevelId,
                ely.academicYearId
            )
            .subscribe({
                next: (schoolClasses) => {
                    this.schoolClasses = schoolClasses.sort(
                        (a, b) => a.rank - b.rank
                    );
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    schoolClassChanged = (schoolClassId: number) => {
        this.studentClasses = [];
        this.studentSubjects = [];
        this.subjectCounts = {};
        this.selectedStudentClassId = null;
        this.stateSvc.set({
            schoolClassId,
            selectedStudentClassId: null
        });
        this.studentClassesSvc
            .getBySchoolClassId(schoolClassId, Status.Active)
            .subscribe({
                next: (studentClasses) => {
                    this.studentClasses = studentClasses.sort((a, b) =>
                        a.student.upi.localeCompare(b.student.upi)
                    );
                    // Eager parallel fetch of per-student subject counts so each
                    // row can render a clickable count badge. Uses the existing
                    // /studentSubjects/byStudentClassId/{id} endpoint.
                    if (studentClasses.length) {
                        const reqs = studentClasses.map((sc) =>
                            this.studentSubjectsSvc.get(
                                '/studentSubjects/byStudentClassId/' + sc.id
                            )
                        );
                        forkJoin(reqs).subscribe({
                            next: (results) => {
                                const counts: Record<number, number> = {};
                                studentClasses.forEach((sc, idx) => {
                                    counts[parseInt(sc.id)] = (results[idx] || []).length;
                                });
                                this.subjectCounts = counts;
                            },
                            error: () => {
                                // Non-fatal: row badges just won't render.
                            }
                        });
                    }
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    // Loads the selected student's subject assignments into the right pane,
    // replacing the previous "search by selected student dropdown" flow.
    studentClassClicked = (studentClassId: number) => {
        this.selectedStudentClassId = studentClassId;
        this.stateSvc.set({selectedStudentClassId: studentClassId});
        this.studentSubjects = [];
        this.studentSubjectsSvc
            .get('/studentSubjects/byStudentClassId/' + studentClassId)
            .subscribe({
                next: (studentSubjects) => {
                    this.studentSubjects = studentSubjects;
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    dataSubmitted = (ssSearch: StudentSubjectSearch) => {
        this.studentSubjects = [];
        if (!ssSearch.academicYearId)
            this.toastr.info('Select academic year before searching!');
        else if (!ssSearch.curriculumId)
            this.toastr.info('Select curriculum before searching!');
        else if (!ssSearch.educationLevelId)
            this.toastr.info('Select education level before searching!');
        else if (!ssSearch.schoolClassId)
            this.toastr.info('Select class before searching!');
        else if (!ssSearch.studentClassId)
            this.toastr.info('Select student before searching!');
        else {
            this.studentSubjectsSvc
                .get(
                    '/studentSubjects/byStudentClassId/' +
                        ssSearch.studentClassId
                )
                .subscribe({
                    next: (studentSubjects) => {
                        studentSubjects.length <= 0
                            ? this.toastr.info(
                                  'No records found with the search parameters selected!'
                              )
                            : (this.studentSubjects = studentSubjects);
                    },
                    error: (err) => {
                        this.toastr.error(err.error);
                    }
                });
        }
    };

    deleteItem(id: number) {
        Swal.fire({
            title: `Delete student subject record?`,
            text: `Confirm if you want to delete student subject record.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `Delete`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.studentSubjectsSvc
                    .delete('/studentSubjects', id)
                    .subscribe(
                        (res) => {
                            this.toastr.success(
                                'Student subject record deleted successfully!'
                            );
                            this.studentSubjects.splice(
                                this.studentSubjects.findIndex(
                                    (e) => e.id == id.toString()
                                ),
                                1
                            );
                        },
                        (err) => {
                            this.toastr.error(err);
                        }
                    );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    // Restores the previous filter selection (if any) when the page is
    // re-entered after navigating away (e.g. to /students/students-subjects/add).
    // Waits for loadInitials() to populate curricula + academicYears, then
    // replays the dependent loads in order so the dropdowns and lists
    // repopulate just as the user left them - including the selected student's
    // subjects on the right-hand pane.
    private restoreStateIfAny(): void {
        const state = this.stateSvc.get();
        if (!state || !state.curriculumId) return;

        const waitForInitials = () => {
            if (!this.doneLoading) {
                setTimeout(waitForInitials, 50);
                return;
            }
            // Push saved values into the form dropdowns.
            this.searchForm?.setFormValues(state);

            // Cascade: educationLevels -> schoolClasses -> studentClasses (+ counts) -> subjects.
            this.educationLevelSvc
                .educationLevelsByCurriculum(state.curriculumId!)
                .subscribe({
                    next: (els) => {
                        this.educationLevels = els.sort((a, b) => a.rank - b.rank);
                        if (!state.educationLevelId || !state.academicYearId) return;

                        this.schoolClassSvc
                            .getByEducationLevelandYear(
                                state.educationLevelId,
                                state.academicYearId
                            )
                            .subscribe({
                                next: (scs) => {
                                    this.schoolClasses = scs.sort((a, b) => a.rank - b.rank);
                                    if (!state.schoolClassId) return;

                                    this.studentClassesSvc
                                        .getBySchoolClassId(state.schoolClassId, Status.Active)
                                        .subscribe({
                                            next: (stcs) => {
                                                this.studentClasses = stcs.sort((a, b) =>
                                                    a.student.upi.localeCompare(b.student.upi)
                                                );
                                                if (stcs.length) {
                                                    const reqs = stcs.map((sc) =>
                                                        this.studentSubjectsSvc.get(
                                                            '/studentSubjects/byStudentClassId/' + sc.id
                                                        )
                                                    );
                                                    forkJoin(reqs).subscribe({
                                                        next: (results) => {
                                                            const counts: Record<number, number> = {};
                                                            stcs.forEach((sc, idx) => {
                                                                counts[parseInt(sc.id)] = (results[idx] || []).length;
                                                            });
                                                            this.subjectCounts = counts;
                                                        }
                                                    });
                                                }
                                                if (state.selectedStudentClassId) {
                                                    this.studentClassClicked(state.selectedStudentClassId);
                                                }
                                            }
                                        });
                                }
                            });
                    }
                });
        };
        waitForInitials();
    }
}
