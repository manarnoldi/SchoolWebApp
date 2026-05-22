import {Component, OnDestroy, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of, Subject} from 'rxjs';
import {catchError, takeUntil} from 'rxjs/operators';
import {LoadingStateService} from '@/core/services/loading-state.service';
import {AcademicYearsService} from '../../services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {ExamTypeService} from '@/cbe/exams/services/exam-type.service';
import {ExamService} from '@/cbe/exams/services/exam.service';
import {ExamResultService} from '@/cbe/exams/services/exam-result.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {GradesService} from '@/academics/services/grades.service';
import {Status} from '@/core/enums/status';

@Component({
    selector: 'app-dashboard-exam-summary',
    templateUrl: './dashboard-exam-summary.component.html',
    styleUrl: './dashboard-exam-summary.component.scss'
})
export class DashboardExamSummaryComponent implements OnInit, OnDestroy {
    sessions: any[] = [];
    examTypes: any[] = [];
    schoolClasses: any[] = [];

    selectedSessionId: any = null;
    selectedExamTypeId: any = null;
    activeCurriculumId: number = null;
    activeAcademicYearId: number = null;

    classPerformance: {
        className: string;
        studentCount: number;
        classAverage: number;
        classAvgGrade: string;
        highestAvg: number;
        highestGrade: string;
        lowestAvg: number;
        lowestGrade: string;
        topStudent: string;
    }[] = [];

    grades: any[] = [];
    isLoading: boolean = false;
    hasLoaded: boolean = false;

    // Signals when the component is being torn down. Every HTTP subscription
    // in this widget pipes through `takeUntil(destroy$)` so navigating away
    // mid-load cancels the in-flight cascade — otherwise pending requests
    // would keep firing after `loadingState.resume()` and the global spinner
    // would stay up indefinitely on the page the user moved to.
    private destroy$ = new Subject<void>();
    showTopStudent: boolean = true;
    configuredExamTypeId: string = null;
    averageMethod: string = 'students_with_scores';

    constructor(
        private toastr: ToastrService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private schoolClassesSvc: SchoolClassesService,
        private examTypeSvc: ExamTypeService,
        private examSvc: ExamService,
        private examResultSvc: ExamResultService,
        private curriculaSvc: CurriculumService,
        private studentClassSvc: StudentClassService,
        private globalSettingSvc: GlobalSettingService,
        private gradesSvc: GradesService,
        private loadingState: LoadingStateService
    ) {}

    ngOnInit(): void {
        // Class exam performance fans out into many sequential per-class queries
        // and can take a while on a populated school. Suspend the global spinner
        // for the lifetime of this widget so the user can browse and act on the
        // rest of the dashboard while these requests trickle in. The widget's
        // own rows will appear as the data arrives.
        this.loadingState.suspend();

        forkJoin([
            this.globalSettingSvc.getByKey('General', 'ShowTopStudent').pipe(catchError(() => of(null))),
            this.globalSettingSvc.getByKey('General', 'CurrentExamType').pipe(catchError(() => of(null))),
            this.globalSettingSvc.getByKey('General', 'AverageCalculation').pipe(catchError(() => of(null))),
            this.gradesSvc.get('/grades').pipe(catchError(() => of([]))),
            this.globalSettingSvc.getByKey('Grading', 'ExamResults').pipe(catchError(() => of(null)))
        ]).pipe(takeUntil(this.destroy$)).subscribe({
            next: ([showTopSetting, examTypeSetting, avgSetting, allGrades, gradingSetting]) => {
                this.showTopStudent = showTopSetting?.settingValue !== 'false';
                this.configuredExamTypeId = examTypeSetting?.settingValue || null;
                this.averageMethod = avgSetting?.settingValue || 'students_with_scores';
                let gradingCategory = (gradingSetting as any)?.settingValue || '4-Point';
                this.grades = (allGrades as any[]).filter((g) => g.category === gradingCategory)
                    .sort((a, b) => (b.minScore || 0) - (a.minScore || 0));
                this.loadInitialData();
            },
            error: () => this.loadInitialData()
        });
    }

