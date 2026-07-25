import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of, switchMap, map, Observable, catchError} from 'rxjs';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {GradesService} from '@/academics/services/grades.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {ExamService} from '@/cbe/exams/services/exam.service';
import {SchoolExamService} from '@/cbe/exams/services/school-exam.service';
import {ExamResultService} from '@/cbe/exams/services/exam-result.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {AuthService} from '@/core/services/auth.service';
import {ReportsService} from '@/reports/services/reports.service';
import {SubjectAnalysisNoteService} from '@/reports/services/subject-analysis-note.service';
import {Status} from '@/core/enums/status';

import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
(pdfMake as any).vfs = pdfFonts;

interface Band {
    abbr: string;
    minScore: number;
    maxScore: number;
    color: string;
}

// One exam's computed column: the class's overall performance across all
// subjects examined in that school exam.
interface ExamColumn {
    label: string;
    schoolExamId: number;
    nSat: number;
    highest: number;
    lowest: number;
    mean: number;
    meanGrade: string;
    bandCounts: {[abbr: string]: number};
    startDate: string;
}

@Component({
    selector: 'app-class-performance-trend',
    templateUrl: './class-performance-trend.component.html',
    styleUrl: './class-performance-trend.component.scss'
})
export class ClassPerformanceTrendComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/academics/class-performance'], title: 'Academics: Class Performance'}
    ];
    dashboardTitle = 'Academics: Class Performance Trend';

    curricula: any[] = [];
    academicYears: any[] = [];
    learningLevels: any[] = [];
    schoolClasses: any[] = [];
    allGrades: any[] = [];
    gradingSettings: any[] = [];
    averageMethod: string = 'students_with_scores';
    schoolExamsInYear: any[] = [];

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSchoolClassId: any = null;

    columns: ExamColumn[] = [];
    bands: Band[] = [];
    narrative: string = '';
    actionPlan: string = '';
    className: string = '';

    chartMean: {label: string; mean: number; x: number; y: number; h: number}[] = [];
    chartDist: {label: string; x: number; segments: {abbr: string; color: string; count: number; y: number; h: number}[]}[] = [];
    readonly chartW_col = 64;
    readonly chartPlotH = 120;
    readonly chartPlotTop = 12;
    readonly chartLeft = 34;

    loaded = false;
    isLoading = false;
    isSavingPlan = false;
    // The term/exam selected at the top; drives the action plan (view/save) and
    // which exam's plan Print All uses. Defaults to the latest released exam.
    focalExamId: any = null;
    isPrintingAll = false;

    constructor(
        private toastr: ToastrService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private schoolClassesSvc: SchoolClassesService,
        private learningLevelSvc: LearningLevelsService,
        private gradesSvc: GradesService,
        private globalSettingSvc: GlobalSettingService,
        private schoolExamSvc: SchoolExamService,
        private examSvc: ExamService,
        private examResultSvc: ExamResultService,
        private studentClassSvc: StudentClassService,
        private schoolSvc: SchoolDetailsService,
        private userSvc: AuthService,
        private reportSvc: ReportsService,
        private noteSvc: SubjectAnalysisNoteService
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.academicYearSvc.get('/academicYears'),
            this.gradesSvc.get('/grades'),
            this.globalSettingSvc.getByModule('Grading'),
            this.globalSettingSvc.getByKey('General', 'AverageCalculation')
        ]).subscribe({
            next: ([curricula, academicYears, allGrades, gradingSettings, avgSetting]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort((a, b) => b.rank - a.rank);
                this.allGrades = allGrades;
                this.gradingSettings = (gradingSettings as any[]) || [];
                this.averageMethod = (avgSetting as any)?.settingValue || 'students_with_scores';
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    private settingVal = (key: string): string =>
        this.gradingSettings.find((s) => s.settingKey === key)?.settingValue || '';

    private examGradingCategoryFor = (edLevelId: any): string => {
        let globalVal = this.settingVal('ExamResults') || '4-Point';
        if (!edLevelId) return globalVal;
        return this.settingVal(`ExamResults:${edLevelId}`) || globalVal;
    };

    private bandColor = (abbr: string): string => {
        let a = (abbr || '').toUpperCase();
        if (a.startsWith('EE')) return '#198754';
        if (a.startsWith('ME')) return '#0dcaf0';
        if (a.startsWith('AE')) return '#fd7e14';
        if (a.startsWith('BE')) return '#dc3545';
        return '#6c757d';
    };

    private gradeForPercent = (percent: number, grades: any[]): any =>
        grades.find((g) => percent >= g.minScore && percent <= g.maxScore);

    resetOutput = () => {
        this.loaded = false;
        this.columns = [];
        this.chartMean = [];
        this.chartDist = [];
        this.narrative = '';
    };

    onCurriculumChange = () => {
        this.schoolClasses = this.schoolExamsInYear = [];
        this.filterAcademicYearId = this.filterSchoolClassId = null;
        this.resetOutput();
        if (!this.filterCurriculumId) return;
        this.learningLevelSvc.getLearningLevelsByCurriculum(this.filterCurriculumId).subscribe({
            next: (levels) => { this.learningLevels = levels.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onAcademicYearChange = () => {
        this.schoolClasses = this.schoolExamsInYear = [];
        this.filterSchoolClassId = null;
        this.resetOutput();
        if (!this.filterAcademicYearId || !this.filterCurriculumId) return;

        forkJoin([
            this.sessionsSvc.get(`/sessions/byCurriculumYearId?curriculumId=${this.filterCurriculumId}&academicYearId=${this.filterAcademicYearId}`),
            this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${this.filterAcademicYearId}`)
        ]).pipe(
            switchMap(([sessions, schoolClasses]: any[]) => {
                let currLLIds = this.learningLevels.map((ll) => +ll.id);
                this.schoolClasses = schoolClasses
                    .filter((sc: any) => currLLIds.includes(+sc.learningLevelId))
                    .sort((a: any, b: any) => (a.rank || 0) - (b.rank || 0));

                let orderedSessions = (sessions as any[]).sort((a, b) => (a.rank || 0) - (b.rank || 0));
                if (!orderedSessions.length) return of([] as any[]);
                let reqs = orderedSessions.map((s: any, idx: number) =>
                    this.schoolExamSvc
                        .get(`/schoolExams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${s.id}`)
                        .pipe(map((exams: any[]) => (exams || []).map((e) => ({
                            ...e,
                            _sessionId: s.id,
                            _sessionName: s.sessionName || `Term ${idx + 1}`,
                            _sessionRank: s.rank || idx + 1
                        }))))
                );
                return forkJoin(reqs).pipe(map((perSession: any[][]) => perSession.flat()));
            })
        ).subscribe({
            next: (allSchoolExams: any[]) => {
                this.schoolExamsInYear = (allSchoolExams || [])
                    .filter((se) => se.isReleased)
                    .sort((a, b) =>
                        (a._sessionRank - b._sessionRank) ||
                        ((a.examStartDate || '').localeCompare(b.examStartDate || '')));
                this.focalExamId = this.schoolExamsInYear.length
                    ? this.schoolExamsInYear[this.schoolExamsInYear.length - 1].id : null;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onSchoolClassChange = () => this.resetOutput();

    private educationLevelIdFor(cls: any): any {
        return cls?.learningLevel?.educationLevelId
            ?? this.learningLevels.find((ll) => +ll.id === +(cls?.learningLevelId))?.educationLevelId;
    }
    private classEducationLevelId(): any {
        return this.educationLevelIdFor(this.schoolClasses.find((c) => c.id == this.filterSchoolClassId));
    }

    // Grade bands (highest → lowest) for a class's education level.
    private bandsFor(edLevelId: any): Band[] {
        return this.allGrades
            .filter((g) => g.category === this.examGradingCategoryFor(edLevelId))
            .slice()
            .sort((a, b) => b.minScore - a.minScore)
            .map((g) => ({abbr: g.abbr, minScore: g.minScore, maxScore: g.maxScore, color: this.bandColor(g.abbr)}));
    }

    // Fetch + compute one class's overall exam columns across the year (no state
    // mutation), reusable for the on-screen load and Print All.
    private computeColumns(schoolClassId: number, grades: any[], bands: Band[], studentCount: number): Observable<ExamColumn[]> {
        if (!this.schoolExamsInYear.length) return of([]);
        let reqs = this.schoolExamsInYear.map((se) => {
            let url = `/exams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${se._sessionId}&schoolClassId=${schoolClassId}&examTypeId=${se.examTypeId ?? se.examType?.id}`;
            return this.examSvc.get(url).pipe(
                switchMap((exams: any[]) => {
                    if (!exams || !exams.length) return of(null);
                    let resultReqs = exams.map((e) => this.examResultSvc.get(`/examResults/byExamId/${e.id}`));
                    return forkJoin(resultReqs).pipe(
                        map((allResults: any[][]) => this.buildColumn(se, exams, allResults, grades, studentCount, bands))
                    );
                })
            );
        });
        return forkJoin(reqs).pipe(map((cols) => (cols.filter(Boolean) as ExamColumn[]).filter((c) => c.nSat > 0)));
    }

    loadReport = () => {
        if (!this.filterSchoolClassId) {
            this.toastr.info('Please select a Class.');
            return;
        }
        if (!this.schoolExamsInYear.length) {
            this.toastr.info('No released school exams found for the selected year.');
            return;
        }

        let cls = this.schoolClasses.find((c) => c.id == this.filterSchoolClassId);
        this.className = cls?.name || '';

        let edLevelId = this.classEducationLevelId();
        let grades = this.allGrades
            .filter((g) => g.category === this.examGradingCategoryFor(edLevelId))
            .sort((a, b) => a.rank - b.rank);
        this.bands = this.bandsFor(edLevelId);

        this.isLoading = true;
        this.resetOutput();

        this.studentClassSvc.getBySchoolClassId(this.filterSchoolClassId, Status.Active).pipe(
            switchMap((studentClasses) => {
                let studentCount = (studentClasses || []).filter((sc: any) => sc.student).length;
                return this.computeColumns(this.filterSchoolClassId, grades, this.bands, studentCount);
            })
        ).subscribe({
            next: (columns) => {
                this.columns = columns;
                this.buildCharts();
                this.narrative = this.buildNarrativeFor(this.columns, this.className, this.bands);
                this.loaded = true;
                this.isLoading = false;
                if (!this.columns.length) this.toastr.info('No exam results found for this class in the selected year.');
                this.loadActionPlan();
            },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error); }
        });
    };

    // Aggregate every subject in the school exam into per-student overall
    // averages, then reduce to class stats + a grade-band distribution.
    private buildColumn(se: any, exams: any[], allResults: any[][], grades: any[], studentCount: number, bands: Band[]): ExamColumn {
        let studentAgg = new Map<number, {sumPct: number; count: number}>();
        exams.forEach((exam, idx) => {
            let examMark = exam.examMark || 100;
            (allResults[idx] || []).forEach((r: any) => {
                if (r.score == null) return;
                let a = studentAgg.get(+r.studentId) || {sumPct: 0, count: 0};
                a.sumPct += examMark > 0 ? (r.score / examMark) * 100 : 0;
                a.count++;
                studentAgg.set(+r.studentId, a);
            });
        });

        let overalls = [...studentAgg.values()].filter((a) => a.count > 0).map((a) => a.sumPct / a.count);

        let bandCounts: {[abbr: string]: number} = {};
        bands.forEach((b) => (bandCounts[b.abbr] = 0));
        overalls.forEach((p) => {
            let g = this.gradeForPercent(Math.round(p), grades);
            if (g && bandCounts[g.abbr] != null) bandCounts[g.abbr]++;
        });

        let sum = overalls.reduce((s, p) => s + p, 0);
        let mean = 0;
        if (this.averageMethod === 'all_allocated_students' && studentCount > 0) mean = sum / studentCount;
        else if (overalls.length > 0) mean = sum / overalls.length;
        mean = Math.round(mean * 10) / 10;

        return {
            label: `${se._sessionName} - ${se.examType?.name || 'Exam'}`,
            schoolExamId: se.id,
            nSat: overalls.length,
            highest: overalls.length ? Math.round(Math.max(...overalls) * 10) / 10 : 0,
            lowest: overalls.length ? Math.round(Math.min(...overalls) * 10) / 10 : 0,
            mean,
            meanGrade: this.gradeForPercent(Math.round(mean), grades)?.abbr || '',
            bandCounts,
            startDate: se.examStartDate || ''
        };
    }

    private meetingOrAbove(col: ExamColumn, bands: Band[]): number {
        return bands
            .filter((b) => b.abbr.toUpperCase().startsWith('ME') || b.abbr.toUpperCase().startsWith('EE'))
            .reduce((sum, b) => sum + (col.bandCounts[b.abbr] || 0), 0);
    }

    private buildNarrativeFor(columns: ExamColumn[], className: string, bands: Band[]): string {
        if (!columns.length) return '';
        let first = columns[0];
        let last = columns[columns.length - 1];
        if (columns.length === 1) {
            let ma = this.meetingOrAbove(first, bands);
            let pct = first.nSat ? Math.round((ma / first.nSat) * 100) : 0;
            return `Only one assessment (${first.label}) is available for ${className}. ` +
                `The class mean was ${first.mean}% (highest ${first.highest}%, lowest ${first.lowest}%), with ${ma} of ${first.nSat} learners (${pct}%) meeting expectations or above. ` +
                `A trend comparison will be available once a second assessment is recorded.`;
        }

        let delta = Math.round((last.mean - first.mean) * 10) / 10;
        let dir = delta > 0 ? 'improved' : delta < 0 ? 'declined' : 'held steady';
        let move = delta > 0 ? 'rise' : delta < 0 ? 'drop' : 'change';
        let ma = this.meetingOrAbove(last, bands);
        let pct = last.nSat ? Math.round((ma / last.nSat) * 100) : 0;

        return `Across ${columns.length} assessments, the overall class mean for ${className} ${dir} from ` +
            `${first.mean}% (${first.label}) to ${last.mean}% (${last.label}) - a ${Math.abs(delta)} percentage-point ${move}. ` +
            `The highest learner average moved from ${first.highest}% to ${last.highest}%, and the lowest from ${first.lowest}% to ${last.lowest}%. ` +
            `In the latest assessment, ${ma} of ${last.nSat} learners (${pct}%) met expectations (ME) or above.`;
    }

    private buildCharts() {
        this.chartMean = [];
        this.chartDist = [];
        if (!this.columns.length) return;

        let maxN = Math.max(1, ...this.columns.map((c) => c.nSat));
        let bottom = this.chartPlotTop + this.chartPlotH;

        this.columns.forEach((c, i) => {
            let x = this.chartLeft + i * this.chartW_col + (this.chartW_col - 34) / 2;
            let mh = (c.mean / 100) * this.chartPlotH;
            this.chartMean.push({label: c.label, mean: c.mean, x, y: bottom - mh, h: mh});

            let segs: {abbr: string; color: string; count: number; y: number; h: number}[] = [];
            let yCursor = bottom;
            [...this.bands].reverse().forEach((b) => {
                let count = c.bandCounts[b.abbr] || 0;
                let h = (count / maxN) * this.chartPlotH;
                yCursor -= h;
                segs.push({abbr: b.abbr, color: b.color, count, y: yCursor, h});
            });
            this.chartDist.push({label: c.label, x, segments: segs});
        });
    }

    get chartWidth(): number {
        return this.chartLeft + Math.max(1, this.columns.length) * this.chartW_col + 10;
    }
    get chartBottom(): number {
        return this.chartPlotTop + this.chartPlotH;
    }

    // "x,y x,y …" of the mean points for the on-screen line chart.
    get meanLinePoints(): string {
        return this.chartMean.map((m) => `${m.x + 17},${m.y}`).join(' ');
    }

    // Split a "Term 2 - Mid-Term Exam" label into two lines to avoid overlap.
    labelParts = (label: string): string[] => {
        let idx = (label || '').indexOf(' - ');
        return idx < 0 ? [label || '', ''] : [label.substring(0, idx), label.substring(idx + 3)];
    };

    // Load the saved plan for the top-selected term/exam.
    private loadActionPlan() {
        this.fetchActionPlan();
    }

    onFocalExamChange = () => { if (this.loaded) this.fetchActionPlan(); };

    // subjectId 0 = class-level note; keyed also by the selected school exam.
    private fetchActionPlan() {
        this.actionPlan = '';
        if (!this.focalExamId) return;
        this.noteSvc.getByParams(+this.filterAcademicYearId, +this.filterSchoolClassId, 0, +this.focalExamId).subscribe({
            next: (note) => { this.actionPlan = note?.actionPlan || ''; },
            error: () => {}
        });
    }

    saveActionPlan = () => {
        if (!this.loaded || !this.focalExamId) return;
        this.isSavingPlan = true;
        this.noteSvc.upsert({
            academicYearId: +this.filterAcademicYearId,
            schoolClassId: +this.filterSchoolClassId,
            subjectId: 0,
            schoolExamId: +this.focalExamId,
            actionPlan: this.actionPlan
        }).subscribe({
            next: () => { this.isSavingPlan = false; this.toastr.success('Action plan saved.'); },
            error: (err) => { this.isSavingPlan = false; this.toastr.error(err?.error?.message || err?.error || 'Failed to save action plan.'); }
        });
    };

    get focalExamLabel(): string {
        let se = this.schoolExamsInYear.find((s) => s.id == this.focalExamId);
        return se ? `${se._sessionName} - ${se.examType?.name || 'Exam'}` : '';
    }

    getFilterName = (type: string): string => {
        if (type === 'year') return this.academicYears.find((y) => y.id == this.filterAcademicYearId)?.name || '';
        return '';
    };

    // Single-report preview of the current on-screen selection.
    previewReport = () => {
        if (!this.columns.length) {
            this.toastr.info('Load the report first.');
            return;
        }
        this.withSchoolAndLogo((school, logo) => {
            let meta = {className: this.className, actionPlan: this.actionPlan, examLabel: this.focalExamLabel};
            pdfMake.createPdf(this.buildDocDefinition(this.columns, this.bands, meta, school, logo)).getBlob((pdfBlob) => {
                window.open(URL.createObjectURL(pdfBlob), '_blank');
            });
        });
    };

    // Print every class (in the selected curriculum + year) into one combined
    // PDF, each using the action plan saved for the chosen batch exam.
    printAll = () => {
        if (!this.schoolClasses.length) { this.toastr.info('No classes found for the selected year.'); return; }
        if (!this.schoolExamsInYear.length || !this.focalExamId) { this.toastr.info('No released school exams for the selected year.'); return; }
        let examLabel = this.focalExamLabel;

        this.isPrintingAll = true;
        forkJoin([
            this.schoolSvc.get('/schooldetails'),
            this.reportSvc.loadImageAsBase64('assets/img/shule-nova-logo-only.png').pipe(switchMap((blob) => this.reportSvc.readBlobAsBase64(blob)))
        ]).pipe(
            switchMap(([school, logo]: any[]) => {
                // Each class may be a different education level → its own bands.
                let perClass = this.schoolClasses.map((cls) => {
                    let edLevelId = this.educationLevelIdFor(cls);
                    let grades = this.allGrades.filter((g) => g.category === this.examGradingCategoryFor(edLevelId)).sort((a, b) => a.rank - b.rank);
                    let bands = this.bandsFor(edLevelId);
                    return this.studentClassSvc.getBySchoolClassId(cls.id, Status.Active).pipe(
                        switchMap((scs) => {
                            let studentCount = (scs || []).filter((sc: any) => sc.student).length;
                            return forkJoin([
                                this.computeColumns(cls.id, grades, bands, studentCount),
                                this.noteSvc.getByParams(+this.filterAcademicYearId, +cls.id, 0, +this.focalExamId).pipe(catchError(() => of(null)))
                            ]).pipe(map(([columns, note]: any[]) => ({cls, columns, bands, actionPlan: note?.actionPlan || ''})));
                        })
                    );
                });
                return forkJoin(perClass).pipe(map((results) => ({school, logo, results})));
            })
        ).subscribe({
            next: ({school, logo, results}: any) => {
                let withData = results.filter((r: any) => r.columns.length > 0);
                if (!withData.length) {
                    this.isPrintingAll = false;
                    this.toastr.info('No exam results found for any class in the selected year.');
                    return;
                }
                let docDefs = withData.map((r: any) =>
                    this.buildDocDefinition(r.columns, r.bands, {className: r.cls.name || '', actionPlan: r.actionPlan, examLabel}, school[0], logo));
                this.reportSvc.printBatchDocs(docDefs.map((d: any) => of(d))).subscribe({
                    next: () => { this.isPrintingAll = false; },
                    error: () => { this.isPrintingAll = false; this.toastr.error('Error generating the combined PDF.'); }
                });
            },
            error: (err) => { this.isPrintingAll = false; this.toastr.error(err?.error || 'Error preparing the combined report.'); }
        });
    };

    private withSchoolAndLogo(cb: (school: any, logo: string) => void) {
        this.schoolSvc.get('/schooldetails').subscribe({
            next: (school) => {
                this.reportSvc.loadImageAsBase64('assets/img/shule-nova-logo-only.png')
                    .pipe(switchMap((blob) => this.reportSvc.readBlobAsBase64(blob)))
                    .subscribe({
                        next: (logo) => cb(school[0], logo),
                        error: () => this.toastr.error('Error loading logo.')
                    });
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    private buildDocDefinition(columns: ExamColumn[], bands: Band[], meta: {className: string; actionPlan: string; examLabel: string}, school: any, systemLogo: string): any {
        let title = `CLASS PERFORMANCE ANALYSIS - ${meta.className} (${this.getFilterName('year')})`.toUpperCase();
        let narrative = this.buildNarrativeFor(columns, meta.className, bands);

        let summaryHeader = [{text: '', style: 'tableHeader'}, ...columns.map((c) => ({text: c.label, style: 'tableHeader', alignment: 'center'}))];
        let summaryRows = [
            summaryHeader,
            ['Learners sat', ...columns.map((c) => ({text: c.nSat, alignment: 'center'}))],
            ['Highest average', ...columns.map((c) => ({text: c.highest + '%', alignment: 'center'}))],
            ['Lowest average', ...columns.map((c) => ({text: c.lowest + '%', alignment: 'center'}))],
            ['Class mean', ...columns.map((c) => ({text: c.mean + '%', alignment: 'center', bold: true}))],
            ['Mean grade', ...columns.map((c) => ({text: c.meanGrade, alignment: 'center', bold: true}))]
        ];
        let distHeader = [{text: 'Grade', style: 'tableHeader'}, ...columns.map((c) => ({text: c.label, style: 'tableHeader', alignment: 'center'}))];
        let distRows = [distHeader, ...bands.map((b) => [
            {text: b.abbr, bold: true},
            ...columns.map((c) => ({text: c.bandCounts[b.abbr] || 0, alignment: 'center'}))
        ])];
        let colWidths = [85, ...columns.map(() => '*')];

        let content: any[] = [
            {...this.reportSvc.getDIVIDER('portrait')},
            this.reportSvc.getReportHeader(school),
            {...this.reportSvc.getDIVIDER('portrait'), marginBottom: 1},
            this.reportSvc.getReportTitle(title),
            {text: meta.examLabel, alignment: 'center', bold: true, fontSize: 10, color: '#333', marginBottom: 2},
            {...this.reportSvc.getDIVIDER('portrait'), marginBottom: 4},

            {text: 'Performance Analysis', bold: true, fontSize: 11, marginBottom: 2},
            {layout: this.reportSvc.getTableLayout(), table: {headerRows: 1, widths: colWidths, body: summaryRows}, fontSize: 9, marginBottom: 8},

            {text: 'Performance Distribution', bold: true, fontSize: 11, marginBottom: 2},
            {layout: this.reportSvc.getTableLayout(), table: {headerRows: 1, widths: colWidths, body: distRows}, fontSize: 9, marginBottom: 8},

            {text: 'Charts', bold: true, fontSize: 11, marginBottom: 2},
            {columns: [this.meanChartCanvas(columns), this.distChartSvg(columns, bands)], columnGap: 10, marginBottom: 8},

            {text: 'Analysis', bold: true, fontSize: 11, marginBottom: 2},
            {text: narrative, fontSize: 9, alignment: 'justify', marginBottom: 8},

            {text: `Conclusion & Action Plan${meta.examLabel ? ' (' + meta.examLabel + ')' : ''}`, bold: true, fontSize: 11, marginBottom: 2},
            {text: meta.actionPlan || '', fontSize: 9, alignment: 'justify', marginBottom: 8},

            {...this.reportSvc.getDIVIDER('portrait')},
            this.reportSvc.getPrintDetails(
                (this.userSvc?.currentUser?.firstName || '') + ' ' + (this.userSvc?.currentUser?.lastName || ''),
                new Date().toLocaleString('en-GB')
            )
        ];

        return {
            pageSize: 'A4',
            pageOrientation: 'portrait',
            pageMargins: [25, 20, 25, 30],
            info: {title, author: 'ShuleNova', subject: title},
            watermark: this.reportSvc.getWatermark('ShuleNova - ' + (school?.name || '')),
            footer: this.reportSvc.getFooter('portrait'),
            images: {systemLogo, schoolLogo: school?.logoAsBase64},
            styles: {tableHeader: this.reportSvc.getHEADER_STYLE()},
            content
        };
    }

    private meanChartCanvas(columns: ExamColumn[]): any {
        const plotH = 90, colW = 46, left = 24, base = plotH + 12;
        let pts = columns.map((c, i) => ({x: left + i * colW + 19, y: base - (c.mean / 100) * plotH}));
        let canvas: any[] = [{type: 'line', x1: left, y1: base, x2: left + columns.length * colW + 4, y2: base, lineWidth: 0.5, lineColor: '#999'}];
        for (let i = 0; i < pts.length - 1; i++)
            canvas.push({type: 'line', x1: pts[i].x, y1: pts[i].y, x2: pts[i + 1].x, y2: pts[i + 1].y, lineWidth: 1.5, lineColor: '#4169E1'});
        pts.forEach((p) => canvas.push({type: 'ellipse', x: p.x, y: p.y, r1: 2, r2: 2, color: '#4169E1'}));
        return {
            width: 'auto',
            stack: [
                {text: 'Class mean (%)', fontSize: 8, bold: true, marginBottom: 2},
                {canvas},
                {columns: columns.map((c) => ({text: c.mean + '%', fontSize: 6, bold: true, alignment: 'center'})), marginTop: 1},
                {columns: columns.map((c) => ({text: c.label, fontSize: 5, alignment: 'center'})), columnGap: 0, marginTop: 1}
            ]
        };
    }

    private esc = (s: string): string =>
        (s || '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');

    // Stacked distribution bars as an SVG so the learner count can be stamped
    // inside each segment (pdfMake's canvas element can't render text).
    private distChartSvg(columns: ExamColumn[], bands: Band[]): any {
        const colW = 46, left = 24, plotH = 90, base = plotH + 12, barW = 26;
        const iw = left + columns.length * colW + 10;
        const ih = base + 24;
        let maxN = Math.max(1, ...columns.map((c) => c.nSat));
        let p: string[] = [`<line x1="${left}" y1="${base}" x2="${left + columns.length * colW + 4}" y2="${base}" stroke="#999" stroke-width="0.5"/>`];
        columns.forEach((c, i) => {
            let bx = left + i * colW + 6;
            let yc = base;
            [...bands].reverse().forEach((b) => {
                let count = c.bandCounts[b.abbr] || 0;
                let h = (count / maxN) * plotH;
                if (h > 0) {
                    yc -= h;
                    p.push(`<rect x="${bx}" y="${yc}" width="${barW}" height="${h}" fill="${b.color}"/>`);
                    if (h >= 9) p.push(`<text x="${bx + barW / 2}" y="${yc + h / 2 + 2.5}" text-anchor="middle" font-size="7" font-weight="bold" fill="#ffffff">${count}</text>`);
                }
            });
            let parts = this.labelParts(c.label);
            p.push(`<text x="${bx + barW / 2}" y="${base + 9}" text-anchor="middle" font-size="5" fill="#555">${this.esc(parts[0])}</text>`);
            p.push(`<text x="${bx + barW / 2}" y="${base + 16}" text-anchor="middle" font-size="5" fill="#555">${this.esc(parts[1])}</text>`);
        });
        let svg = `<svg xmlns="http://www.w3.org/2000/svg" width="${iw}" height="${ih}" viewBox="0 0 ${iw} ${ih}">${p.join('')}</svg>`;
        return {
            width: 'auto',
            stack: [
                {text: 'Distribution (learners)', fontSize: 8, bold: true, marginBottom: 2},
                {svg, width: Math.min(iw, 250)},
                {columns: bands.map((b) => ({text: `■ ${b.abbr}`, fontSize: 6, color: b.color, alignment: 'center'})), marginTop: 2}
            ]
        };
    }
}
