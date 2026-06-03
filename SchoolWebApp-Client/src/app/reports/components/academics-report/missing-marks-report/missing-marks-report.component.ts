import {Curriculum} from '@/academics/models/curriculum';
import {Exam} from '@/academics/models/exam';
import {ExamResult} from '@/academics/models/exam-result';
import {ExamType} from '@/academics/models/exam-type';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {ExamResultsService} from '@/academics/services/exam-results.service';
import {ExamTypesService} from '@/academics/services/exam-types.service';
import {ExamsService} from '@/academics/services/exams.service';
import {Session} from '@/class/models/session';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolExamService} from '@/cbe/exams/services/school-exam.service';
import {MissingMarksReportService} from '@/reports/services/academicsReports/missing-marks-report.service';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-missing-marks-report',
    templateUrl: './missing-marks-report.component.html',
    styleUrl: './missing-marks-report.component.scss'
})
export class MissingMarksReportComponent implements OnInit {
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    missingMarksResults: ExamResult[] = [];
    curricula: Curriculum[] = [];
    academicYears: AcademicYear[] = [];
    sessions: Session[] = [];
    examTypes: ExamType[] = [];
    schoolExams: any[] = [];

    constructor(
        private toastr: ToastrService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private curriculaSvc: CurriculumService,
        private examsSvc: ExamsService,
        private examTypesSvc: ExamTypesService,
        private examResultsSvc: ExamResultsService,
        private schoolSvc: SchoolDetailsService,
        private schoolExamSvc: SchoolExamService,
        private missingMarksRptSvc: MissingMarksReportService
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems() {
        let acadYearsReq = this.academicYearSvc.get('/academicYears');
        let curriculaReq = this.curriculaSvc.get('/curricula');
        let examTypesReq = this.examTypesSvc.get('/examTypes');
        forkJoin([acadYearsReq, curriculaReq, examTypesReq]).subscribe({
            next: ([academicYears, curricula, examTypes]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort(
                    (a, b) => b.rank - a.rank
                );
                this.examTypes = examTypes.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    }

    academicYearChanged = (acadYearId: number) => {
        this.missingMarksResults = [];
        if (acadYearId) {
            this.curriculumYearChanged();
        }
    };

    curriculumChanged = (curriculumId: number) => {
        this.missingMarksResults = [];
        if (curriculumId) {
            this.curriculumYearChanged();
        }
    };

    curriculumYearChanged = () => {
        this.sessions = [];
        this.missingMarksResults = [];
        this.resetFormControls(true, true);

        let curriculumId =
            this.ssFilterFormComponent.schoolSoftFilterForm.get(
                'curriculumId'
            ).value;
        let acadYearId =
            this.ssFilterFormComponent.schoolSoftFilterForm.get(
                'academicYearId'
            ).value;

        if (curriculumId && acadYearId) {
            this.sessionsSvc
                .getByCurriculumYear(curriculumId, acadYearId)
                .subscribe({
                    next: (sessions) => {
                        this.sessions = sessions.sort(
                            (a, b) => a.rank - b.rank
                        );
                    },
                    error: (err) => {
                        this.toastr.error(err.error);
                    }
                });
        }
    };

    sessionChanged = (sessionId: number) => {
        this.resetFormControls(false, true);
        this.missingMarksResults = [];
        this.schoolExams = [];
        if (!sessionId) return;

        let curriculumId =
            this.ssFilterFormComponent.schoolSoftFilterForm.get('curriculumId').value;
        let acadYearId =
            this.ssFilterFormComponent.schoolSoftFilterForm.get('academicYearId').value;
        if (!curriculumId || !acadYearId) return;

        // Load the school exams for this term; the user picks one and we derive
        // its exam type for the search.
        this.schoolExamSvc
            .get(`/schoolExams/examSearch?academicYearId=${acadYearId}&curriculumId=${curriculumId}&sessionId=${sessionId}`)
            .subscribe({
                next: (items) => { this.schoolExams = items; },
                error: (err) => this.toastr.error(err.error)
            });
    };

    schoolExamChanged = (schoolExamId: number) => {
        this.missingMarksResults = [];
    };

    onButtonSearchClick = (ssf: SchoolSoftFilter) => {
        this.missingMarksResults = [];
        if (!ssf?.academicYearId)
            this.toastr.info('Select academic year before searching!');
        else if (!ssf.curriculumId)
            this.toastr.info('Select curriculum before searching!');
        else if (!ssf.sessionId)
            this.toastr.info('Select session before searching!');
        else if (!ssf.schoolExamId)
            this.toastr.info('Select school exam before searching!');
        else {
            // The school exam carries the exam type the API still filters by.
            let schoolExam = this.schoolExams.find((se) => se.id == ssf.schoolExamId);
            let examTypeId = schoolExam?.examTypeId ?? schoolExam?.examType?.id;
            // Single server-side call instead of one request per exam. The
            // API returns every allocated student with no result for the
            // selection, already scoped by year/curriculum/session/type.
            this.examResultsSvc
                .getMissingMarks(
                    ssf.academicYearId,
                    ssf.curriculumId,
                    ssf.sessionId,
                    examTypeId
                )
                .subscribe({
                    next: (examResults) => {
                        if (!examResults || examResults.length <= 0) {
                            this.toastr.info(
                                'No missing marks found for the selections (or no exams are registered).'
                            );
                            return;
                        }
                        this.missingMarksResults.push(...examResults);
                        this.missingMarksResults.sort(
                            (a, b) =>
                                a.exam?.schoolClass.rank -
                                    b.exam?.schoolClass.rank ||
                                a.exam?.subject.rank - b.exam?.subject.rank ||
                                a.student.upi.localeCompare(b.student.upi)
                        );
                    },
                    error: (err) => {
                        this.toastr.error(err.error);
                    }
                });
        }
    };

    printReport = () => {
        this.schoolSvc.get('/schooldetails').subscribe({
            next: (school) => {
                const examTypeId =
                    this.ssFilterFormComponent.schoolSoftFilterForm.get(
                        'examTypeId'
                    ).value;
                const sessionId =
                    this.ssFilterFormComponent.schoolSoftFilterForm.get(
                        'sessionId'
                    ).value;
                const academicYearId =
                    this.ssFilterFormComponent.schoolSoftFilterForm.get(
                        'academicYearId'
                    ).value;

                let reportTitle =
                    'MISSING MARKS REPORT FOR ' +
                    this.examTypes
                        .find((et) => et.id == examTypeId)
                        ?.name?.toUpperCase() +
                    ' - ' +
                    this.sessions
                        .find((et) => et.id == sessionId)
                        ?.sessionName?.toUpperCase() +
                    ' ' +
                    this.academicYears
                        .find((et) => et.id == academicYearId)
                        ?.name?.toUpperCase();
                this.missingMarksRptSvc.generateReport(
                    school[0],
                    this.missingMarksResults,
                    reportTitle
                );
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    resetFormControls = (
        sessionIdReset: boolean,
        examTypeIdReset: boolean
    ) => {
        if (sessionIdReset)
            this.ssFilterFormComponent.schoolSoftFilterForm
                .get('sessionId')
                ?.reset();
        if (examTypeIdReset) {
            this.ssFilterFormComponent.schoolSoftFilterForm
                .get('examTypeId')
                ?.reset();
            this.ssFilterFormComponent.schoolSoftFilterForm
                .get('schoolExamId')
                ?.reset();
        }
    };
}
