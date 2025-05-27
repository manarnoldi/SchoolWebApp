import {Curriculum} from '@/academics/models/curriculum';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolClassesService} from '@/class/services/school-classes.service';
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
import {Component, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-students-subjects',
    templateUrl: './students-subjects.component.html',
    styleUrl: './students-subjects.component.scss'
})
export class StudentsSubjectsComponent implements OnInit {
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
        private studentSubjectsSvc: StudentSubjectsService
    ) {}

    ngOnInit(): void {
        this.loadInitials();
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
        this.studentClassesSvc
            .get('/studentClasses/bySchoolClassId/' + schoolClassId)
            .subscribe({
                next: (studentClasses) => {
                    this.studentClasses = studentClasses.sort((a, b) =>
                        a.student.upi.localeCompare(b.student.upi)
                    );
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
}
