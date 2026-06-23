import {Component, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {GradesService} from '@/academics/services/grades.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {ExamService} from '@/cbe/exams/services/exam.service';
import {ExamTypeService} from '@/cbe/exams/services/exam-type.service';
import {SchoolExamService} from '@/cbe/exams/services/school-exam.service';
import {ExamResultService} from '@/cbe/exams/services/exam-result.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
import {AuthService} from '@/core/services/auth.service';
import {ReportsService} from '@/reports/services/reports.service';
import {Status} from '@/core/enums/status';

import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
(pdfMake as any).vfs = pdfFonts;

@Component({
    selector: 'app-broadsheet',
    templateUrl: './broadsheet.component.html',
    styleUrl: './broadsheet.component.scss'
})
export class BroadsheetComponent implements OnInit {
    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    examTypes: any[] = [];
    grades: any[] = [];
    learningLevels: any[] = [];
    gradingCategory: string = '4-Point';
    gradingCategories: string[] = ['4-Point', '8-Point'];
    selectedGradingCategory: string = '4-Point';
    allGrades: any[] = [];
    rankingMethod: string = 'mean_marks';
    // 'subjects_done' (default) divides totals/means by subjects with marks;
    // 'subjects_expected' divides by all subjects the student is allocated to.
    meanBasis: string = 'subjects_done';
    // Raw Grading-module settings, used to resolve the exam-results grading
    // category (4-Point/8-Point) per education level.
    gradingSettings: any[] = [];

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterSchoolClassId: any = null;
    filterExamTypeId: any = null;
    filterSchoolExamId: any = null;
    schoolExams: any[] = [];

    subjects: string[] = [];
    broadsheetRows: {
        rank: number;
        upi: string;
        fullName: string;
        scores: { [subject: string]: { score: number | null; grade: string; points: number } };
        total: number;
        totalOutOf: number;
        totalPoints: number;
        totalPointsOutOf: number;
        average: number;
        meanPoints: number;
        averageGrade: string;
    }[] = [];

    loaded: boolean = false;
    isLoading: boolean = false;
    averageMethod: string = 'students_with_scores';
    page: number = 1;
    pageSize: number = 20;

    constructor(
        private toastr: ToastrService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private schoolClassesSvc: SchoolClassesService,
        private learningLevelSvc: LearningLevelsService,
        private gradesSvc: GradesService,
        private globalSettingSvc: GlobalSettingService,
        private examSvc: ExamService,
        private examTypeSvc: ExamTypeService,
        private schoolExamSvc: SchoolExamService,
        private examResultSvc: ExamResultService,
        private studentClassSvc: StudentClassService,
        private studentSubjectsSvc: StudentSubjectsService,
        private schoolSvc: SchoolDetailsService,
        private userSvc: AuthService,
        private reportSvc: ReportsService
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.academicYearSvc.get('/academicYears'),
            this.examTypeSvc.get('/examTypes'),
            this.gradesSvc.get('/grades'),
            this.globalSettingSvc.getByModule('Grading'),
            this.globalSettingSvc.getByKey('General', 'AverageCalculation')
        ]).subscribe({
            next: ([curricula, academicYears, examTypes, allGrades, gradingSettings, avgSetting]) => {
                this.gradingSettings = (gradingSettings as any[]) || [];
                this.averageMethod = (avgSetting as any)?.settingValue || 'students_with_scores';
                this.meanBasis = this.settingVal('MeanBasis') || 'subjects_done';
                this.rankingMethod = this.settingVal('RankingMethod') || 'mean_marks';
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort((a, b) => b.rank - a.rank);
                this.examTypes = examTypes.sort((a, b) => a.rank - b.rank);
                this.allGrades = allGrades;
                // Until a class is loaded, show the global default; loadBroadsheet
                // re-resolves it against the selected class's education level.
                this.applyExamGrading(null);
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    private settingVal = (key: string): string =>
        this.gradingSettings.find((s) => s.settingKey === key)?.settingValue || '';

    // Effective exam-results grading category for an education level: the level's
    // own override if set, else the global default, else 4-Point.
    private examGradingCategoryFor = (edLevelId: any): string => {
        let globalVal = this.settingVal('ExamResults') || '4-Point';
        if (!edLevelId) return globalVal;
        return this.settingVal(`ExamResults:${edLevelId}`) || globalVal;
    };

    // Ranking method for an education level: the level's override if set, else
    // the global default, else mean_marks.
    private rankingMethodFor = (edLevelId: any): string => {
        let globalVal = this.settingVal('RankingMethod') || 'mean_marks';
        if (!edLevelId) return globalVal;
        return this.settingVal(`RankingMethod:${edLevelId}`) || globalVal;
    };

    private applyExamGrading = (edLevelId: any) => {
        this.gradingCategory = this.examGradingCategoryFor(edLevelId);
        this.selectedGradingCategory = this.gradingCategory;
        this.grades = this.allGrades.filter((g) => g.category === this.gradingCategory).sort((a, b) => a.rank - b.rank);
    };

    // Combined broadsheet column setting: 'MG' omits the Points sub-column,
    // anything else (default) includes it (M/G/P).
    get combinedIncludesPoints(): boolean {
        return this.settingVal('BroadsheetCombinedColumns') !== 'MG';
    }
    get combinedColsLabel(): string {
        return this.combinedIncludesPoints ? 'M/G/P' : 'M/G';
    }

    onGradingCategoryChange = () => {
        this.grades = this.allGrades.filter(g => g.category === this.selectedGradingCategory).sort((a, b) => a.rank - b.rank);
    };

    // Any filter change invalidates the loaded broadsheet - hide and clear it
    // so stale data isn't shown until the user clicks Load again.
    onFilterChange = () => {
        this.loaded = false;
        this.broadsheetRows = [];
        this.page = 1;
    };

    // Session drives the School Exam list (a school exam carries the exam type
    // the broadsheet query still filters by).
    onSessionChange = () => {
        this.onFilterChange();
        this.schoolExams = [];
        this.filterSchoolExamId = this.filterExamTypeId = null;
        if (!this.filterSessionId || !this.filterCurriculumId || !this.filterAcademicYearId) return;
        this.schoolExamSvc
            .get(`/schoolExams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${this.filterSessionId}`)
            .subscribe({
                next: (items) => { this.schoolExams = items; },
                error: (err) => this.toastr.error(err.error)
            });
    };

    onSchoolExamChange = () => {
        this.onFilterChange();
        let se = this.schoolExams.find((s) => s.id == this.filterSchoolExamId);
        this.filterExamTypeId = se?.examTypeId ?? se?.examType?.id ?? null;
    };

    onCurriculumChange = () => {
        this.sessions = this.schoolClasses = this.schoolExams = [];
        this.filterSchoolExamId = null;
        this.broadsheetRows = [];
        this.page = 1;
        this.filterAcademicYearId = this.filterSessionId = this.filterSchoolClassId = this.filterExamTypeId = null;
        this.loaded = false;
        if (!this.filterCurriculumId) return;
        this.learningLevelSvc.getLearningLevelsByCurriculum(this.filterCurriculumId).subscribe({
            next: (levels) => { this.learningLevels = levels.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onAcademicYearChange = () => {
        this.sessions = this.schoolClasses = this.schoolExams = [];
        this.filterSessionId = this.filterSchoolClassId = this.filterExamTypeId = this.filterSchoolExamId = null;
        this.loaded = false;
        this.broadsheetRows = [];
        this.page = 1;
        if (!this.filterAcademicYearId || !this.filterCurriculumId) return;
        forkJoin([
            this.sessionsSvc.get(`/sessions/byCurriculumYearId?curriculumId=${this.filterCurriculumId}&academicYearId=${this.filterAcademicYearId}`),
            this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${this.filterAcademicYearId}`)
        ]).subscribe({
            next: ([sessions, schoolClasses]) => {
                this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                let currLLIds = this.learningLevels.map((ll) => +ll.id);
                this.schoolClasses = schoolClasses.filter((sc) => currLLIds.includes(+sc.learningLevelId)).sort((a, b) => (a.rank || 0) - (b.rank || 0));
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    getGradeForPercent = (percent: number): string => {
        let grade = this.grades.find((g) => percent >= g.minScore && percent <= g.maxScore);
        return grade ? grade.abbr : '';
    };

    getGradeForPoints = (points: number): string => {
        // Find exact match first
        let grade = this.grades.find((g) => g.points == points);
        if (grade) return grade.abbr;
        // Find closest grade by points
        if (this.grades.length === 0) return '';
        let closest = this.grades.reduce((prev, curr) =>
            Math.abs(curr.points - points) < Math.abs(prev.points - points) ? curr : prev
        );
        return closest ? closest.abbr : '';
    };

    getPointsForPercent = (percent: number): number => {
        let grade = this.grades.find((g) => percent >= g.minScore && percent <= g.maxScore);
        return grade ? grade.points : 0;
    };

    loadBroadsheet = () => {
        if (!this.filterSessionId || !this.filterSchoolClassId || !this.filterExamTypeId) {
            this.toastr.info('Please select Session, Class, and Exam Type.');
            return;
        }

        this.isLoading = true;
        this.loaded = false;

        // Resolve the grading category for this class's education level so the
        // broadsheet uses the right 4-Point/8-Point scale.
        let selClass = this.schoolClasses.find((sc) => +sc.id === +this.filterSchoolClassId);
        let edLevelId = selClass?.learningLevel?.educationLevelId
            ?? this.learningLevels.find((ll) => +ll.id === +(selClass?.learningLevelId))?.educationLevelId;
        this.applyExamGrading(edLevelId);
        this.rankingMethod = this.rankingMethodFor(edLevelId);

        let url = `/exams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${this.filterSessionId}&schoolClassId=${this.filterSchoolClassId}&examTypeId=${this.filterExamTypeId}`;

        forkJoin([
            this.examSvc.get(url),
            this.studentClassSvc.getBySchoolClassId(this.filterSchoolClassId, Status.Active),
            this.studentSubjectsSvc.get(`/studentSubjects/allocationsBySchoolClassId/${this.filterSchoolClassId}`)
        ]).subscribe({
            next: ([exams, studentClasses, allocations]) => {
                if (exams.length === 0) {
                    this.isLoading = false;
                    this.toastr.info('No exams found.');
                    return;
                }

                let subjectMap = new Map<string, number>();
                // Map a subject's display key (abbr/name) to its id and exam mark
                // so the "expected subjects" basis can be evaluated against the
                // student's allocation, which is keyed by subjectId.
                let subjectIdByKey = new Map<string, number>();
                let examMarkByKey = new Map<string, number>();
                exams.forEach((e) => {
                    let key = e.subject?.abbr || e.subject?.name;
                    if (!key) return;
                    if (!subjectMap.has(key)) subjectMap.set(key, e.subject?.rank || 0);
                    if (!subjectIdByKey.has(key)) subjectIdByKey.set(key, +e.subjectId);
                    if (!examMarkByKey.has(key)) examMarkByKey.set(key, e.examMark || 0);
                });
                this.subjects = [...subjectMap.entries()].sort((a, b) => a[1] - b[1]).map((e) => e[0]);

                let students = studentClasses.map((sc) => sc.student).filter(Boolean);

                // Build per-student allocated subjectId sets. Allocation rows carry
                // studentClassId, so bridge to studentId via the student-class list.
                let studentIdByClassId = new Map<number, number>();
                studentClasses.forEach((sc) => { if (sc.student) studentIdByClassId.set(+sc.id, +sc.student.id); });
                let allocatedByStudentId = new Map<number, Set<number>>();
                (allocations || []).forEach((a: any) => {
                    let sid = studentIdByClassId.get(+a.studentClassId);
                    if (!sid) return;
                    if (!allocatedByStudentId.has(sid)) allocatedByStudentId.set(sid, new Set<number>());
                    allocatedByStudentId.get(sid)!.add(+a.subjectId);
                });
                let useExpected = this.meanBasis === 'subjects_expected';

                let resultRequests = exams.map((exam) =>
                    this.examResultSvc.get(`/examResults/byExamId/${exam.id}`)
                );

                forkJoin(resultRequests).subscribe({
                    next: (allResults) => {
                        let studentScores: any = {};
                        exams.forEach((exam, idx) => {
                            allResults[idx].forEach((r) => {
                                if (!studentScores[r.studentId]) studentScores[r.studentId] = {};
                                studentScores[r.studentId][exam.subject?.abbr || exam.subject?.name] = {
                                    score: r.score, examMark: exam.examMark
                                };
                            });
                        });

                        let examMark = exams[0]?.examMark || 100;
                        let maxGradePoints = this.grades.length > 0
                            ? Math.max(...this.grades.map((g: any) => +g.points || 0))
                            : 0;
                        let rows = students.map((student) => {
                            let scores: any = {};
                            let total = 0;
                            let totalOutOf = 0;
                            let totalPoints = 0;
                            let count = 0;
                            this.subjects.forEach((subj) => {
                                let entry = studentScores[+student.id]?.[subj];
                                if (entry && entry.score != null) {
                                    let pct = entry.examMark > 0 ? (entry.score / entry.examMark) * 100 : 0;
                                    let pts = this.getPointsForPercent(pct);
                                    scores[subj] = {score: Math.round(entry.score * 10) / 10, grade: this.getGradeForPercent(pct), points: pts};
                                    total += entry.score;
                                    totalOutOf += entry.examMark || 0;
                                    totalPoints += pts;
                                    count++;
                                } else {
                                    scores[subj] = {score: null, grade: '', points: 0};
                                }
                            });
                            // Under the "expected subjects" basis, divide by every
                            // subject the student is allocated to (that is examined),
                            // not just the ones with marks - missing subjects count
                            // as zero. Falls back to recorded count when the student
                            // has no allocation data.
                            // Keep students with no marks as-is (don't apply the
                            // expected denominator) so they don't read as 0/900.
                            let allocated = allocatedByStudentId.get(+student.id);
                            let expectedSubjects = (useExpected && count > 0 && allocated && allocated.size > 0)
                                ? this.subjects.filter((s) => allocated!.has(subjectIdByKey.get(s) ?? -1))
                                : null;
                            let denomCount = expectedSubjects ? expectedSubjects.length : count;
                            let outOf = expectedSubjects
                                ? expectedSubjects.reduce((sum, s) => sum + (examMarkByKey.get(s) || 0), 0)
                                : totalOutOf;

                            let average = denomCount > 0 ? Math.round((total / denomCount) * 10) / 10 : 0;
                            let meanPoints = denomCount > 0 ? Math.round((totalPoints / denomCount) * 10) / 10 : 0;
                            let meanGrade = this.getGradeForPoints(meanPoints);
                            let totalPointsOutOf = denomCount * maxGradePoints;
                            return {
                                rank: 0, upi: student.upi || '', fullName: student.fullName || '',
                                scores, total: Math.round(total * 10) / 10, totalOutOf: outOf, totalPoints, totalPointsOutOf, average, meanPoints, averageGrade: meanGrade
                            };
                        });

                        // Rank by setting: mean_marks (average %) or mean_points (total points)
                        let rankField = this.rankingMethod === 'mean_marks' ? 'average' : 'totalPoints';
                        // Use integer-rounded values for comparison to avoid FP precision issues
                        // and ensure equal displayed values get the same rank
                        let getRankValue = (r: any) => Math.round((r[rankField] || 0) * 10);
                        rows.sort((a, b) => getRankValue(b) - getRankValue(a));
                        let currentRank = 1;
                        rows.forEach((r, i) => {
                            if (i > 0 && getRankValue(r) === getRankValue(rows[i - 1])) {
                                r.rank = rows[i - 1].rank;
                            } else {
                                r.rank = currentRank;
                            }
                            currentRank = i + 2;
                        });
                        this.broadsheetRows = rows;
                        this.loaded = true;
                        this.isLoading = false;
                    },
                    error: (err) => { this.isLoading = false; this.toastr.error(err.error); }
                });
            },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error); }
        });
    };

    getFilterName = (type: string): string => {
        if (type === 'year') return this.academicYears.find((y) => y.id == this.filterAcademicYearId)?.name || '';
        if (type === 'session') return this.sessions.find((s) => s.id == this.filterSessionId)?.sessionName || '';
        if (type === 'class') return this.schoolClasses.find((sc) => sc.id == this.filterSchoolClassId)?.name || '';
        if (type === 'examType') return this.examTypes.find((et) => et.id == this.filterExamTypeId)?.name || '';
        return '';
    };

    printBroadsheet = (mode: string = 'marks') => {
        this.schoolSvc.get('/schooldetails').subscribe({
            next: (school) => {
                this.reportSvc.loadImageAsBase64('assets/img/shule-nova-logo-only.png').subscribe({
                    next: (blob) => {
                        const reader = new FileReader();
                        reader.onloadend = () => {
                            const base64data: string = reader.result as string;
                            // 'mgp' = combined report: each subject shows Marks,
                            // Grade and (optionally) Points under single-letter
                            // M G [P] sub-columns. Points is dropped when the
                            // BroadsheetCombinedColumns setting is 'MG'.
                            let mgp = mode === 'mgp';
                            let includePoints = this.combinedIncludesPoints;
                            let modeLabel = mgp ? this.combinedColsLabel : mode === 'points' ? 'POINTS' : mode === 'grades' ? 'GRADES' : 'MARKS';
                            let reportTitle = `BROADSHEET (${modeLabel}) - ${this.getFilterName('year')} ${this.getFilterName('session')} - ${this.getFilterName('examType')} - ${this.getFilterName('class')}`.toUpperCase();

                            let summaryTitles = ['Total M', 'Total P', 'Mean M', 'Mean P', 'Mean G', 'Rank'];
                            let headers: any[] = [
                                {text: '#', style: 'tableHeader', ...(mgp ? {rowSpan: 2} : {})},
                                {text: 'Adm#', style: 'tableHeader', ...(mgp ? {rowSpan: 2} : {})},
                                {text: 'Student Name', style: 'tableHeader', ...(mgp ? {rowSpan: 2} : {})}
                            ];
                            let widths: any[] = ['auto', 'auto', '*'];
                            let body: any[] = [];

                            if (mgp) {
                                // Row 1: subject spanning the sub-columns. Row 2: M | G [| P].
                                let span = includePoints ? 3 : 2;
                                let subHeaders: any[] = [{}, {}, {}];
                                this.subjects.forEach((s) => {
                                    headers.push({text: s, style: 'tableHeader', colSpan: span, alignment: 'center'});
                                    for (let i = 1; i < span; i++) headers.push({});
                                    subHeaders.push(
                                        {text: 'M', style: 'tableHeader', alignment: 'center'},
                                        {text: 'G', style: 'tableHeader', alignment: 'center'}
                                    );
                                    if (includePoints) subHeaders.push({text: 'P', style: 'tableHeader', alignment: 'center'});
                                    for (let i = 0; i < span; i++) widths.push('auto');
                                });
                                summaryTitles.forEach((t) => { headers.push({text: t, style: 'tableHeader', rowSpan: 2}); subHeaders.push({}); widths.push('auto'); });
                                body.push(headers, subHeaders);
                            } else {
                                this.subjects.forEach((s) => { headers.push({text: s, style: 'tableHeader'}); widths.push('auto'); });
                                summaryTitles.forEach((t) => { headers.push({text: t, style: 'tableHeader'}); widths.push('auto'); });
                                body.push(headers);
                            }
                            this.broadsheetRows.forEach((row, idx) => {
                                let rowData: any[] = [
                                    {text: idx + 1, noWrap: true},
                                    {text: row.upi, noWrap: true},
                                    {text: row.fullName, noWrap: true}
                                ];
                                this.subjects.forEach((subj) => {
                                    let entry = row.scores[subj];
                                    if (mgp) {
                                        if (entry.score != null) {
                                            rowData.push(
                                                {text: `${entry.score}`, alignment: 'center', noWrap: true},
                                                {text: entry.grade, alignment: 'center', noWrap: true}
                                            );
                                            if (includePoints) rowData.push({text: `${entry.points}`, alignment: 'center', noWrap: true});
                                        } else {
                                            rowData.push({text: '-', alignment: 'center'}, {text: '-', alignment: 'center'});
                                            if (includePoints) rowData.push({text: '-', alignment: 'center'});
                                        }
                                        return;
                                    }
                                    let cellText = '-';
                                    if (entry.score != null) {
                                        if (mode === 'points') {
                                            cellText = `${entry.points}`;
                                        } else if (mode === 'grades') {
                                            cellText = entry.grade;
                                        } else {
                                            cellText = `${entry.score}`;
                                        }
                                    }
                                    rowData.push({text: cellText, alignment: 'center', noWrap: true});
                                });
                                rowData.push({text: `${row.total}/${row.totalOutOf}`, noWrap: true, bold: true});
                                rowData.push({text: `${row.totalPoints}/${row.totalPointsOutOf}`, noWrap: true, bold: true});
                                rowData.push({text: row.average + '%', noWrap: true});
                                rowData.push({text: row.meanPoints, noWrap: true});
                                rowData.push({text: row.averageGrade, noWrap: true, bold: true});
                                rowData.push({text: row.rank, noWrap: true, bold: true});
                                body.push(rowData);
                            });

                            // Summary rows
                            let avgRow: any[] = [
                                {text: '', colSpan: 3, bold: true}, {}, {},
                            ];
                            let highRow: any[] = [
                                {text: '', colSpan: 3, bold: true}, {}, {},
                            ];
                            let lowRow: any[] = [
                                {text: '', colSpan: 3, bold: true}, {}, {},
                            ];
                            // Set labels
                            avgRow[0] = {text: 'Class Average', colSpan: 3, bold: true, alignment: 'right', fillColor: '#fff3cd'};
                            highRow[0] = {text: 'Highest', colSpan: 3, bold: true, alignment: 'right', fillColor: '#d1e7dd'};
                            lowRow[0] = {text: 'Lowest', colSpan: 3, bold: true, alignment: 'right', fillColor: '#f8d7da'};

                            this.subjects.forEach((subj) => {
                                if (mgp) {
                                    avgRow.push(
                                        {text: this.getSubjectAvg(subj), alignment: 'center', bold: true, fillColor: '#fff3cd'},
                                        {text: '', fillColor: '#fff3cd'}
                                    );
                                    highRow.push(
                                        {text: this.getSubjectMax(subj), alignment: 'center', bold: true, fillColor: '#d1e7dd'},
                                        {text: '', fillColor: '#d1e7dd'}
                                    );
                                    lowRow.push(
                                        {text: this.getSubjectMin(subj), alignment: 'center', bold: true, fillColor: '#f8d7da'},
                                        {text: '', fillColor: '#f8d7da'}
                                    );
                                    if (includePoints) {
                                        avgRow.push({text: this.getSubjectPointsAvg(subj), alignment: 'center', bold: true, fillColor: '#fff3cd'});
                                        highRow.push({text: this.getSubjectPointsMax(subj), alignment: 'center', bold: true, fillColor: '#d1e7dd'});
                                        lowRow.push({text: this.getSubjectPointsMin(subj), alignment: 'center', bold: true, fillColor: '#f8d7da'});
                                    }
                                } else if (mode === 'points') {
                                    avgRow.push({text: this.getSubjectPointsAvg(subj), alignment: 'center', bold: true, fillColor: '#fff3cd'});
                                    highRow.push({text: this.getSubjectPointsMax(subj), alignment: 'center', bold: true, fillColor: '#d1e7dd'});
                                    lowRow.push({text: this.getSubjectPointsMin(subj), alignment: 'center', bold: true, fillColor: '#f8d7da'});
                                } else if (mode === 'grades') {
                                    avgRow.push({text: '', alignment: 'center', bold: true, fillColor: '#fff3cd'});
                                    highRow.push({text: '', alignment: 'center', bold: true, fillColor: '#d1e7dd'});
                                    lowRow.push({text: '', alignment: 'center', bold: true, fillColor: '#f8d7da'});
                                } else {
                                    avgRow.push({text: this.getSubjectAvg(subj), alignment: 'center', bold: true, fillColor: '#fff3cd'});
                                    highRow.push({text: this.getSubjectMax(subj), alignment: 'center', bold: true, fillColor: '#d1e7dd'});
                                    lowRow.push({text: this.getSubjectMin(subj), alignment: 'center', bold: true, fillColor: '#f8d7da'});
                                }
                            });

                            // Total M, Total P, Mean M, Mean P, Mean G, Rank
                            avgRow.push(
                                {text: this.getClassTotalMarksAvgOutOf(), alignment: 'center', bold: true, fillColor: '#fff3cd'},
                                {text: this.getClassTotalPointsAvgOutOf(), alignment: 'center', bold: true, fillColor: '#fff3cd'},
                                {text: this.getClassAvg() + '%', alignment: 'center', bold: true, fillColor: '#fff3cd'},
                                {text: this.getClassMeanPoints(), alignment: 'center', bold: true, fillColor: '#fff3cd'},
                                {text: this.getClassAvgGrade(), alignment: 'center', bold: true, fillColor: '#fff3cd'},
                                {text: '', fillColor: '#fff3cd'}
                            );
                            highRow.push(
                                {text: this.getMaxTotal(), alignment: 'center', bold: true, fillColor: '#d1e7dd'},
                                {text: this.getMaxTotalPoints(), alignment: 'center', bold: true, fillColor: '#d1e7dd'},
                                {text: this.getMax('average') + '%', alignment: 'center', bold: true, fillColor: '#d1e7dd'},
                                {text: this.getMax('meanPoints'), alignment: 'center', bold: true, fillColor: '#d1e7dd'},
                                {text: '', fillColor: '#d1e7dd'},
                                {text: '', fillColor: '#d1e7dd'}
                            );
                            lowRow.push(
                                {text: this.getMinTotal(), alignment: 'center', bold: true, fillColor: '#f8d7da'},
                                {text: this.getMinTotalPoints(), alignment: 'center', bold: true, fillColor: '#f8d7da'},
                                {text: this.getMin('average') + '%', alignment: 'center', bold: true, fillColor: '#f8d7da'},
                                {text: this.getMin('meanPoints'), alignment: 'center', bold: true, fillColor: '#f8d7da'},
                                {text: '', fillColor: '#f8d7da'},
                                {text: '', fillColor: '#f8d7da'}
                            );

                            body.push(avgRow, highRow, lowRow);

                            const docDefinition: any = {
                                pageOrientation: 'landscape',
                                pageMargins: [15, 15, 15, 35],
                                pageSize: 'A4',
                                info: {
                                    title: reportTitle,
                                    author: (this.userSvc?.currentUser?.firstName || '') + ' ' + (this.userSvc?.currentUser?.lastName || ''),
                                    subject: reportTitle
                                },
                                watermark: this.reportSvc.getWatermark('ShuleNova - ' + school[0]?.name),
                                footer: this.reportSvc.getFooter('landscape'),
                                images: { systemLogo: base64data, schoolLogo: school[0]?.logoAsBase64 },
                                styles: { tableHeader: this.reportSvc.getHEADER_STYLE() },
                                content: [
                                    {...this.reportSvc.getDIVIDER('landscape')},
                                    this.reportSvc.getReportHeader(school[0]),
                                    {...this.reportSvc.getDIVIDER('landscape'), marginBottom: 1},
                                    this.reportSvc.getReportTitle(reportTitle),
                                    {...this.reportSvc.getDIVIDER('landscape'), marginBottom: 1},
                                    {
                                        // Combined mode packs 3x the columns, so use a
                                        // tighter layout and smaller font to help it fit.
                                        layout: mgp ? {
                                            hLineWidth: () => 0.4, vLineWidth: () => 0.4,
                                            hLineColor: () => '#aaa', vLineColor: () => '#aaa',
                                            paddingLeft: () => 2, paddingRight: () => 2,
                                            paddingTop: () => 1, paddingBottom: () => 1
                                        } : this.reportSvc.getTableLayout(),
                                        table: { headerRows: mgp ? 2 : 1, widths, body },
                                        marginBottom: 2, color: '#002D62', fontSize: mgp ? 7 : 8
                                    },
                                    {...this.reportSvc.getDIVIDER('landscape')},
                                    this.reportSvc.getPrintDetails(
                                        (this.userSvc?.currentUser?.firstName || '') + ' ' + (this.userSvc?.currentUser?.lastName || ''),
                                        new Date().toLocaleString('en-GB')
                                    )
                                ]
                            };

                            pdfMake.createPdf(docDefinition).getBlob((pdfBlob) => {
                                const pdfUrl = URL.createObjectURL(pdfBlob);
                                window.open(pdfUrl, '_blank');
                            });
                        };
                        reader.readAsDataURL(blob);
                    },
                    error: () => this.toastr.error('Error loading logo.')
                });
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    pageChanged = (page: number) => { this.page = page; };
    pageSizeChanged = (pageSize: number) => { this.pageSize = pageSize; this.page = 1; };

    // The class summary (average/highest/lowest) is rendered only on the last
    // page so it reads as an end-of-report total, not a per-page footer that
    // could hide the fact more pages follow.
    get isLastPage(): boolean {
        if (this.broadsheetRows.length === 0) return false;
        return this.page >= Math.ceil(this.broadsheetRows.length / this.pageSize);
    }

    getSubjectAvg = (subj: string): string => {
        let scored = this.broadsheetRows.filter((r) => r.scores[subj]?.score != null);
        if (scored.length === 0) return '-';
        let total = scored.reduce((sum, r) => sum + (r.scores[subj].score || 0), 0);
        let divisor = this.averageMethod === 'all_allocated_students'
            ? this.broadsheetRows.length : scored.length;
        return (Math.round((total / divisor) * 10) / 10).toString();
    };

    getSubjectMax = (subj: string): string => {
        let scored = this.broadsheetRows.filter((r) => r.scores[subj]?.score != null);
        if (scored.length === 0) return '-';
        return Math.max(...scored.map((r) => r.scores[subj].score || 0)).toString();
    };

    getSubjectMin = (subj: string): string => {
        let scored = this.broadsheetRows.filter((r) => r.scores[subj]?.score != null);
        if (scored.length === 0) return '-';
        return Math.min(...scored.map((r) => r.scores[subj].score || 0)).toString();
    };

    getSubjectPointsAvg = (subj: string): string => {
        let scored = this.broadsheetRows.filter((r) => r.scores[subj]?.score != null);
        if (scored.length === 0) return '-';
        let total = scored.reduce((sum, r) => sum + (r.scores[subj].points || 0), 0);
        let divisor = this.averageMethod === 'all_allocated_students'
            ? this.broadsheetRows.length : scored.length;
        return (Math.round((total / divisor) * 10) / 10).toString();
    };

    getSubjectPointsMax = (subj: string): string => {
        let scored = this.broadsheetRows.filter((r) => r.scores[subj]?.score != null);
        if (scored.length === 0) return '-';
        return Math.max(...scored.map((r) => r.scores[subj].points || 0)).toString();
    };

    getSubjectPointsMin = (subj: string): string => {
        let scored = this.broadsheetRows.filter((r) => r.scores[subj]?.score != null);
        if (scored.length === 0) return '-';
        return Math.min(...scored.map((r) => r.scores[subj].points || 0)).toString();
    };

    getClassTotalMarksAvg = (): string => {
        if (this.broadsheetRows.length === 0) return '-';
        let total = this.broadsheetRows.reduce((sum, r) => sum + r.total, 0);
        return (Math.round((total / this.broadsheetRows.length) * 10) / 10).toString();
    };

    getClassTotalMarksAvgOutOf = (): string => {
        if (this.broadsheetRows.length === 0) return '';
        let avg = this.broadsheetRows.reduce((sum, r) => sum + r.total, 0) / this.broadsheetRows.length;
        let avgOutOf = this.broadsheetRows.reduce((sum, r) => sum + (r.totalOutOf || 0), 0) / this.broadsheetRows.length;
        if (avgOutOf <= 0) return (Math.round(avg * 10) / 10).toString();
        return `${Math.round(avg * 10) / 10}/${Math.round(avgOutOf)}`;
    };

    getClassTotalPointsAvg = (): string => {
        if (this.broadsheetRows.length === 0) return '-';
        let total = this.broadsheetRows.reduce((sum, r) => sum + r.totalPoints, 0);
        return (Math.round((total / this.broadsheetRows.length) * 10) / 10).toString();
    };

    getClassTotalPointsAvgOutOf = (): string => {
        if (this.broadsheetRows.length === 0) return '';
        let avg = this.broadsheetRows.reduce((sum, r) => sum + r.totalPoints, 0) / this.broadsheetRows.length;
        let avgOutOf = this.broadsheetRows.reduce((sum, r) => sum + (r.totalPointsOutOf || 0), 0) / this.broadsheetRows.length;
        if (avgOutOf <= 0) return (Math.round(avg * 10) / 10).toString();
        return `${Math.round(avg * 10) / 10}/${Math.round(avgOutOf)}`;
    };

    getClassAvg = (): string => {
        if (this.broadsheetRows.length === 0) return '-';
        let total = this.broadsheetRows.reduce((sum, r) => sum + r.average, 0);
        return (Math.round((total / this.broadsheetRows.length) * 10) / 10).toString();
    };

    getClassMeanPoints = (): string => {
        if (this.broadsheetRows.length === 0) return '-';
        let total = this.broadsheetRows.reduce((sum, r) => sum + r.meanPoints, 0);
        return (Math.round((total / this.broadsheetRows.length) * 10) / 10).toString();
    };

    getClassAvgGrade = (): string => {
        if (this.broadsheetRows.length === 0) return '';
        let totalMeanPoints = this.broadsheetRows.reduce((sum, r) => sum + r.meanPoints, 0);
        let avgPoints = totalMeanPoints / this.broadsheetRows.length;
        return this.getGradeForPoints(Math.round(avgPoints * 10) / 10);
    };

    getMaxTotal = (): string => {
        if (this.broadsheetRows.length === 0) return '-';
        let max = Math.max(...this.broadsheetRows.map((r) => r.total));
        let row = this.broadsheetRows.find((r) => r.total === max);
        let outOf = row?.totalOutOf || 0;
        return outOf > 0 ? `${max}/${outOf}` : max.toString();
    };

    getMinTotal = (): string => {
        if (this.broadsheetRows.length === 0) return '-';
        let min = Math.min(...this.broadsheetRows.map((r) => r.total));
        let row = this.broadsheetRows.find((r) => r.total === min);
        let outOf = row?.totalOutOf || 0;
        return outOf > 0 ? `${min}/${outOf}` : min.toString();
    };

    getMaxTotalPoints = (): string => {
        if (this.broadsheetRows.length === 0) return '-';
        let max = Math.max(...this.broadsheetRows.map((r) => r.totalPoints));
        let row = this.broadsheetRows.find((r) => r.totalPoints === max);
        let outOf = row?.totalPointsOutOf || 0;
        return outOf > 0 ? `${max}/${outOf}` : max.toString();
    };

    getMinTotalPoints = (): string => {
        if (this.broadsheetRows.length === 0) return '-';
        let min = Math.min(...this.broadsheetRows.map((r) => r.totalPoints));
        let row = this.broadsheetRows.find((r) => r.totalPoints === min);
        let outOf = row?.totalPointsOutOf || 0;
        return outOf > 0 ? `${min}/${outOf}` : min.toString();
    };

    getMax = (field: string): string => {
        if (this.broadsheetRows.length === 0) return '-';
        return Math.max(...this.broadsheetRows.map((r) => r[field])).toString();
    };

    getMin = (field: string): string => {
        if (this.broadsheetRows.length === 0) return '-';
        return Math.min(...this.broadsheetRows.map((r) => r[field])).toString();
    };
}
