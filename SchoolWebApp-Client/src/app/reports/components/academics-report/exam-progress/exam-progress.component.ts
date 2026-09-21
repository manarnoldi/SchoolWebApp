import {Component, Input, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of, switchMap, map} from 'rxjs';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {ExamService} from '@/cbe/exams/services/exam.service';
import {ExamTypeService} from '@/cbe/exams/services/exam-type.service';
import {SchoolExamService} from '@/cbe/exams/services/school-exam.service';
import {ExamResultService} from '@/cbe/exams/services/exam-result.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {AuthService} from '@/core/services/auth.service';
import {ReportsService} from '@/reports/services/reports.service';
import {Status} from '@/core/enums/status';

import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
(pdfMake as any).vfs = pdfFonts;

// One sitting of the chosen exam type - a single week of a weekly marathon.
interface Sitting {
    schoolExamId: number;
    label: string;
    startDate: string;
}

// A student's run of marks across those sittings, in date order.
interface StudentRow {
    studentId: number;
    upi: string;
    name: string;
    // Percentage per sitting; null where the student has no mark.
    marks: (number | null)[];
    average: number | null;
    // Last sitting less the first one the student sat - the progress figure.
    change: number | null;
    sat: number;
}

@Component({
    selector: 'app-exam-progress',
    templateUrl: './exam-progress.component.html'
})
export class ExamProgressComponent implements OnInit {
    // 'class' lists every learner's run with the class average; 'student' works
    // one learner at a time, subject by subject, and prints them in bulk. Same
    // data and same filters - two ways of reading it, hosted as two tabs.
    @Input() mode: 'class' | 'student' = 'class';

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/academics/exam-progress'], title: 'Academics: Exam Progress'}
    ];
    dashboardTitle = 'Academics: Exam Progress';

    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    examTypes: any[] = [];

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterSchoolClassId: any = null;
    filterExamTypeId: any = null;
    // 'all' averages every subject the student sat in a sitting; otherwise the
    // marks are that one subject's. Applied to the loaded data, so changing it
    // does not re-fetch.
    filterSubjectId: any = 'all';

    // Subjects found in the loaded sittings, for the subject selector.
    subjects: any[] = [];
    sittings: Sitting[] = [];
    rows: StudentRow[] = [];
    classAverages: (number | null)[] = [];
    // studentId -> sittingIndex -> [{subjectId, pct}], the loaded marks that
    // rows/classAverages are computed from.
    private marksByStudent = new Map<number, Map<number, {subjectId: number; pct: number}[]>>();

    isLoading = false;
    loaded = false;

    constructor(
        private toastr: ToastrService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private schoolClassesSvc: SchoolClassesService,
        private schoolSvc: SchoolDetailsService,
        private examSvc: ExamService,
        private examTypeSvc: ExamTypeService,
        private schoolExamSvc: SchoolExamService,
        private examResultSvc: ExamResultService,
        private studentClassSvc: StudentClassService,
        private userSvc: AuthService,
        private reportSvc: ReportsService
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.academicYearSvc.get('/academicYears'),
            this.examTypeSvc.get('/examTypes')
        ]).subscribe({
            next: ([curricula, academicYears, examTypes]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort((a, b) => b.rank - a.rank);
                // Types that can hold several exams a term first: those are the
                // ones this report is for.
                this.examTypes = (examTypes as any[]).sort((a, b) =>
                    (b.allowMultiplePerTerm ? 1 : 0) - (a.allowMultiplePerTerm ? 1 : 0) || a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    onCurriculumChange = () => {
        this.filterAcademicYearId = this.filterSessionId = this.filterSchoolClassId = null;
        this.sessions = this.schoolClasses = [];
        this.reset();
    };

    onAcademicYearChange = () => {
        this.filterSessionId = this.filterSchoolClassId = null;
        this.reset();
        if (!this.filterCurriculumId || !this.filterAcademicYearId) return;
        forkJoin([
            this.sessionsSvc.get(`/sessions/byCurriculumYearId?curriculumId=${this.filterCurriculumId}&academicYearId=${this.filterAcademicYearId}`),
            this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${this.filterAcademicYearId}`)
        ]).subscribe({
            next: ([sessions, classes]) => {
                this.sessions = sessions;
                this.schoolClasses = classes;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onFilterChange = () => this.reset();

    private reset(): void {
        this.loaded = false;
        this.rows = [];
        this.sittings = [];
        this.subjects = [];
        this.classAverages = [];
        this.marksByStudent.clear();
        this.filterSubjectId = 'all';
        this.selectedStudentId = null;
    }

    examTypeName = (): string =>
        this.examTypes.find((et) => et.id == this.filterExamTypeId)?.name || '';

    filterName = (which: 'year' | 'session' | 'class'): string => {
        if (which === 'year') return this.academicYears.find((y) => y.id == this.filterAcademicYearId)?.name || '';
        if (which === 'session') return this.sessions.find((s) => s.id == this.filterSessionId)?.sessionName || '';
        return this.schoolClasses.find((c) => c.id == this.filterSchoolClassId)?.name || '';
    };

    subjectName = (): string =>
        this.filterSubjectId === 'all'
            ? 'All subjects (average)'
            : this.subjects.find((s) => s.id == this.filterSubjectId)?.name || '';

    loadReport = () => {
        if (!this.filterSessionId || !this.filterSchoolClassId || !this.filterExamTypeId) {
            this.toastr.info('Please select Term, Class and Exam Type.');
            return;
        }
        this.isLoading = true;
        this.reset();

        // Every exam of this type registered in the term, oldest first: those
        // are the report's columns.
        this.schoolExamSvc
            .get(`/schoolExams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${this.filterSessionId}`)
            .pipe(
                switchMap((schoolExams: any[]) => {
                    let mine = (schoolExams || [])
                        .filter((se) => +(se.examTypeId ?? se.examType?.id) === +this.filterExamTypeId)
                        .sort((a, b) => (a.examStartDate || '').localeCompare(b.examStartDate || ''));
                    if (!mine.length) return of(null);

                    this.sittings = mine.map((se) => ({
                        schoolExamId: +se.id,
                        label: se.description || this.asShortDate(se.examStartDate),
                        startDate: se.examStartDate
                    }));

                    // Per sitting: the class's exams, then each exam's results.
                    let perSitting = mine.map((se) =>
                        this.examSvc
                            .get(`/exams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${this.filterSessionId}&schoolClassId=${this.filterSchoolClassId}&schoolExamId=${se.id}`)
                            .pipe(
                                switchMap((exams: any[]) => {
                                    if (!exams?.length) return of({exams: [], results: [] as any[][]});
                                    return forkJoin(exams.map((e) => this.examResultSvc.get(`/examResults/byExamId/${e.id}`)))
                                        .pipe(map((results) => ({exams, results: results as any[][]})));
                                })
                            )
                    );

                    return forkJoin([
                        this.studentClassSvc.getBySchoolClassId(this.filterSchoolClassId, Status.Active),
                        forkJoin(perSitting)
                    ]);
                })
            )
            .subscribe({
                next: (res: any) => {
                    this.isLoading = false;
                    if (!res) {
                        this.toastr.info(`No ${this.examTypeName()} exams are registered for this term.`);
                        return;
                    }
                    let [studentClasses, sittingData] = res;
                    this.collectMarks(studentClasses, sittingData);
                    this.applySubject();
                    this.loaded = true;
                    if (!this.rows.length) this.toastr.info('No marks found for this selection.');
                },
                error: (err) => {
                    this.isLoading = false;
                    this.toastr.error(err.error?.message || 'Error loading the report.');
                }
            });
    };

    // Keeps every mark as a percentage, per student, per sitting, per subject.
    // The subject selector then reads from this without re-fetching.
    private collectMarks(studentClasses: any[], sittingData: any[]): void {
        let students = (studentClasses || []).filter((sc) => sc.student);
        this.studentsById = new Map(students.map((sc) => [+sc.student.id, sc.student]));

        let subjectsSeen = new Map<number, any>();
        sittingData.forEach((sd, sittingIdx) => {
            (sd.exams || []).forEach((exam: any, examIdx: number) => {
                if (exam.subject) subjectsSeen.set(+exam.subjectId, exam.subject);
                let outOf = exam.examMark || 100;
                ((sd.results[examIdx] as any[]) || [])
                    .filter((r) => r.score != null)
                    .forEach((r) => {
                        let pct = outOf > 0 ? Math.round((r.score / outOf) * 1000) / 10 : 0;
                        let perSitting = this.marksByStudent.get(+r.studentId)
                            ?? new Map<number, {subjectId: number; pct: number}[]>();
                        let list = perSitting.get(sittingIdx) ?? [];
                        list.push({subjectId: +exam.subjectId, pct});
                        perSitting.set(sittingIdx, list);
                        this.marksByStudent.set(+r.studentId, perSitting);
                    });
            });
        });
        this.subjects = Array.from(subjectsSeen.values()).sort((a, b) =>
            (a.name || '').localeCompare(b.name || ''));
    }

    private studentsById = new Map<number, any>();

    // Rebuilds the rows for the chosen subject, or the average of all subjects.
    applySubject = () => {
        let rows: StudentRow[] = [];
        this.studentsById.forEach((student, studentId) => {
            let perSitting = this.marksByStudent.get(studentId);
            let marks: (number | null)[] = this.sittings.map((_, idx) => {
                let entries = (perSitting?.get(idx) || [])
                    .filter((m) => this.filterSubjectId === 'all' || +m.subjectId === +this.filterSubjectId);
                if (!entries.length) return null;
                let avg = entries.reduce((s, m) => s + m.pct, 0) / entries.length;
                return Math.round(avg * 10) / 10;
            });

            let sat = marks.filter((m) => m != null).length;
            if (!sat) return;
            let average = Math.round((marks.reduce((s: number, m) => s + (m ?? 0), 0) / sat) * 10) / 10;
            let scored = marks.filter((m) => m != null) as number[];
            let change = scored.length > 1
                ? Math.round((scored[scored.length - 1] - scored[0]) * 10) / 10
                : null;

            rows.push({
                studentId,
                upi: student.upi || '',
                name: student.fullName || '',
                marks, average, change, sat
            });
        });

        // Best average first, so the class list reads as a ranking.
        rows.sort((a, b) => (b.average ?? 0) - (a.average ?? 0) || a.name.localeCompare(b.name));
        this.rows = rows;

        this.classAverages = this.sittings.map((_, idx) => {
            let vals = rows.map((r) => r.marks[idx]).filter((m) => m != null) as number[];
            return vals.length ? Math.round((vals.reduce((s, v) => s + v, 0) / vals.length) * 10) / 10 : null;
        });
    };

    // Improvement, decline or no change - used for the arrow and its colour.
    trend = (change: number | null): 'up' | 'down' | 'flat' | 'none' => {
        if (change == null) return 'none';
        if (change > 0.05) return 'up';
        if (change < -0.05) return 'down';
        return 'flat';
    };

    // --- Week on week ---------------------------------------------------
    // What a teacher asks at each sitting: did this learner improve on their
    // last one? Compares a mark with the previous exam the learner actually
    // sat, so a missed week does not read as a collapse.
    weekDelta = (marks: (number | null)[], idx: number): number | null => {
        if (marks[idx] == null) return null;
        for (let i = idx - 1; i >= 0; i--)
            if (marks[i] != null) return Math.round((marks[idx]! - marks[i]!) * 10) / 10;
        return null;
    };

    // --- Per-student detail ---------------------------------------------
    selectedStudentId: number | null = null;

    selectStudent = (row: StudentRow) => {
        this.selectedStudentId = this.selectedStudentId === row.studentId ? null : row.studentId;
    };

    selectedStudent = (): StudentRow | null =>
        this.rows.find((r) => r.studentId === this.selectedStudentId) || null;

    // The chosen learner's marks broken down by subject, one row per subject
    // across the same sittings - the subject-by-subject view of their progress.
    studentSubjectRows = () =>
        this.selectedStudentId == null ? [] : this.subjectRowsFor(this.selectedStudentId);

    subjectRowsFor = (studentId: number): {name: string; marks: (number | null)[]; average: number | null; change: number | null}[] => {
        let perSitting = this.marksByStudent.get(studentId);
        if (!perSitting) return [];

        return this.subjects.map((subject) => {
            let marks = this.sittings.map((_, idx) => {
                let entry = (perSitting!.get(idx) || []).find((m) => +m.subjectId === +subject.id);
                return entry ? entry.pct : null;
            });
            let scored = marks.filter((m) => m != null) as number[];
            let average = scored.length
                ? Math.round((scored.reduce((s, v) => s + v, 0) / scored.length) * 10) / 10 : null;
            let change = scored.length > 1
                ? Math.round((scored[scored.length - 1] - scored[0]) * 10) / 10 : null;
            return {name: subject.name || '', marks, average, change};
        }).filter((r) => r.marks.some((m) => m != null));
    };

    // --- Charts ----------------------------------------------------------
    // Marks over successive exams is change-over-time, so a line. Drawn as
    // inline SVG on a 0-100 scale, as the other trend reports do. Blue is the
    // learner, orange the class - a validated, colour-blind-safe pair, and the
    // class line is dashed so the two never rely on colour alone.
    readonly seriesStudent = '#2a78d6';
    readonly seriesClass = '#eb6834';
    readonly chartWidth = 760;
    private readonly padLeft = 34;
    // Enough room for the last exam's label, which is centred on the last point.
    private readonly padRight = 34;
    private readonly padTop = 12;
    private readonly plotHeight = 170;

    get chartBottom(): number { return this.padTop + this.plotHeight; }

    // The marks on show usually sit in a narrow band - a 0-100 axis would press
    // them into a flat line and hide the week-to-week movement the report is
    // for. The axis therefore covers the marks plus a margin, rounded to tens,
    // and the chart says so underneath whenever it does not start at zero.
    private axisRange = (): {min: number; max: number} => {
        let values: number[] = [];
        let take = (vals: (number | null)[]) => vals.forEach((v) => { if (v != null) values.push(v); });
        take(this.classAverages);
        let st = this.selectedStudent();
        if (st) take(st.marks);
        if (!values.length) return {min: 0, max: 100};

        let min = Math.max(0, Math.floor((Math.min(...values) - 5) / 10) * 10);
        let max = Math.min(100, Math.ceil((Math.max(...values) + 5) / 10) * 10);
        // Keep a sensible span, so two close marks are not magnified into a cliff.
        if (max - min < 20) {
            min = Math.max(0, min - (20 - (max - min)) / 2);
            max = Math.min(100, min + 20);
        }
        return {min, max};
    };

    axisMin = (): number => this.axisRange().min;

    // Five gridlines across whatever range the axis covers.
    gridLines = (): {y: number; label: string}[] => {
        let {min, max} = this.axisRange();
        let step = (max - min) / 4;
        return [0, 1, 2, 3, 4].map((i) => {
            let v = min + step * i;
            return {y: this.yFor(v), label: String(Math.round(v))};
        });
    };

    xFor = (idx: number): number => {
        let span = this.chartWidth - this.padLeft - this.padRight;
        let n = this.sittings.length;
        return n <= 1 ? this.padLeft + span / 2 : this.padLeft + (span * idx) / (n - 1);
    };

    yFor = (pct: number): number => {
        let {min, max} = this.axisRange();
        let span = max - min || 1;
        let clamped = Math.max(min, Math.min(max, pct));
        return this.padTop + this.plotHeight - ((clamped - min) / span) * this.plotHeight;
    };

    points = (values: (number | null)[]): {x: number; y: number; v: number; idx: number}[] =>
        values.map((v, idx) => (v == null ? null : {x: this.xFor(idx), y: this.yFor(v), v, idx}))
            .filter(Boolean) as {x: number; y: number; v: number; idx: number}[];

    // Separate runs of consecutive marks, so a missed exam leaves a gap in the
    // line rather than a straight line drawn through it.
    lineSegments = (values: (number | null)[]): string[] => {
        let segments: string[] = [];
        let current: string[] = [];
        values.forEach((v, idx) => {
            if (v == null) {
                if (current.length > 1) segments.push(current.join(' '));
                current = [];
            } else {
                current.push(`${this.xFor(idx)},${this.yFor(v)}`);
            }
        });
        if (current.length > 1) segments.push(current.join(' '));
        return segments;
    };

    classAverageValues = (): (number | null)[] => this.classAverages;

    // A tiny line for one subject's row in the breakdown table.
    sparkPoints = (values: (number | null)[]): string => {
        let n = values.length;
        let w = 74, h = 20;
        return values
            .map((v, idx) => (v == null ? null : `${n <= 1 ? w / 2 : (w * idx) / (n - 1)},${h - (Math.max(0, Math.min(100, v)) / 100) * h}`))
            .filter(Boolean)
            .join(' ');
    };

    private asShortDate = (d: string): string =>
        d ? new Date(d).toLocaleDateString('en-GB', {day: '2-digit', month: 'short'}) : '';


    // --- Printing ---------------------------------------------------------
    // Three printouts, built from the same pieces: the class list, one learner,
    // or every learner one after another. Each carries the chart as well as the
    // figures, because the shape of the run is the point of the report.
    isPrinting = false;

    printClassReport = () => this.withPrintAssets((assets) => {
        let title = `CLASS EXAM PROGRESS - ${this.examTypeName()} - ${this.filterName('class')}`.toUpperCase();
        this.emit(this.buildDoc(assets, title, this.classContent(assets)));
    });

    printStudentReport = () => {
        let st = this.selectedStudent();
        if (!st) { this.toastr.info('Select a learner first.'); return; }
        this.withPrintAssets((assets) => {
            let title = `LEARNER EXAM PROGRESS - ${st!.name}`.toUpperCase();
            this.emit(this.buildDoc(assets, title, this.studentContent(assets, st!)));
        });
    };

    // Bulk: every learner in the class, one per page in a single PDF - the way
    // report cards are produced.
    printAllStudentReports = () => this.withPrintAssets((assets) => {
        let title = `LEARNER EXAM PROGRESS - ${this.filterName('class')} - ${this.examTypeName()}`.toUpperCase();
        let content: any[] = [];
        this.rows.forEach((r, i) => {
            if (i > 0) content.push({text: '', pageBreak: 'before'});
            content = content.concat(this.studentContent(assets, r));
        });
        this.emit(this.buildDoc(assets, title, content));
        this.toastr.success(`${this.rows.length} learner report(s) generated.`);
    });

    private withPrintAssets = (build: (assets: any) => void) => {
        if (!this.rows.length) { this.toastr.info('Load the report first.'); return; }
        this.isPrinting = true;
        this.schoolSvc.get('/schooldetails').subscribe({
            next: (school) => {
                this.reportSvc.loadImageAsBase64('assets/img/shule-nova-logo-only.png').subscribe({
                    next: (blob) => {
                        const reader = new FileReader();
                        reader.onloadend = () => {
                            build({school: school[0], logo: reader.result as string});
                            this.isPrinting = false;
                        };
                        reader.readAsDataURL(blob);
                    },
                    error: () => { this.isPrinting = false; this.toastr.error('Error loading logo.'); }
                });
            },
            error: (err) => { this.isPrinting = false; this.toastr.error(err.error); }
        });
    };

    private emit = (doc: any) => {
        pdfMake.createPdf(doc).getBlob((pdfBlob) => {
            window.open(URL.createObjectURL(pdfBlob), '_blank');
        });
    };

    private buildDoc = (assets: any, title: string, content: any[]): any => {
        let author = (this.userSvc?.currentUser?.firstName || '') + ' ' + (this.userSvc?.currentUser?.lastName || '');
        return {
            pageSize: 'A4',
            pageOrientation: 'landscape',
            pageMargins: [15, 15, 15, 30],
            info: {title, author, subject: title},
            watermark: this.reportSvc.getWatermark('ShuleNova - ' + assets.school?.name),
            footer: this.reportSvc.getFooter('landscape'),
            images: {systemLogo: assets.logo, schoolLogo: assets.school?.logoAsBase64},
            styles: {tableHeader: this.reportSvc.getHEADER_STYLE()},
            content
        };
    };

    private reportTop = (assets: any, title: string, subtitle: string): any[] => [
        {...this.reportSvc.getDIVIDER('landscape')},
        this.reportSvc.getReportHeader(assets.school),
        {...this.reportSvc.getDIVIDER('landscape'), marginBottom: 1},
        this.reportSvc.getReportTitle(title),
        {text: subtitle, alignment: 'center', italics: true, fontSize: 8, color: '#555', marginBottom: 2},
        {...this.reportSvc.getDIVIDER('landscape'), marginBottom: 2}
    ];

    private printFoot = (): any[] => [
        {...this.reportSvc.getDIVIDER('landscape')},
        this.reportSvc.getPrintDetails(
            (this.userSvc?.currentUser?.firstName || '') + ' ' + (this.userSvc?.currentUser?.lastName || ''),
            new Date().toLocaleString('en-GB'))
    ];

    // The class list: the class average chart, then every learner's run.
    private classContent = (assets: any): any[] => {
        let head: any[] = [
            {text: '#', style: 'tableHeader', alignment: 'center'},
            {text: 'Adm No', style: 'tableHeader'},
            {text: 'Student', style: 'tableHeader'}
        ];
        this.sittings.forEach((s) => head.push({text: s.label, style: 'tableHeader', alignment: 'center'}));
        head.push(
            {text: 'Sat', style: 'tableHeader', alignment: 'center'},
            {text: 'Average', style: 'tableHeader', alignment: 'center'},
            {text: 'Change', style: 'tableHeader', alignment: 'center'}
        );

        let body: any[] = [head];
        this.rows.forEach((r, i) => {
            let line: any[] = [
                {text: i + 1, alignment: 'center', fontSize: 8},
                {text: r.upi, fontSize: 8},
                {text: r.name, fontSize: 8}
            ];
            r.marks.forEach((m, mi) => line.push(this.markCell(r.marks, mi)));
            line.push(
                {text: `${r.sat}/${this.sittings.length}`, alignment: 'center', fontSize: 8},
                {text: r.average == null ? '-' : r.average, alignment: 'center', bold: true, fontSize: 8},
                this.changeCell(r.change)
            );
            body.push(line);
        });

        let avgLine: any[] = [
            {text: '', fontSize: 8}, {text: '', fontSize: 8},
            {text: 'Class average', bold: true, fontSize: 8}
        ];
        this.classAverages.forEach((a) =>
            avgLine.push({text: a == null ? '-' : a, alignment: 'center', bold: true, fontSize: 8}));
        avgLine.push({text: '', fontSize: 8}, {text: '', fontSize: 8}, {text: '', fontSize: 8});
        body.push(avgLine);

        return [
            ...this.reportTop(assets,
                `CLASS EXAM PROGRESS - ${this.examTypeName()} - ${this.filterName('class')}`.toUpperCase(),
                `${this.filterName('year')} ${this.filterName('session')} - ${this.subjectName()} - marks are percentages; the small figure under a mark is the move from that learner's previous exam`),
            {
                svg: this.chartSvg([{values: this.classAverages, color: this.seriesClass, dashed: true, label: 'Class average'}]),
                width: 470, alignment: 'center', marginBottom: 4
            },
            {
                layout: this.reportSvc.getTableLayout(),
                table: {
                    headerRows: 1,
                    widths: ['auto', 'auto', '*', ...this.sittings.map(() => 'auto'), 'auto', 'auto', 'auto'],
                    body
                },
                fontSize: 8, marginBottom: 4
            },
            ...this.printFoot()
        ];
    };

    // One learner: their line against the class, then subject by subject.
    private studentContent = (assets: any, row: StudentRow): any[] => {
        let subjectRows = this.subjectRowsFor(row.studentId);

        let head: any[] = [{text: 'Subject', style: 'tableHeader'}];
        this.sittings.forEach((s) => head.push({text: s.label, style: 'tableHeader', alignment: 'center'}));
        head.push(
            {text: 'Average', style: 'tableHeader', alignment: 'center'},
            {text: 'Change', style: 'tableHeader', alignment: 'center'}
        );

        let body: any[] = [head];
        subjectRows.forEach((sr) => {
            let line: any[] = [{text: sr.name, fontSize: 8}];
            sr.marks.forEach((m, mi) => line.push(this.markCell(sr.marks, mi)));
            line.push(
                {text: sr.average == null ? '-' : sr.average, alignment: 'center', bold: true, fontSize: 8},
                this.changeCell(sr.change)
            );
            body.push(line);
        });

        // Overall row, so the subject lines tie back to the headline figures.
        let overall: any[] = [{text: 'Overall', bold: true, fontSize: 8}];
        row.marks.forEach((m, mi) => overall.push(this.markCell(row.marks, mi)));
        overall.push(
            {text: row.average == null ? '-' : row.average, alignment: 'center', bold: true, fontSize: 8},
            this.changeCell(row.change)
        );
        body.push(overall);

        return [
            ...this.reportTop(assets,
                `LEARNER EXAM PROGRESS - ${this.examTypeName()}`.toUpperCase(),
                `${this.filterName('class')} - ${this.filterName('year')} ${this.filterName('session')} - ${this.subjectName()}`),
            {
                columns: [
                    {text: `${row.upi ? row.upi + ' - ' : ''}${row.name}`, bold: true, fontSize: 10, color: '#002D62'},
                    {text: `Average ${row.average ?? '-'}   |   Sat ${row.sat}/${this.sittings.length}   |   Change ${row.change == null ? '-' : (row.change > 0 ? '+' + row.change : row.change)}`,
                     alignment: 'right', fontSize: 9}
                ],
                marginBottom: 3
            },
            {
                svg: this.chartSvg([
                    {values: row.marks, color: this.seriesStudent, dashed: false, label: row.name},
                    {values: this.classAverages, color: this.seriesClass, dashed: true, label: 'Class average'}
                ]),
                width: 470, alignment: 'center', marginBottom: 4
            },
            {
                layout: this.reportSvc.getTableLayout(),
                table: {
                    headerRows: 1,
                    widths: ['*', ...this.sittings.map(() => 'auto'), 'auto', 'auto'],
                    body
                },
                fontSize: 8, marginBottom: 4
            },
            ...this.printFoot()
        ];
    };

    // A mark with the move from the learner's previous exam underneath it.
    private markCell = (marks: (number | null)[], idx: number): any => {
        let m = marks[idx];
        let d = this.weekDelta(marks, idx);
        if (m == null) return {text: '-', alignment: 'center', fontSize: 8, color: '#999'};
        if (d == null) return {text: String(m), alignment: 'center', fontSize: 8};
        return {
            alignment: 'center',
            stack: [
                {text: String(m), fontSize: 8, alignment: 'center'},
                {text: `${d > 0 ? '+' : ''}${d}`, fontSize: 6, alignment: 'center', color: this.deltaColour(d)}
            ]
        };
    };

    private changeCell = (change: number | null): any => ({
        text: change == null ? '-' : (change > 0 ? '+' + change : String(change)),
        alignment: 'center', fontSize: 8, bold: true, color: this.deltaColour(change)
    });

    private deltaColour = (d: number | null): string =>
        d == null ? '#000' : d > 0.05 ? '#1a7f37' : d < -0.05 ? '#b42318' : '#555';

    // The on-screen line chart as an SVG pdfMake can place, sized to sit above
    // the table on one landscape page.
    private chartSvg = (series: {values: (number | null)[]; color: string; dashed: boolean; label: string}[]): string => {
        const w = 700, h = 200, left = 30, right = 30, top = 22, plotH = 130;
        const bottom = top + plotH;
        let all: number[] = [];
        series.forEach((s) => s.values.forEach((v) => { if (v != null) all.push(v); }));
        let min = all.length ? Math.max(0, Math.floor((Math.min(...all) - 5) / 10) * 10) : 0;
        let max = all.length ? Math.min(100, Math.ceil((Math.max(...all) + 5) / 10) * 10) : 100;
        if (max - min < 20) { min = Math.max(0, min - (20 - (max - min)) / 2); max = Math.min(100, min + 20); }

        let n = this.sittings.length;
        let x = (i: number) => n <= 1 ? left + (w - left - right) / 2 : left + ((w - left - right) * i) / (n - 1);
        let y = (v: number) => top + plotH - ((Math.max(min, Math.min(max, v)) - min) / (max - min || 1)) * plotH;

        let p: string[] = [];
        [0, 1, 2, 3, 4].forEach((i) => {
            let v = min + ((max - min) / 4) * i;
            p.push(`<line x1="${left}" y1="${y(v)}" x2="${w - right}" y2="${y(v)}" stroke="#e3e3e0" stroke-width="0.7"/>`);
            p.push(`<text x="${left - 4}" y="${y(v) + 3}" text-anchor="end" font-size="7" fill="#52514e">${Math.round(v)}</text>`);
        });
        this.sittings.forEach((s, i) =>
            p.push(`<text x="${x(i)}" y="${bottom + 12}" text-anchor="middle" font-size="7" fill="#52514e">${this.esc(s.label)}</text>`));

        series.forEach((s, si) => {
            this.lineRuns(s.values).forEach((run) => {
                let pts = run.map((idx) => `${x(idx)},${y(s.values[idx]!)}`).join(' ');
                p.push(`<polyline points="${pts}" fill="none" stroke="${s.color}" stroke-width="2"${s.dashed ? ' stroke-dasharray="5 3"' : ''}/>`);
            });
            s.values.forEach((v, idx) => {
                if (v == null) return;
                p.push(`<circle cx="${x(idx)}" cy="${y(v)}" r="3" fill="${s.color}" stroke="#ffffff" stroke-width="1.5"/>`);
                // Only the first series carries its values: where a learner runs
                // close to the class line, two labels on the same spot collide.
                if (si === 0)
                    p.push(`<text x="${x(idx)}" y="${y(v) - 6}" text-anchor="middle" font-size="7" fill="#0b0b0b">${v}</text>`);
            });
        });

        // Legend. Identity never rests on colour alone, so the class line is
        // dashed here as well. Long names are clipped so entries cannot run
        // into each other.
        let lx = left;
        series.forEach((s) => {
            let label = s.label.length > 24 ? s.label.slice(0, 23) + '.' : s.label;
            p.push(`<line x1="${lx}" y1="8" x2="${lx + 18}" y2="8" stroke="${s.color}" stroke-width="2"${s.dashed ? ' stroke-dasharray="5 3"' : ''}/>`);
            p.push(`<text x="${lx + 22}" y="11" font-size="7" fill="#0b0b0b">${this.esc(label)}</text>`);
            lx += 44 + label.length * 4.2;
        });

        return `<svg xmlns="http://www.w3.org/2000/svg" width="${w}" height="${h}" viewBox="0 0 ${w} ${h}">${p.join('')}</svg>`;
    };

    // Indexes of consecutive marks, so a missed exam breaks the line.
    private lineRuns = (values: (number | null)[]): number[][] => {
        let runs: number[][] = [], current: number[] = [];
        values.forEach((v, idx) => {
            if (v == null) { if (current.length > 1) runs.push(current); current = []; }
            else current.push(idx);
        });
        if (current.length > 1) runs.push(current);
        return runs;
    };

    private esc = (s: string): string =>
        (s || '').replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}
