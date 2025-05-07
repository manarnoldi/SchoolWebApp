import {Curriculum} from '@/academics/models/curriculum';
import {Subject} from '@/academics/models/subject';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {EducationLevelSubjectService} from '@/academics/services/education-level-subject.service';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {StudentClass} from '@/students/models/student-class';
import {StudentDetails} from '@/students/models/student-details';
import {StudentSubject} from '@/students/models/student-subject';
import {StudentClassService} from '@/students/services/student-class.service';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-students-subjects-add-form',
    templateUrl: './students-subjects-add-form.component.html',
    styleUrl: './students-subjects-add-form.component.scss'
})
export class StudentsSubjectsAddFormComponent implements OnInit {
    studentSubjectId: number;
    eduLevelId: number;

    academicYears: AcademicYear[] = [];
    curricula: Curriculum[] = [];
    subjects: Subject[] = [];
    educationLevels: EducationLevel[] = [];
    schoolClasses: SchoolClass[] = [];
    studentClasses: StudentClass[] = [];

    isLoading: boolean = true;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/students/students-subjects'], title: 'Students Subject List'},
        {
            link: ['/students/students-subjects/add'],
            title: 'Add Students Subjects'
        }
    ];
    dashboardTitle = 'Academics: Add Students Subjects';

    studentSubjectsAddForm: FormGroup;
    editMode: boolean = false;

    constructor(
        private formBuilder: FormBuilder,
        private curriculaSvc: CurriculumService,
        private router: Router,
        private route: ActivatedRoute,
        private academicYearsSvc: AcademicYearsService,
        private toastr: ToastrService,
        private educationLevelSubjectSvc: EducationLevelSubjectService,
        private educationLevelSvc: EducationLevelService,
        private schoolClassesSvc: SchoolClassesService,
        private studentClassSvc: StudentClassService,
        private studentSubjectsSvc: StudentSubjectsService
    ) {
        // private examTypesSvc: ExamTypesService,
        // private sessionSvc: SessionsService,
        // private examsSvc: ExamsService,
        // private subjectsSvc: SubjectsService,
        // private datePipe: DatePipe
    }

    ngOnInit(): void {
        this.initializeForm();
        this.refreshItems();
    }
    refreshItems() {
        this.route.queryParams.subscribe((params) => {
            this.studentSubjectId = parseInt(params['id']);
            this.eduLevelId = parseInt(params['eduLevelId']);

            let curriculaReq = this.curriculaSvc.get('/curricula');
            let academicYearsReq = this.academicYearsSvc.get('/academicYears');

            forkJoin([curriculaReq, academicYearsReq]).subscribe(
                ([curricula, academicYears]) => {
                    this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                    this.academicYears = academicYears.sort(
                        (a, b) => b.rank - a.rank
                    );

                    if (this.studentSubjectId && this.eduLevelId > 0) {
                    }
                    this.isLoading = true;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    }

    schoolClassChanged = () => {
        this.studentClasses = [];
        let schoolClassId =
            this.studentSubjectsAddForm.get('schoolClassId').value;
        if (!schoolClassId || schoolClassId == null) {
            return;
        }

        this.studentClassSvc
            .get('/studentClasses/bySchoolClassId/' + schoolClassId)
            .subscribe({
                next: (studentClasses) => {
                    this.studentClasses = studentClasses;
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    curriculumChanged = () => {
        this.schoolClasses = [];
        this.educationLevels = [];
        this.studentClasses = [];
        this.subjects = [];

        this.studentSubjectsAddForm.get('educationLevelId').reset();
        this.studentSubjectsAddForm.get('academicYearId').reset();

        let curriculumId =
            this.studentSubjectsAddForm.get('curriculumId').value;

        if (!curriculumId || curriculumId == null) {
            return;
        }

        this.educationLevelSvc
            .get('/educationLevels/byCurriculumId/' + curriculumId)
            .subscribe({
                next: (eduLevels) => {
                    this.educationLevels = eduLevels.sort(
                        (a, b) => a.rank - b.rank
                    );
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    educationLevelYearChanged = () => {
        this.subjects = [];
        this.schoolClasses = [];

        this.studentSubjectsAddForm.get('schoolClassId').reset();

        if (
            !this.studentSubjectsAddForm.get('educationLevelId').value ||
            !this.studentSubjectsAddForm.get('academicYearId').value
        ) {
            return;
        }
        let eduLevelId =
            this.studentSubjectsAddForm.get('educationLevelId').value;
        let acadYearId =
            this.studentSubjectsAddForm.get('academicYearId').value;

        let educationLevelSubjectsReq = this.educationLevelSubjectSvc.get(
            '/educationLevelSubjects/byEducationLevelYearId/' +
                eduLevelId +
                '/' +
                acadYearId
        );
        let schoolClassReq = this.schoolClassesSvc.get(
            '/schoolClasses/byEducationLevelYearId/' +
                eduLevelId +
                '/' +
                acadYearId
        );

        forkJoin([educationLevelSubjectsReq, schoolClassReq]).subscribe({
            next: ([educationLevelSubjects, schoolClasses]) => {
                educationLevelSubjects.forEach((els) => {
                    this.subjects.push(els.subject);
                });
                this.subjects.sort((a, b) => a.rank - b.rank);
                this.schoolClasses = schoolClasses.sort(
                    (a, b) => a.rank - b.rank
                );
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    initializeForm = () => {
        this.studentSubjectsAddForm = this.formBuilder.group({
            academicYearId: [null, Validators.required],
            schoolClassId: [null, Validators.required],
            curriculumId: [null, Validators.required],
            educationLevelId: [null, Validators.required]
        });
    };

    onSubmit = () => {
        if (this.studentClasses.filter((item) => item.isSelected).length <= 0) {
            this.toastr.error(
                'You have not selected the students to be assigned the subjects!'
            );
        } else if (
            this.subjects.filter((item) => item.isSelected).length <= 0
        ) {
            this.toastr.error(
                'You have not selected the subjects to be assigned to the students!'
            );
        }
        Swal.fire({
            title: `Submit students-subjects allocation?`,
            text: `Confirm if you want to submit students-subjects allocation.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `Submit allocation`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let studentSubjects: StudentSubject[] = [];

                this.studentClasses.forEach((st) => {
                    if (st.isSelected) {
                        this.subjects.forEach((sub) => {
                            if (sub.isSelected) {
                                let ss = new StudentSubject();
                                ss.studentClassId = parseInt(st.id);
                                ss.subjectId = parseInt(sub.id);
                                studentSubjects.push(ss);
                            }
                        });
                    }
                });

                this.studentSubjectsSvc
                    .createBatch('/studentSubjects/batch', studentSubjects)
                    .subscribe({
                        next: (res) => {
                            this.toastr.success(
                                'Students subjects allocations saved successfully.'
                            );
                            this.subjects = [];
                            this.studentClasses = [];
                        },
                        error: (err) => {
                            this.toastr.error(err.error?.message);
                        }
                    });
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };

    get f() {
        return this.studentSubjectsAddForm.controls;
    }
}
