import {ClassLeadership} from '@/class/models/class-leadership';
import {SchoolClass} from '@/class/models/school-class';
import {ClassLeadershipsService} from '@/class/services/class-leaderships.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {Status} from '@/core/enums/status';
import {ClassListReportService} from '@/reports/services/class-reports/class-list-report.service';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {StudentClass} from '@/students/models/student-class';
import {StudentClassService} from '@/students/services/student-class.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-class-lists-report',
    templateUrl: './class-lists-report.component.html',
    styleUrl: './class-lists-report.component.scss'
})
export class ClassListsReportComponent implements OnInit {
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    academicYears: AcademicYear[] = [];
    schoolClasses: SchoolClass[] = [];
    studentClasses: StudentClass[] = [];

    schoolClassLeaders: ClassLeadership[] = [];

    currentStatus: Status = Status.Active;

    constructor(
        private toastr: ToastrService,
        private acadYearsSvc: AcademicYearsService,
        private schoolClassesSvc: SchoolClassesService,
        private studentClassesSvc: StudentClassService,
        private schoolSvc: SchoolDetailsService,
        private classListRptSvc: ClassListReportService,
        private classLeadershipsSvc: ClassLeadershipsService
    ) {}

    ngOnInit(): void {
        this.loadInitials();
    }

    get selectedCount(): number {
        return this.schoolClasses.filter((i) => i.isSelected).length;
    }

    schoolClassChanged = (classId: number) => {
        this.schoolClasses = [];
        this.studentClasses = [];
        this.schoolClassLeaders = [];
    };

    schoolClassClicked = (schoolClassId: number) => {
        this.studentClasses = [];
        this.schoolClassLeaders = [];

        let scLeadersReq =
            this.classLeadershipsSvc.getBySchoolClassId(schoolClassId);
        let scBySchoolClassIdReq = this.studentClassesSvc.getBySchoolClassId(
            schoolClassId,
            this.currentStatus
        );

        forkJoin([scLeadersReq, scBySchoolClassIdReq]).subscribe({
            next: ([scLeaders, studentClasses]) => {
                this.schoolClassLeaders = scLeaders;
                this.studentClasses = studentClasses.sort((a, b) =>
                    a.student?.upi.localeCompare(b.student?.upi)
                );
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    statusChanged = (status: Status) => {
        this.currentStatus = status;
        this.schoolClasses = [];
        this.studentClasses = [];
        this.schoolClassLeaders = [];
    };

    academicYearChanged = (academicYearId: number) => {
        this.schoolClasses = [];
        this.studentClasses = [];
        this.schoolClassLeaders = [];
    };

    searchForDataMethod = (ssFilter: SchoolSoftFilter) => {
        if (!ssFilter.academicYearId || ssFilter.academicYearId == null) {
            this.toastr.error('Select academic year before clicking search!');
            return;
        } else if (!ssFilter.status == null) {
            this.toastr.error('Select status before clicking search!');
            return;
        }
        this.schoolClasses = [];
        this.studentClasses = [];
        this.schoolClassLeaders = [];
        this.schoolClassesSvc
            .getByAcademicYearId(ssFilter.academicYearId)
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

    printReport = () => {
        this.schoolSvc.get('/schooldetails').subscribe({
            next: (school) => {
                const status =
                    this.ssFilterFormComponent.schoolSoftFilterForm.get(
                        'status'
                    ).value;

                const reportTitle = 'SCHOOL CLASS LIST REPORT';
                const classListsRequests = this.schoolClasses
                    .filter((s) => s.isSelected)
                    .map((s) =>
                        this.studentClassesSvc.getBySchoolClassId(
                            parseInt(s.id),
                            status
                        )
                    );

                forkJoin(classListsRequests).subscribe({
                    next: (studentClass) => {
                        this.classListRptSvc.printByBatch(
                            studentClass.filter((s) => s.length > 0),
                            school[0],
                            reportTitle
                        );
                    },
                    error: (err) => {
                        this.toastr.error(err.error || err);
                    }
                });
            },
            error: (err) => {
                this.toastr.error(err.error || err);
            }
        });
    };

    loadInitials = () => {
        this.acadYearsSvc.get('/academicYears').subscribe({
            next: (academicYears) => {
                this.academicYears = academicYears.sort(
                    (a, b) => b.rank - a.rank
                );
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };
}
