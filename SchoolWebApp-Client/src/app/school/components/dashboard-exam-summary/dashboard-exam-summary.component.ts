import {Component, OnDestroy, OnInit} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of, Subject} from 'rxjs';
import {catchError, takeUntil} from 'rxjs/operators';
import {LoadingStateService} from '@/core/services/loading-state.service';
import {AcademicYearsService} from '../../services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolExamService} from '@/cbe/exams/services/school-exam.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';

@Component({
    selector: 'app-dashboard-exam-summary',
    templateUrl: './dashboard-exam-summary.component.html',
    styleUrl: './dashboard-exam-summary.component.scss'
})
export class DashboardExamSummaryComponent implements OnInit, OnDestroy {
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolExams: any[] = [];

    selectedAcademicYearId: any = null;
    selectedSessionId: any = null;
    selectedSchoolExamId: any = null;
    // Derived from the selected school exam - the summary endpoint still filters
    // by exam type (and only released exams).
    selectedExamTypeId: any = null;
    activeCurriculumId: any = null;

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
        private schoolExamSvc: SchoolExamService,
        private curriculaSvc: CurriculumService,
        private globalSettingSvc: GlobalSettingService,
        private loadingState: LoadingStateService,
        private http: HttpClient
    ) {}

    ngOnInit(): void {
        // Class exam performance fans out into many sequential per-class queries
        // and can take a while on a populated school. Suspend the global spinner
        // for the lifetime of this widget so the user can browse and act on the
        // rest of the dashboard while these requests trickle in.
        this.loadingState.suspend();

        // ShowTopStudent (display toggle) + CurrentExamType (used to pick a
        // sensible default school exam) are the only client-side settings; the
        // averaging/grading happen server-side in /dashboard/classExamSummary.
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
        forkJoin([
            this.academicYearSvc.get('/academicYears'),
            this.curriculaSvc.get('/curricula')
        ]).pipe(takeUntil(this.destroy$)).subscribe({
            next: ([academicYears, curricula]) => {
                this.academicYears = academicYears.sort((a, b) => b.rank - a.rank);
                let activeYear = academicYears.find((y) => y.status === true) || this.academicYears[0];
                let topCurriculum = curricula.sort((a, b) => a.rank - b.rank)[0];
                if (!activeYear || !topCurriculum) return;
                this.selectedAcademicYearId = activeYear.id;
                this.activeCurriculumId = topCurriculum.id;
                // Initial load auto-runs once the defaults settle.
                this.loadSessions(true);
            },
            error: () => {}
        });
    }

    // Year drives Sessions, which drives School Exams. `autoLoad` runs the
    // summary once the cascade settles (used on first load only; manual filter
    // changes leave it to the Load button).
    private loadSessions(autoLoad: boolean) {
        this.sessions = this.schoolExams = [];
        this.selectedSessionId = this.selectedSchoolExamId = this.selectedExamTypeId = null;
        if (!this.activeCurriculumId || !this.selectedAcademicYearId) return;
        this.sessionsSvc
            .getByCurriculumYear(this.activeCurriculumId, this.selectedAcademicYearId)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: (sessions) => {
                    this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                    if (this.sessions.length === 0) return;
                    let openSession = this.sessions.find((s) => s.status === true);
                    this.selectedSessionId = openSession ? openSession.id : this.sessions[0].id;
                    this.loadSchoolExams(autoLoad);
                },
                error: () => {}
            });
    }

    private loadSchoolExams(autoLoad: boolean) {
        this.schoolExams = [];
        this.selectedSchoolExamId = this.selectedExamTypeId = null;
        if (!this.selectedSessionId || !this.activeCurriculumId || !this.selectedAcademicYearId) return;
        this.schoolExamSvc
            .get(`/schoolExams/examSearch?academicYearId=${this.selectedAcademicYearId}&curriculumId=${this.activeCurriculumId}&sessionId=${this.selectedSessionId}`)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
                next: (items) => {
                    // Only released school exams - the summary only counts those.
                    this.schoolExams = (items || [])
                        .filter((se: any) => se.isReleased)
                        .sort((a: any, b: any) => (b.examStartDate || '').localeCompare(a.examStartDate || ''));
                    if (this.schoolExams.length === 0) return;
                    // Default to the configured exam type's released exam, else
                    // the most recent released one.
                    let match = this.configuredExamTypeId
                        ? this.schoolExams.find((se: any) => String(se.examTypeId) === this.configuredExamTypeId)
                        : null;
                    let chosen = match || this.schoolExams[0];
                    this.selectedSchoolExamId = chosen.id;
                    this.selectedExamTypeId = chosen.examTypeId ?? chosen.examType?.id;
                    if (autoLoad) this.loadClassesAndPerformance();
                },
                error: () => {}
            });
    }

    onAcademicYearChange = () => {
        this.clearSummary();
        this.loadSessions(false);
    };

    onSessionChange = () => {
        this.clearSummary();
        this.loadSchoolExams(false);
    };

    onSchoolExamChange = () => {
        this.clearSummary();
        let se = this.schoolExams.find((s: any) => s.id == this.selectedSchoolExamId);
        this.selectedExamTypeId = se?.examTypeId ?? se?.examType?.id ?? null;
    };

    private clearSummary() {
        this.classPerformance = [];
        this.hasLoaded = false;
    }

    loadClassesAndPerformance() {
        if (!this.selectedSessionId || !this.selectedExamTypeId) return;

        this.isLoading = true;
        this.classPerformance = [];
        this.hasLoaded = false;

        // Single round-trip — the server does the per-class loop, builds the
        // ranked summary, and caches the result for 5 minutes.
        let params = new HttpParams()
            .set('academicYearId', String(this.selectedAcademicYearId))
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
        // Cancel every in-flight HTTP subscription this widget started.
        this.destroy$.next();
        this.destroy$.complete();

        // Release the spinner-suspension so other pages get normal loader
        // behaviour after the user leaves the dashboard.
        this.loadingState.resume();
    }
}