    loadInitialData() {
        let acadYearsReq = this.academicYearSvc.get('/academicYears');
        let examTypesReq = this.examTypeSvc.get('/examTypes');
        let curriculaReq = this.curriculaSvc.get('/curricula');

        forkJoin([acadYearsReq, examTypesReq, curriculaReq]).pipe(takeUntil(this.destroy$)).subscribe({
            next: ([academicYears, examTypes, curricula]) => {
                this.examTypes = examTypes.sort((a, b) => a.rank - b.rank);
                // Use configured exam type if it exists in the list
                if (this.configuredExamTypeId) {
                    let match = this.examTypes.find((et) => et.id.toString() === this.configuredExamTypeId);
                    if (match) this.selectedExamTypeId = match.id;
                }
                let activeYear = academicYears.find((y) => y.status === true);
                let topCurriculum = curricula.sort((a, b) => a.rank - b.rank)[0];
                if (!activeYear || !topCurriculum) return;
                this.activeAcademicYearId = parseInt(activeYear.id);
                this.activeCurriculumId = parseInt(topCurriculum.id);

                this.sessionsSvc.getByCurriculumYear(
                    this.activeCurriculumId,
                    this.activeAcademicYearId
                ).pipe(takeUntil(this.destroy$)).subscribe({
                    next: (sessions) => {
                        this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                        if (this.sessions.length > 0 && this.examTypes.length > 0) {
                            let openSession = this.sessions.find((s) => s.status === true);
                            this.selectedSessionId = openSession ? openSession.id : this.sessions[0].id;
                            if (!this.selectedExamTypeId) {
                                this.selectedExamTypeId = this.examTypes[0].id;
                            }
                            this.loadClassesAndPerformance();
                        }
                    },
                    error: () => {}
                });
            },
            error: () => {}
        });
    }

    loadClassesAndPerformance() {
        if (!this.selectedSessionId || !this.selectedExamTypeId) return;

        this.isLoading = true;
        this.classPerformance = [];
        this.hasLoaded = false;

        this.schoolClassesSvc.get('/schoolClasses/byAcademicYearId/' + this.activeAcademicYearId).pipe(takeUntil(this.destroy$)).subscribe({
            next: (classes) => {
                this.schoolClasses = classes.sort((a, b) => {
                    let nameA = a.learningLevel?.name || '';
                    let nameB = b.learningLevel?.name || '';
                    return nameA.localeCompare(nameB);
                });

                if (this.schoolClasses.length === 0) {
                    this.isLoading = false;
                    this.hasLoaded = true;
                    return;
                }

                this.processClassesSequentially(0);
            },
            error: () => {
                this.isLoading = false;
                this.hasLoaded = true;
            }
        });
    }

