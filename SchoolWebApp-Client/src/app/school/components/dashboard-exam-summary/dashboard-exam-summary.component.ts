import {Component, OnDestroy, OnInit} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of, Subject} from 'rxjs';
import {catchError, takeUntil} from 'rxjs/operators';
import {LoadingStateService} from '@/core/services/loading-state.service';
import {AcademicYearsService} from '../../services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {ExamTypeService} from '@/cbe/exams/services/exam-type.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';

@Component({
    selector: 'app-dashboard-exam-summary',
    templateUrl: './dashboard-exam-summary.component.html',
    styleUrl: './dashboard-exam-summary.component.scss'
})
export class DashboardExamSummaryComponent implements OnInit, OnDestroy {
    sessions: any[] = [];
    examTypes: any[] = [];

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

    constructor(
        private toastr: ToastrService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private examTypeSvc: ExamTypeService,
        private curriculaSvc: CurriculumService,
        private globalSettingSvc: GlobalSettingService,
        private loadingState: LoadingStateService,
        private http: HttpClient
    ) {}

    ngOnInit(): void {
        // Class exam performance fans out into many sequential per-class queries
        // and can take a while on a populated school. Suspend the global spinner
        // for the lifetime of this widget so the user can browse and act on the
        // rest of the dashboard while these requests trickle in. The widget's
        // own rows will appear as the data arrives.
        this.loadingState.suspend();

        // Settings that affect what the WIDGET displays vs. what the server
        // computes. Averaging method + grading category now live entirely
        // server-side in the new /dashboard/classExamSummary endpoint — we
        // only need ShowTopStudent (display toggle) and CurrentExamType
        // (initial dropdown selection) on the client.
        forkJoin([
            this.globalSettingSvc.getByKey('General', 'ShowTopStudent').pipe(catchError(() => of(null))),
            this.globalSettingSvc.getByKey('General', 'CurrentExamType').pipe(catchError(() => of(null)))
        ]).pipe(takeUntil(this.destroy$)).subscribe({
            next: ([showTopSetting, examTypeSetting]) => {
                this.showTopStudent = showTopSetting?.settingValue !== 'false';
                this.configuredExamTypeId = examTypeSetting?.settingValue || null;
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

        // Single round-trip — the server does the per-class loop, builds the
        // ranked summary, and caches the result for 5 minutes. Replaces what
        // used to be 3+ requests per class on the client (and was tripping
        // site4now's perimeter rate limiter into 403/CORS errors on schools
        // with many classes).
        let params = new HttpParams()
            .set('academicYearId', String(this.activeAcademicYearId))
            .set('curriculumId', String(this.activeCurriculumId))
            .set('sessionId', String(this.selectedSessionId))
            .set('examTypeId', String(this.selectedExamTypeId));

        this.http
            .get<any[]>('/dashboard/classExamSummary', {params})
            .pipe(catchError(() => of([])), takeUntil(this.destroy$))
            .subscribe({
                next: (rows) => {
                    this.classPerformance = (rows || []).map((r: any) => ({
                        className: r.className,
                        studentCount: r.studentCount,
                        classAverage: r.classAverage,
                        classAvgGrade: r.classAvgGrade,
                        highestAvg: r.highestAvg,
                        highestGrade: r.highestGrade,
                        lowestAvg: r.lowestAvg,
                        lowestGrade: r.lowestGrade,
                        topStudent: r.topStudent
                    }));
                    this.isLoading = false;
                    this.hasLoaded = true;
                },
                error: () => {
                    this.classPerformance = [];
                    this.isLoading = false;
                    this.hasLoaded = true;
                }
            });
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
