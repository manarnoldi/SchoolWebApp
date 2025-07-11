import {Curriculum} from '@/academics/models/curriculum';
import {Exam} from '@/academics/models/exam';
import {ExamName} from '@/academics/models/exam-name';
import {ExamResult} from '@/academics/models/exam-result';
import {ExamType} from '@/academics/models/exam-type';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {ExamNamesService} from '@/academics/services/exam-names.service';
import {ExamResultsService} from '@/academics/services/exam-results.service';
import {ExamTypesService} from '@/academics/services/exam-types.service';
import {ExamsService} from '@/academics/services/exams.service';
import {Session} from '@/class/models/session';
import {SessionsService} from '@/class/services/sessions.service';
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
    examNames: ExamName[] = [];

    constructor(
        private toastr: ToastrService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private curriculaSvc: CurriculumService,
        private examsSvc: ExamsService,
        private examTypesSvc: ExamTypesService,
        private examNamesSvc: ExamNamesService,
        private examResultsSvc: ExamResultsService,
        private schoolSvc: SchoolDetailsService,
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
        this.resetFormControls(true, true, true);

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
        this.resetFormControls(false, true, true);
        this.missingMarksResults = [];
    };

    examTypeChanged = (examTypeId: number) => {
        this.missingMarksResults = [];
        this.resetFormControls(false, false, true);

        this.examNamesSvc.getByExamTypeId(examTypeId).subscribe({
            next: (examNames) => {
                this.examNames = examNames;
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    examNameChanged = (examNameId: number) => {
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
        else if (!ssf.examTypeId)
            this.toastr.info('Select exam type before searching!');
        else if (!ssf.examNameId)
            this.toastr.info('Select exam name before searching!');
        else {
            this.examsSvc.getExamsBySearch(ssf).subscribe({
                next: (exams) => {
                    if (!exams || exams.length <= 0) {
                        this.toastr.error(
                            'There are no exams registered under the selections!'
                        );
                        return;
                    }

                    let missingMarksReq = [];
                    exams.forEach((ex) => {
                        missingMarksReq.push(
                            this.examResultsSvc.loadExamResults(ex, true)
                        );
                    });

                    forkJoin(missingMarksReq).subscribe({
                        next: (examResults) => {
                            this.missingMarksResults.push(
                                ...examResults.flat()
                            );
                            this.missingMarksResults.sort(
                                (a, b) =>
                                    a.exam?.schoolClass.rank -
                                        b.exam?.schoolClass.rank ||
                                    a.exam?.subject.rank -
                                        b.exam?.subject.rank ||
                                    a.student.upi.localeCompare(b.student.upi)
                            );
                        },
                        error: (err) => {
                            this.toastr.error(err.error);
                        }
                    });
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
                const examNameId =
                    this.ssFilterFormComponent.schoolSoftFilterForm.get(
                        'examNameId'
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
                        .name.toUpperCase() +
                    ': ' +
                    this.examNames
                        .find((et) => et.id == examNameId)
                        .name.toUpperCase() +
                    ' ' +
                    this.sessions
                        .find((et) => et.id == sessionId)
                        .sessionName.toUpperCase() +
                    ' ' +
                    this.academicYears
                        .find((et) => et.id == academicYearId)
                        .name.toUpperCase();
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
        examTypeIdReset: boolean,
        examNameIdReset: boolean
    ) => {
        if (sessionIdReset)
            this.ssFilterFormComponent.schoolSoftFilterForm
                .get('sessionId')
                .reset();
        if (examTypeIdReset)
            this.ssFilterFormComponent.schoolSoftFilterForm
                .get('examTypeId')
                .reset();
        if (examNameIdReset)
            this.ssFilterFormComponent.schoolSoftFilterForm
                .get('examNameId')
                .reset();
    };
}
