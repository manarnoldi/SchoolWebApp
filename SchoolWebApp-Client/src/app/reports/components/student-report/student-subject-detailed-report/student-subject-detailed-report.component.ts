import {Curriculum} from '@/academics/models/curriculum';
import {Subject} from '@/academics/models/subject';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {Status} from '@/core/enums/status';
import {StudentSubjectDetailedReportService} from '@/reports/services/student-reports/student-subject-detailed-report.service';
import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {StudentSubject} from '@/students/models/student-subject';
import {StudentClassService} from '@/students/services/student-class.service';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

// One row per student in the picked class showing that student's allocated
// subjects on a single sheet, plus a printable class report. Data is pulled in
// two calls (active students + all class allocations) and grouped client-side,
// replacing the old per-student drill-down + per-student request fan-out.
interface ClassAllocationRow {
    studentClassId: number;
    upi: string;
    fullName: string;
    subjects: Subject[];
}

@Component({
    selector: 'app-student-subject-detailed-report',
    templateUrl: './student-subject-detailed-report.component.html',
    styleUrl: './student-subject-detailed-report.component.scss'
})
export class StudentSubjectDetailedReportComponent implements OnInit {
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    curricula: Curriculum[] = [];
    educationLevels: EducationLevel[] = [];
    academicYears: AcademicYear[] = [];
    schoolClasses: SchoolClass[] = [];

    classRows: ClassAllocationRow[] = [];
    isLoading = false;

    // schoolClasses load only after BOTH educationLevel + academicYear are set
    // (the school-soft-filter emits each change separately).
    private currentEducationLevelId: number | null = null;
    private currentAcademicYearId: number | null = null;
    private selectedSchoolClass: SchoolClass | null = null;

    constructor(
        private toastr: ToastrService,
        private curriculaSvc: CurriculumService,
        private acadYearsSvc: AcademicYearsService,
        private educationLevelSvc: EducationLevelService,
        private schoolClassSvc: SchoolClassesService,
        private studentClassesSvc: StudentClassService,
        private studentSubjectsSvc: StudentSubjectsService,
        private studentSubjectDetailsRptSvc: StudentSubjectDetailedReportService,
        private schoolSvc: SchoolDetailsService
    ) {}

    ngOnInit(): void {
        this.loadInitials();
    }

    loadInitials = () => {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.acadYearsSvc.get('/academicYears')
        ]).subscribe({
            next: ([curricula, academicYears]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort((a, b) => b.rank - a.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    curriculumChanged = (curriculumId: number) => {
        this.resetDownstream();
        this.educationLevelSvc
            .educationLevelsByCurriculum(curriculumId)
            .subscribe({
                next: (els) => {
                    this.educationLevels = els.sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
    };

    academicYearChanged = (yearId: number) => {
        this.currentAcademicYearId = yearId;
        this.resetDownstream();
        this.maybeLoadSchoolClasses();
    };

    educationLevelChanged = (educationLevelId: number) => {
        this.currentEducationLevelId = educationLevelId;
        this.resetDownstream();
        this.maybeLoadSchoolClasses();
    };

    private maybeLoadSchoolClasses() {
        if (!this.currentEducationLevelId || !this.currentAcademicYearId) return;
        this.schoolClassSvc
            .getByEducationLevelandYear(
                this.currentEducationLevelId,
                this.currentAcademicYearId
            )
            .subscribe({
                next: (scs) => {
                    this.schoolClasses = scs.sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
    }

    schoolClassChanged = (schoolClassId: number) => {
        this.classRows = [];
        this.selectedSchoolClass =
            this.schoolClasses.find((sc) => +sc.id === +schoolClassId) ?? null;
        if (!schoolClassId) return;

        this.isLoading = true;
        // Two calls only: the class's active students, and every allocation in
        // the class. Group the allocations by student and attach to each row.
        forkJoin([
            this.studentClassesSvc.getBySchoolClassId(schoolClassId, Status.Active),
            this.studentSubjectsSvc.get(
                '/studentSubjects/allocationsBySchoolClassId/' + schoolClassId
            )
        ]).subscribe({
            next: ([studentClasses, allocations]) => {
                const byStudentClass = new Map<number, Subject[]>();
                (allocations as StudentSubject[]).forEach((a) => {
                    if (a.studentClassId == null || !a.subject) return;
                    const list = byStudentClass.get(a.studentClassId) ?? [];
                    list.push(a.subject);
                    byStudentClass.set(a.studentClassId, list);
                });

                this.classRows = studentClasses
                    .sort((a, b) => a.student.upi.localeCompare(b.student.upi))
                    .map((sc) => ({
                        studentClassId: parseInt(sc.id),
                        upi: sc.student?.upi,
                        fullName: sc.student?.fullName,
                        subjects: (byStudentClass.get(parseInt(sc.id)) ?? []).sort(
                            (x, y) => (x.rank ?? 0) - (y.rank ?? 0)
                        )
                    }));
                this.isLoading = false;
            },
            error: (err) => {
                this.isLoading = false;
                this.toastr.error(err.error);
            }
        });
    };

    private resetDownstream() {
        this.classRows = [];
        this.selectedSchoolClass = null;
    }

    // school-soft-filter-form Search button no-op; cascade events populate the
    // list as the user picks filters.
    searchForDataMethod = (_filter: any) => {};

    printReport = () => {
        if (this.classRows.length === 0) {
            this.toastr.info('Select a class with students first.');
            return;
        }
        this.schoolSvc.get('/schooldetails').subscribe({
            next: (school) => {
                const year = this.academicYears.find(
                    (i) => i.id == this.currentAcademicYearId?.toString()
                );
                const reportTitle =
                    'STUDENT SUBJECT ALLOCATION - ' +
                    (this.selectedSchoolClass?.name?.toUpperCase() ?? '') +
                    (year?.name ? ' (' + year.name + ')' : '');
                this.studentSubjectDetailsRptSvc.printClassReport(
                    school[0],
                    reportTitle,
                    this.classRows
                );
            },
            error: (err) => this.toastr.error(err.error || err)
        });
    };
}