    processClassesSequentially(index: number) {
        if (index >= this.schoolClasses.length) {
            this.classPerformance.sort((a, b) => b.classAverage - a.classAverage);
            this.isLoading = false;
            this.hasLoaded = true;
            return;
        }

        let sc = this.schoolClasses[index];
        let className = (sc.learningLevel?.name || '') +
            (sc.schoolStream?.name ? ' - ' + sc.schoolStream.name : '');

        // Search for exams for this class
        let examUrl = `/exams/examSearch?academicYearId=${this.activeAcademicYearId}&curriculumId=${this.activeCurriculumId}&sessionId=${this.selectedSessionId}&schoolClassId=${sc.id}&examTypeId=${this.selectedExamTypeId}`;

        this.examSvc.get(examUrl).pipe(catchError(() => of([])), takeUntil(this.destroy$)).subscribe({
            next: (exams) => {
                if (!exams || exams.length === 0) {
                    this.processClassesSequentially(index + 1);
                    return;
                }

                // Get students in this class
                this.studentClassSvc.getBySchoolClassId(parseInt(sc.id), Status.Active)
                    .pipe(catchError(() => of([])), takeUntil(this.destroy$))
                    .subscribe({
                        next: (studentClasses) => {
                            let students = studentClasses.map((stc) => stc.student).filter(Boolean);
                            if (students.length === 0) {
                                this.processClassesSequentially(index + 1);
                                return;
                            }

                            // Get results for each exam
                            let resultRequests = exams.map((exam) =>
                                this.examResultSvc.get(`/examResults/byExamId/${exam.id}`)
                                    .pipe(catchError(() => of([])))
                            );

                            forkJoin(resultRequests).pipe(takeUntil(this.destroy$)).subscribe({
                                next: (allResults) => {
                                    // Build per-student scores
                                    let studentScores: { [studentId: number]: { total: number; count: number } } = {};
                                    exams.forEach((exam, idx) => {
                                        let examMark = exam.examMark || 100;
                                        (allResults[idx] || []).forEach((r: any) => {
                                            if (r.score == null) return;
                                            if (!studentScores[r.studentId]) {
                                                studentScores[r.studentId] = {total: 0, count: 0};
                                            }
                                            let pct = examMark > 0 ? (r.score / examMark) * 100 : 0;
                                            studentScores[r.studentId].total += pct;
                                            studentScores[r.studentId].count++;
                                        });
                                    });

                                    // Calculate per-student averages
                                    let studentAvgs: {name: string; avg: number}[] = [];
                                    let useAllStudents = this.averageMethod === 'all_allocated_students';
                                    students.forEach((student) => {
                                        let entry = studentScores[+student.id];
                                        if (entry && entry.count > 0) {
                                            studentAvgs.push({
                                                name: student.fullName || '',
                                                avg: entry.total / entry.count
                                            });
                                        } else if (useAllStudents) {
                                            studentAvgs.push({
                                                name: student.fullName || '',
                                                avg: 0
                                            });
                                        }
                                    });

                                    if (studentAvgs.length > 0) {
                                        let avgs = studentAvgs.map((s) => s.avg);
                                        let classAvg = avgs.reduce((a, b) => a + b, 0) / avgs.length;
                                        let highest = Math.max(...avgs);
                                        let lowest = Math.min(...avgs);
                                        let topStudent = studentAvgs.find((s) => s.avg === highest)?.name || '-';

                                        this.classPerformance.push({
                                            className,
                                            studentCount: studentAvgs.length,
                                            classAverage: Math.round(classAvg * 10) / 10,
                                            classAvgGrade: this.getGradeAbbr(classAvg),
                                            highestAvg: Math.round(highest * 10) / 10,
                                            highestGrade: this.getGradeAbbr(highest),
                                            lowestAvg: Math.round(lowest * 10) / 10,
                                            lowestGrade: this.getGradeAbbr(lowest),
                                            topStudent
                                        });
                                    }

                                    this.processClassesSequentially(index + 1);
                                },
                                error: () => this.processClassesSequentially(index + 1)
                            });
                        },
                        error: () => this.processClassesSequentially(index + 1)
                    });
            },
            error: () => this.processClassesSequentially(index + 1)
        });
    }

    getGradeAbbr(percent: number): string {
        if (this.grades.length === 0) return '';
        let rounded = Math.round(percent * 10) / 10;
        // Exact range match
        let grade = this.grades.find((g) => rounded >= +g.minScore && rounded <= +g.maxScore);
        if (grade) return grade.abbr;
        // Fallback: find closest grade by midpoint
        let closest = this.grades.reduce((prev, curr) => {
            let prevMid = (+prev.minScore + +prev.maxScore) / 2;
            let currMid = (+curr.minScore + +curr.maxScore) / 2;
            return Math.abs(currMid - rounded) < Math.abs(prevMid - rounded) ? curr : prev;
        });
        return closest ? closest.abbr : '';
    }

    getPerformanceColor(avg: number): string {
        if (avg >= 70) return 'text-success';
        if (avg >= 50) return 'text-info';
        if (avg >= 40) return 'text-warning';
        return 'text-danger';
    }

    getBarWidth(avg: number): number {
        return Math.min(avg, 100);
    }

    getBarColor(avg: number): string {
        if (avg >= 70) return 'bg-success';
        if (avg >= 50) return 'bg-info';
        if (avg >= 40) return 'bg-warning';
        return 'bg-danger';
    }

    ngOnDestroy(): void {
        // Cancel every in-flight HTTP subscription this widget started. Without
        // this, the recursive per-class chain keeps firing after navigation
        // and — because `resume()` below has already run — each new request
        // ticks the global spinner on whatever page the user moved to.
        this.destroy$.next();
        this.destroy$.complete();

        // Release the spinner-suspension so other pages get normal loader
        // behaviour after the user leaves the dashboard.
        this.loadingState.resume();
    }
}
