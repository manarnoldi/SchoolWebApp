import {Curriculum} from '@/academics/models/curriculum';
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
import {StudentClass} from '@/students/models/student-class';
import {StudentSubject} from '@/students/models/student-subject';
import {StudentClassService} from '@/students/services/student-class.service';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

// Student Subject Allocation Report - cascade-filter version (Curriculum ->
// Year -> Level -> Class). Once a class is picked, the left pane lists every
// student in the class with a clickable count badge. Click a row to load that
// student's subjects on the right; tick checkboxes + Print to generate PDFs.
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
    studentClasses: StudentClass[] = [];

    studentSubjects: StudentSubject[];

    // studentClassId -> subject count, pre-fetched after the SchoolClass filter
    // resolves so the left student list renders a clickable count badge per row.
    subjectCounts: Record<number, number> = {};
    selectedStudentClassId: number | null = null;

    // Tracked so the parent can fire the schoolClasses load only after BOTH
    // educationLevel + academicYear are set (the underlying school-soft-filter
    // emits each change separately, not as a combined event).
    private currentEducationLevelId: number | null = null;
    private currentAcademicYearId: number | null = null;
    // The /studentSubjects/byStudentClassId endpoint returns rows without the
    // studentClass.schoolClass nav property, so the on-screen table and the
    // PDF print would show blank class names. We cache the schoolClass the
    // user picked and back-fill it onto every returned StudentSubject.
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
        const curriculaReq = this.curriculaSvc.get('/curricula');
        const acadYearReq = this.acadYearsSvc.get('/academicYears');

        forkJoin([curriculaReq, acadYearReq]).subscribe({
            next: ([curricula, academicYears]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort((a, b) => b.rank - a.rank);
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
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
        this.studentClasses = [];
        this.studentSubjects = [];
        this.subjectCounts = {};
        this.selectedStudentClassId = null;
        // Cache the picked schoolClass for back-filling onto returned subjects.
        this.selectedSchoolClass = this.schoolClasses.find(
            (sc) => +sc.id === +schoolClassId
        ) ?? null;
        this.studentClassesSvc
            .getBySchoolClassId(schoolClassId, Status.Active)
            .subscribe({
                next: (stcs) => {
                    this.studentClasses = stcs.sort((a, b) =>
                        a.student.upi.localeCompare(b.student.upi)
                    );
                    if (stcs.length) {
                        // Eager parallel fetch of per-student subject counts.
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
                },
                error: (err) => this.toastr.error(err.error)
            });
    };

    studentClassClicked = (studentClassId: number) => {
        this.selectedStudentClassId = studentClassId;
        this.studentSubjects = [];
        this.studentSubjectsSvc
            .get('/studentSubjects/byStudentClassId/' + studentClassId)
            .subscribe({
                next: (subjects) => {
                    this.studentSubjects = this.attachSchoolClass(subjects);
                },
                error: (err) => this.toastr.error(err.error)
            });
    };

    private resetDownstream() {
        this.studentClasses = [];
        this.studentSubjects = [];
        this.subjectCounts = {};
        this.selectedStudentClassId = null;
        this.selectedSchoolClass = null;
    }

    // Backfills the studentClass.schoolClass nav property on every returned
    // StudentSubject using the schoolClass the user picked in the cascade.
    // Without this the on-screen and PDF "Class name" cells render blank.
    private attachSchoolClass(subjects: StudentSubject[]): StudentSubject[] {
        if (!this.selectedSchoolClass) return subjects;
        for (const ss of subjects) {
            if (!ss.studentClass) {
                ss.studentClass = new StudentClass({schoolClass: this.selectedSchoolClass});
            } else if (!ss.studentClass.schoolClass) {
                ss.studentClass.schoolClass = this.selectedSchoolClass;
            }
        }
        return subjects;
    }

    get selectedCount(): number {
        return this.studentClasses.filter((sc) => sc.isSelected).length;
    }

    // school-soft-filter-form Search button no-op; cascade events populate
    // the list as the user picks filters.
    searchForDataMethod = (_filter: any) => {};

    printReport = () => {
        this.schoolSvc.get('/schooldetails').subscribe({
            next: (school) => {
                const year = this.academicYears.find(
                    (i) => i.id == this.currentAcademicYearId?.toString()
                );
                const reportTitle =
                    'STUDENT SUBJECT ALLOCATION REPORT FOR ' +
                    (year?.name?.toUpperCase() ?? '');
                const requests = this.studentClasses
                    .filter((sc) => sc.isSelected)
                    .map((sc) =>
                        this.studentSubjectsSvc.get(
                            '/studentSubjects/byStudentClassId/' + sc.id
                        )
                    );
                if (requests.length === 0) return;
                forkJoin(requests).subscribe({
                    next: (subjects) => {
                        const enriched = subjects.map((arr) =>
                            this.attachSchoolClass(arr)
                        );
                        this.studentSubjectDetailsRptSvc.printByBatch(
                            enriched.filter((s) => s.length > 0),
                            school[0],
                            reportTitle
                        );
                    },
                    error: (err) => this.toastr.error(err.error || err)
                });
            },
            error: (err) => this.toastr.error(err.error || err)
        });
    };
}
