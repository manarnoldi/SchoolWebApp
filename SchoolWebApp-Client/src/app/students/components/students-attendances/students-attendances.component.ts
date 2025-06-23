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
import {StudentAttendanceSearch} from '@/students/models/student-attendance-search';
import {StudentClass} from '@/students/models/student-class';
import {StudentAttendancesService} from '@/students/services/student-attendances.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {DatePipe} from '@angular/common';
import {AfterViewInit, Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import {StudentsAttendancesTableComponent} from './students-attendances-table/students-attendances-table.component';
import Swal from 'sweetalert2';
import {StudentAttendance} from '@/students/models/student-attendance';
import {Status} from '@/core/enums/status';

@Component({
    selector: 'app-students-attendances',
    templateUrl: './students-attendances.component.html',
    styleUrl: './students-attendances.component.scss'
})
export class StudentsAttendancesComponent implements OnInit {
    @ViewChild(StudentsAttendancesTableComponent)
    studentsAttendancesTableComponent: StudentsAttendancesTableComponent;
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {
            link: ['/students/students-attendances'],
            title: 'Students: Attendances'
        }
    ];

    dashboardTitle = 'Students: Attendances';
    saSearch: StudentAttendanceSearch;
    doneLoading = false;
    curricula: Curriculum[] = [];
    educationLevels: EducationLevel[] = [];
    academicYears: AcademicYear[] = [];
    schoolClasses: SchoolClass[] = [];
    studentClasses: StudentClass[] = [];

    attendanceDate: Date;

    constructor(
        private toastr: ToastrService,
        private curriculaSvc: CurriculumService,
        private academicYearsSvc: AcademicYearsService,
        private educationLevelSvc: EducationLevelService,
        private schoolClassSvc: SchoolClassesService,
        private studentClassesSvc: StudentClassService,
        private studentAttendanceSvc: StudentAttendancesService,
        private datePipe: DatePipe
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
        this.studentClasses = [];
        this.educationLevels = [];
        this.schoolClasses = [];
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
        this.studentClasses = [];
        this.schoolClasses = [];
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
    };

    dateChanged = (selectedDate: Date) => {
        this.studentClasses = [];
        this.attendanceDate = selectedDate;
    };

    deleteAttendance(id: number) {
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
                const studentClass = this.studentClasses.find(
                    (i) => i.id == id.toString()
                );
                let attendDate = this.datePipe.transform(
                    this.attendanceDate,
                    'yyyy-MM-dd'
                );
                this.studentAttendanceSvc
                    .getObjectBySearch(
                        '/studentAttendances/byStudentClassIdAttendanceDate/' +
                            id +
                            '/' +
                            attendDate
                    )
                    .subscribe({
                        next: (att) => {
                            this.studentAttendanceSvc
                                .delete('/studentAttendances', parseInt(att.id))
                                .subscribe(
                                    (res) => {
                                        if (studentClass) {
                                            studentClass.hasRecord = false;
                                            studentClass.isSelected = false;
                                            studentClass.isOriginallySelected =
                                                false;
                                            studentClass.remarks = '';
                                        }
                                        this.toastr.success(
                                            'Record deleted successfully!'
                                        );
                                    },
                                    (err) => {
                                        this.toastr.error(err);
                                    }
                                );
                        },
                        error: (err) => {
                            this.toastr.error(err);
                        }
                    });
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    searchForDataMethod = (saSearch: StudentAttendanceSearch) => {
        this.studentClasses = [];
        this.saSearch = saSearch;
        if (!saSearch.academicYearId)
            this.toastr.info('Select academic year before searching!');
        else if (!saSearch.curriculumId)
            this.toastr.info('Select curriculum before searching!');
        else if (!saSearch.educationLevelId)
            this.toastr.info('Select education level before searching!');
        else if (!saSearch.schoolClassId)
            this.toastr.info('Select class before searching!');
        else {
            this.attendanceDate = saSearch.attendanceDate;
            this.studentClassesSvc
                .getBySchoolClassId(saSearch.schoolClassId, Status.Active)
                .subscribe({
                    next: (studentClasses) => {
                        if (studentClasses.length <= 0) {
                            this.toastr.info(
                                'No records found with the search parameters selected!'
                            );
                        } else {
                            let checkStudAttendancesReq = [];
                            studentClasses.forEach((sc) => {
                                let attendDate = this.datePipe.transform(
                                    this.attendanceDate,
                                    'yyyy-MM-dd'
                                );
                                checkStudAttendancesReq.push(
                                    this.studentAttendanceSvc.getObjectBySearch(
                                        '/studentAttendances/byStudentClassIdAttendanceDate/' +
                                            sc.id +
                                            '/' +
                                            attendDate
                                    )
                                );
                            });

                            forkJoin(checkStudAttendancesReq).subscribe({
                                next: (saRes: StudentAttendance[]) => {
                                    for (let i = 0; i < saRes.length; i++) {
                                        if (saRes[i].id) {
                                            let [hourIn, minuteIn] = saRes[i]
                                                .timeIn
                                                ? saRes[i].timeIn
                                                      .split(':')
                                                      .map(Number)
                                                : [8, 0];
                                            let [hourOut, minuteOut] = saRes[i]
                                                .timeIn
                                                ? saRes[i].timeIn
                                                      .split(':')
                                                      .map(Number)
                                                : [17, 0];

                                            studentClasses[i].isSelected =
                                                saRes[i].present;
                                            studentClasses[i].remarks =
                                                saRes[i].remarks;
                                            studentClasses[i].hasRecord = true;
                                            studentClasses[i].timeIn = {
                                                hour: hourIn,
                                                minute: minuteIn
                                            };
                                            studentClasses[i].timeOut = {
                                                hour: hourOut,
                                                minute: minuteOut
                                            };
                                        }
                                    }
                                    this.studentClasses = studentClasses;
                                    this.studentsAttendancesTableComponent.studentClasses =
                                        studentClasses;
                                    this.studentsAttendancesTableComponent.updateCheckAll();
                                },
                                error: (err) => {
                                    this.toastr.error(err.error);
                                }
                            });
                        }
                    },
                    error: (err) => {
                        this.toastr.error(err.error);
                    }
                });
        }
    };

    submitAttendances = () => {
        if (this.studentClasses.length <= 0)
            this.toastr.error('There are no records to save!');

        Swal.fire({
            title: `Update Students attendance record?`,
            text: `Confirm if you want to batch update students attendance.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `Update Attendances`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let studentAttendances: StudentAttendance[] = [];

                this.studentClasses.forEach((st) => {
                    let sa = new StudentAttendance();
                    let dateIn = new Date(
                        1970,
                        0,
                        1,
                        st.timeIn.hour,
                        st.timeIn.minute
                    );
                    let dateOut = new Date(
                        1970,
                        0,
                        1,
                        st.timeIn.hour,
                        st.timeIn.minute
                    );

                    sa.date = this.attendanceDate;
                    sa.present = st.isSelected;
                    sa.remarks = st.remarks;
                    sa.timeIn = this.datePipe.transform(dateIn, 'HH:mm:ss');
                    sa.timeOut = this.datePipe.transform(dateOut, 'HH:mm:ss');
                    sa.studentClassId = parseInt(st.id);

                    studentAttendances.push(sa);
                });

                this.studentAttendanceSvc
                    .createBatch(
                        '/studentAttendances/batch',
                        studentAttendances
                    )
                    .subscribe({
                        next: (res) => {
                            this.toastr.success(
                                'Students attendances updated successfully.'
                            );
                            this.searchForDataMethod(this.saSearch);
                        },
                        error: (err) => {
                            this.toastr.error(err.error?.message);
                        }
                    });
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };
}
