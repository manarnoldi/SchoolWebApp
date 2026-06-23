import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of, map, switchMap} from 'rxjs';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {GradesService} from '@/academics/services/grades.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {ExamService} from '@/cbe/exams/services/exam.service';
import {ExamTypeService} from '@/cbe/exams/services/exam-type.service';
import {SchoolExamService} from '@/cbe/exams/services/school-exam.service';
import {ExamResultService} from '@/cbe/exams/services/exam-result.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
import {StudentValueScoreService} from '@/cbe/values/services/student-value-score.service';
import {StudentCoCurriculumActivityService} from '@/cbe/cocurriculum/services/student-co-curriculum-activity.service';
import {StudentCoCurriculumScoreService} from '@/cbe/cocurriculum/services/student-co-curriculum-score.service';
import {StudentResponsibilityService} from '@/cbe/responsibilities/services/student-responsibility.service';
import {StudentCommunityServiceActivityService} from '@/cbe/community-service/services/student-community-service-activity.service';
import {ValueService} from '@/cbe/values/services/value.service';
import {ValueScoreService} from '@/cbe/values/services/value-score.service';
import {ResponsibilityService} from '@/cbe/responsibilities/services/responsibility.service';
import {CoCurriculumScoreService} from '@/cbe/cocurriculum/services/co-curriculum-score.service';
import {AuthService} from '@/core/services/auth.service';
import {ReportsService} from '@/reports/services/reports.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {Status} from '@/core/enums/status';

import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
(pdfMake as any).vfs = pdfFonts;

@Component({
    selector: 'app-report-form',
    templateUrl: './report-form.component.html',
    styleUrl: './report-form.component.scss'
})
export class ReportFormComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/academics/report-forms'], title: 'Academics: Report Forms'}
    ];
    dashboardTitle = 'Academics: Report Forms';

    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    students: any[] = [];
    examTypes: any[] = [];
    grades: any[] = [];
    allGrades: any[] = [];
    gradingCategory: string = '4-Point';
    gradingCategories: string[] = ['4-Point', '8-Point'];
    selectedGradingCategory: string = '4-Point';
    values: any[] = [];
    valueScores: any[] = [];
    allCoCurriculumScores: any[] = [];
    responsibilities: any[] = [];
    socialSkills: any[] = [];
    rankingMethod: string = 'mean_points';
    // 'subjects_done' (default) divides totals/averages by subjects with marks;
    // 'subjects_expected' divides by all subjects the student is allocated to.
    meanBasis: string = 'subjects_done';
    // Raw Grading-module settings, used to resolve the exam-results grading
    // category (4-Point/8-Point) per education level.
    gradingSettings: any[] = [];
    learningLevels: any[] = [];
    educationLevels: any[] = [];

    // Global settings
    displayMode: string = 'marks_grades'; // marks, grades, points_grades, marks_grades
    showValues: boolean = true;
    showCoCurricular: boolean = true;
    showResponsibilities: boolean = true;
    showCommunityService: boolean = true;
    showPosition: boolean = false;
    // The word in the title (e.g. SUMMATIVE / PERFORMANCE) and whether the
    // term-dates line is shown - both configurable in Report Form settings.
    reportTypeLabel: string = 'SUMMATIVE';
    showTermDates: boolean = true;
    // Master list of internal exam types; this.examTypes is narrowed to the
    // ones with a registered school exam for the selected session.
    allExamTypes: any[] = [];

    // Class-wide data fetched ONCE per Load and reused for every student's
    // report (exams, all exam results, school details, settings, class
    // leaders, value scores, and the precomputed ranking totals). This is the
    // main speed-up: it removes the per-student re-fetching of class data.
    private shared: any = null;

    // Generation progress (shown on the spinner).
    genCurrent: number = 0;
    genTotal: number = 0;

    // Client-side paging for the students table.
    page: number = 1;
    pageSize: number = 20;

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterSchoolClassId: any = null;

    isGenerating: boolean = false;
    isLoading: boolean = false;
    studentsLoaded: boolean = false;

    // When printing several students, each report's content is collected here
    // and emitted as ONE PDF (one tab / one print job) instead of one per
    // student. bulkMode tells the per-student builder to collect rather than
    // render immediately.
    private bulkMode: boolean = false;
    private bulkDocs: any[] = [];

    studentRows: {
        studentId: number;
        upi: string;
        fullName: string;
        selected: boolean;
        student: any;
    }[] = [];

    constructor(
        private toastr: ToastrService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private schoolClassesSvc: SchoolClassesService,
        private learningLevelSvc: LearningLevelsService,
        private educationLevelSvc: EducationLevelService,
        private gradesSvc: GradesService,
        private schoolSvc: SchoolDetailsService,
        private examSvc: ExamService,
        private examTypeSvc: ExamTypeService,
        private schoolExamSvc: SchoolExamService,
        private examResultSvc: ExamResultService,
        private studentClassSvc: StudentClassService,
        private studentSubjectsSvc: StudentSubjectsService,
        private studentValueScoreSvc: StudentValueScoreService,
        private studentCoCurrActivitySvc: StudentCoCurriculumActivityService,
        private studentCoCurrScoreSvc: StudentCoCurriculumScoreService,
        private studentResponsibilitySvc: StudentResponsibilityService,
        private studentCommServiceSvc: StudentCommunityServiceActivityService,
        private valueSvc: ValueService,
        private valueScoreSvc: ValueScoreService,
        private responsibilitySvc: ResponsibilityService,
        private coCurrScoreSvc: CoCurriculumScoreService,
        private userSvc: AuthService,
        private reportSvc: ReportsService,
        private globalSettingSvc: GlobalSettingService
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.academicYearSvc.get('/academicYears'),
            this.examTypeSvc.get('/examTypes'),
            this.gradesSvc.get('/grades'),
            this.valueSvc.get('/values'),
            this.valueScoreSvc.get('/valueScores'),
            this.responsibilitySvc.get('/responsibilities'),
            this.globalSettingSvc.getByModule('ReportForm'),
            this.coCurrScoreSvc.get('/coCurriculumScores'),
            this.globalSettingSvc.getByModule('Grading')
        ]).subscribe({
            next: ([curricula, academicYears, examTypes, allGrades, values, valueScores, responsibilities, reportSettings, coCurrScores, gradingSettings]) => {
                this.gradingSettings = (gradingSettings as any[]) || [];
                this.meanBasis = this.settingVal('MeanBasis') || 'subjects_done';
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort((a, b) => b.rank - a.rank);
                this.examTypes = examTypes.filter((et) => et.internal).sort((a, b) => a.rank - b.rank);
                this.allExamTypes = this.examTypes;
                this.allGrades = allGrades;
                // Global default until a student's class (hence education level)
                // is known; generateReportForStudent re-resolves it per report.
                this.applyExamGrading(null);
                this.values = values.sort((a, b) => a.rank - b.rank);
                this.valueScores = valueScores.sort((a, b) => a.rank - b.rank);
                let allResp = responsibilities.sort((a, b) => a.rank - b.rank);
                this.responsibilities = allResp.filter((r) => r.category === 'Responsibility');
                this.socialSkills = allResp.filter((r) => r.category === 'Social Skill');
                this.allCoCurriculumScores = coCurrScores.sort((a, b) => a.rank - b.rank);
                this.rankingMethod = this.settingVal('RankingMethod') || 'mean_points';

                // Apply global settings
                (reportSettings as any[]).forEach((s) => {
                    if (s.settingKey === 'DisplayMode') this.displayMode = s.settingValue;
                    if (s.settingKey === 'ShowValues') this.showValues = s.settingValue === 'true';
                    if (s.settingKey === 'ShowCoCurricular') this.showCoCurricular = s.settingValue === 'true';
                    if (s.settingKey === 'ShowResponsibilities') this.showResponsibilities = s.settingValue === 'true';
                    if (s.settingKey === 'ShowCommunityService') this.showCommunityService = s.settingValue === 'true';
                    if (s.settingKey === 'ShowPosition') this.showPosition = s.settingValue === 'true';
                    if (s.settingKey === 'ReportTypeLabel') this.reportTypeLabel = s.settingValue || 'SUMMATIVE';
                    if (s.settingKey === 'ShowTermDates') this.showTermDates = s.settingValue !== 'false';
                });
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    onGradingCategoryChange = () => {
        this.grades = this.allGrades.filter(g => g.category === this.selectedGradingCategory).sort((a, b) => a.rank - b.rank);
    };

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
    // the global default, else mean_points.
    private rankingMethodFor = (edLevelId: any): string => {
        let globalVal = this.settingVal('RankingMethod') || 'mean_points';
        if (!edLevelId) return globalVal;
        return this.settingVal(`RankingMethod:${edLevelId}`) || globalVal;
    };

    private applyExamGrading = (edLevelId: any) => {
        this.gradingCategory = this.examGradingCategoryFor(edLevelId);
        this.selectedGradingCategory = this.gradingCategory;
        this.grades = this.allGrades.filter((g) => g.category === this.gradingCategory).sort((a, b) => a.rank - b.rank);
    };

    onCurriculumChange = () => {
        this.sessions = this.schoolClasses = [];
        this.filterAcademicYearId = this.filterSessionId = this.filterSchoolClassId = null;
        this.studentsLoaded = false;
        if (!this.filterCurriculumId) return;
        forkJoin([
            this.learningLevelSvc.getLearningLevelsByCurriculum(this.filterCurriculumId),
            this.educationLevelSvc.get(`/educationLevels/byCurriculumId?curriculumId=${this.filterCurriculumId}`)
        ]).subscribe({
            next: ([levels, edLevels]) => {
                this.learningLevels = levels.sort((a, b) => a.rank - b.rank);
                this.educationLevels = edLevels.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onAcademicYearChange = () => {
        this.sessions = this.schoolClasses = [];
        this.filterSessionId = this.filterSchoolClassId = null;
        this.studentsLoaded = false;
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

    onClassChange = () => {
        this.studentsLoaded = false;
    };

    loadStudents = () => {
        if (!this.filterSessionId || !this.filterSchoolClassId) {
            this.toastr.info('Please select Session and Class.');
            return;
        }
        this.isLoading = true;
        this.studentsLoaded = false;
        this.shared = null; // new class/session -> rebuild the class-wide cache
        forkJoin([
            this.studentClassSvc.getBySchoolClassId(this.filterSchoolClassId, Status.Active),
            this.schoolExamSvc.get(`/schoolExams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${this.filterSessionId}`)
        ]).subscribe({
            next: ([studentClasses, schoolExams]) => {
                // Only show exam-type columns that actually have a school exam
                // registered for this session (e.g. hide END until it is set up).
                let registeredTypeIds = new Set(
                    (schoolExams || [])
                        .map((se: any) => +(se.examTypeId ?? se.examType?.id))
                        .filter((id: number) => !!id)
                );
                this.examTypes = registeredTypeIds.size > 0
                    ? this.allExamTypes.filter((et) => registeredTypeIds.has(+et.id))
                    : this.allExamTypes;

                this.studentRows = studentClasses
                    .map((sc) => sc.student)
                    .filter(Boolean)
                    .sort((a, b) => (a.fullName || '').localeCompare(b.fullName || ''))
                    .map((s) => ({
                        studentId: +s.id,
                        upi: s.upi || '',
                        fullName: s.fullName || '',
                        selected: false,
                        student: s
                    }));
                this.page = 1;
                this.studentsLoaded = true;
                this.isLoading = false;
            },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error); }
        });
    };

    toggleSelectAll = () => {
        let all = this.allSelected();
        this.studentRows.forEach((r) => r.selected = !all);
    };

    pageChanged = (page: number) => { this.page = page; };
    pageSizeChanged = (pageSize: number) => { this.pageSize = pageSize; this.page = 1; };

    allSelected = (): boolean => {
        return this.studentRows.length > 0 && this.studentRows.every((r) => r.selected);
    };

    getSelectedCount = (): number => {
        return this.studentRows.filter((r) => r.selected).length;
    };

    previewStudent = (row: any, mode: string = 'preview') => {
        this.bulkMode = false;
        this.genTotal = 1;
        this.genCurrent = 1;
        this.generateReportForStudent(row.student, null, mode);
    };

    printSelected = (mode: string = 'preview') => {
        let selected = this.studentRows.filter((r) => r.selected);
        if (selected.length === 0) {
            this.toastr.info('Please select at least one student.');
            return;
        }
        this.isGenerating = true;
        this.bulkMode = true;
        this.bulkDocs = [];
        this.genTotal = selected.length;
        this.genCurrent = 0;
        let idx = 0;
        let generateNext = () => {
            if (idx >= selected.length) {
                this.bulkMode = false;
                this.emitBulkReport(mode, selected.length);
                return;
            }
            this.genCurrent = idx + 1;
            this.generateReportForStudent(selected[idx].student, () => {
                idx++;
                generateNext();
            }, mode);
        };
        generateNext();
    };

    // Merge every collected per-student report into a single PDF, with each
    // student starting on a new page, then render or print it once.
    private emitBulkReport = (mode: string, count: number) => {
        let docs = this.bulkDocs;
        this.bulkDocs = [];
        if (docs.length === 0) { this.isGenerating = false; return; }

        let base = docs[0];
        let merged: any[] = [];
        docs.forEach((doc, i) => {
            if (i > 0) merged.push({text: '', pageBreak: 'before'});
            merged = merged.concat(doc.content);
        });
        base.content = merged;
        base.info = {...(base.info || {}), title: `Report Cards (${count})`};

        let pdfDoc = pdfMake.createPdf(base);
        let done = () => { this.isGenerating = false; this.toastr.success(`${count} report(s) generated.`); };
        if (mode === 'print') {
            pdfDoc.print();
            done();
        } else {
            pdfDoc.getBlob((pdfBlob) => {
                window.open(URL.createObjectURL(pdfBlob), '_blank');
                done();
            });
        }
    };

    sectionBox = (title: string, content: any, color: string): any => {
        return {
            layout: {
                hLineWidth: (i) => (i === 0 || i === 2) ? 0.8 : 0,
                vLineWidth: (i, node) => (i === 0 || i === node.table.widths.length) ? 0.8 : 0,
                hLineColor: () => color,
                vLineColor: () => color,
                paddingLeft: () => 5,
                paddingRight: () => 5,
                paddingTop: () => 2,
                paddingBottom: () => 2
            },
            table: {
                widths: ['*'],
                body: [
                    [{text: title, bold: true, fontSize: 9, color: color}],
                    [typeof content === 'string' ? {text: content || '', fontSize: 9} : (content || {text: '', fontSize: 9})]
                ]
            },
            marginBottom: 4
        };
    };

    getGradeForPercent = (percent: number): any => {
        return this.grades.find((g) => percent >= g.minScore && percent <= g.maxScore);
    };

    generateReportForStudent = (student: any, callback?: () => void, mode: string = 'preview') => {
        this.isGenerating = true;

        let session = this.sessions.find((s) => s.id == this.filterSessionId);
        let schoolClass = this.schoolClasses.find((sc) => sc.id == this.filterSchoolClassId);
        let year = this.academicYears.find((y) => y.id == this.filterAcademicYearId);
        let studentId = +student.id;

        // Resolve the grading category for this class's education level before
        // any grades are computed for the report.
        let edLevelId = schoolClass?.learningLevel?.educationLevelId
            ?? this.learningLevels.find((ll) => +ll.id === +(schoolClass?.learningLevelId))?.educationLevelId;
        this.applyExamGrading(edLevelId);
        this.rankingMethod = this.rankingMethodFor(edLevelId);

        let proceed = () => {
            let shared = this.shared;
            // Only THIS student's own data is fetched per report; the heavy
            // class-wide data (exams, all results, settings...) is cached.
            forkJoin([
                this.studentCoCurrActivitySvc.get(`/studentCoCurriculumActivities/byStudentId/${studentId}`),
                this.studentResponsibilitySvc.get(`/studentResponsibilities/byStudentId/${studentId}`),
                this.studentCommServiceSvc.get(`/studentCommunityServiceActivities/byStudentId/${studentId}`),
                this.studentSubjectsSvc.get(`/studentSubjects/byStudentId/${studentId}`)
            ]).subscribe({
                next: ([coCurrActivities, studentResponsibilities, communityService, studentSubjects]) => {
                    let examsByType = shared.examsByType as any[][];

                    // Build a set of subject IDs the student is allocated to
                    let allocatedSubjectIds = new Set(
                        ((studentSubjects as any[]) || []).map((ss: any) => +ss.subjectId)
                    );

                    // Under the "expected subjects" basis, the Total Score and Average
                    // per exam type are divided by every allocated subject examined in
                    // that type (missing ones count as zero), not just the ones with
                    // marks. Null entries mean "use the recorded-subject count" (the
                    // default basis, or when the student has no allocation data).
                    let useExpected = this.meanBasis === 'subjects_expected';
                    let expectedByType: any = {};
                    this.examTypes.forEach((et, typeIdx) => {
                        let exams = examsByType[typeIdx] || [];
                        if (useExpected && allocatedSubjectIds.size > 0) {
                            let allocExams = exams.filter((e: any) => allocatedSubjectIds.has(+e.subjectId));
                            expectedByType[et.id] = {
                                count: allocExams.length,
                                outOf: allocExams.reduce((s: number, e: any) => s + (e.examMark || 0), 0)
                            };
                        } else {
                            expectedByType[et.id] = null;
                        }
                    });

                    // Get unique subjects from exams - filtered to only those the student is allocated to
                    let subjectMap = new Map<string, {name: string, abbr: string, rank: number}>();
                    examsByType.forEach((exams) => {
                        exams.forEach((e: any) => {
                            if (!e.subject) return;
                            if (allocatedSubjectIds.size > 0 && !allocatedSubjectIds.has(+e.subjectId)) return;
                            if (!subjectMap.has(e.subject.abbr || e.subject.name)) {
                                subjectMap.set(e.subject.abbr || e.subject.name, {
                                    name: e.subject.name, abbr: e.subject.abbr, rank: e.subject.rank || 0
                                });
                            }
                        });
                    });
                    let subjects = [...subjectMap.entries()].sort((a, b) => a[1].rank - b[1].rank);

                    // Subject scores read from the cached class-wide results.
                    let subjectScores: any = {};
                    examsByType.forEach((exams, typeIdx) => {
                        exams.forEach((exam: any) => {
                            let results = (shared.resultsByExamId.get(+exam.id) || []) as any[];
                            let studentResult = results.find((r: any) => r.studentId == studentId);
                            let key = exam.subject?.abbr || exam.subject?.name;
                            if (!subjectScores[key]) subjectScores[key] = {};
                            if (studentResult) {
                                let pct = exam.examMark > 0 ? (studentResult.score / exam.examMark) * 100 : 0;
                                let grade = this.getGradeForPercent(pct);
                                subjectScores[key][this.examTypes[typeIdx].id] = {
                                    score: studentResult.score,
                                    examMark: exam.examMark,
                                    grade: grade?.abbr || '',
                                    points: grade?.points || 0
                                };
                            }
                        });
                    });

                    // Positions are a cheap lookup against the cached class totals.
                    let positionsByType: any = {};
                    this.examTypes.forEach((et) => {
                        positionsByType[et.id] = shared.studentTotalsByType[et.id]
                            ? this.computeRank(shared.studentTotalsByType[et.id], studentId)
                            : {position: 0, totalStudents: 0};
                    });
                    let overallPosition = this.computeRank(shared.studentOverallTotals, studentId);
                    let positionData = {positionsByType, overall: overallPosition};

                    this.buildAndPrintReport(student, session, schoolClass, year, subjects, subjectScores, shared.valueScores, coCurrActivities, studentResponsibilities, communityService, shared.schoolDetails, shared.classLeadersText, callback, mode, positionData, expectedByType);
                },
                error: (err) => { this.isGenerating = false; this.bulkMode = false; this.toastr.error(err.error); }
            });
        };

        if (this.shared) {
            proceed();
        } else {
            this.loadSharedData().subscribe({
                next: () => proceed(),
                error: (err) => { this.isGenerating = false; this.bulkMode = false; this.toastr.error(err.error); }
            });
        }
    };

    // Fetch and cache the class-wide data used by EVERY student's report:
    // exams per type, all exam results (fetched once and reused), value scores,
    // school details, class leaders, report settings, and the ranking totals.
    private loadSharedData = () => {
        let examRequests = this.examTypes.map((et) =>
            this.examSvc.get(`/exams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${this.filterSessionId}&schoolClassId=${this.filterSchoolClassId}&examTypeId=${et.id}`)
        );
        return forkJoin([
            ...examRequests,
            this.studentValueScoreSvc.get(`/studentValueScores/bySessionId/${this.filterSessionId}`),
            this.schoolSvc.get('/schooldetails'),
            this.globalSettingSvc.getByModule('ReportForm'),
            this.schoolClassesSvc.get(`/schoolClassLeaders/bySchoolClassId/${this.filterSchoolClassId}`)
        ]).pipe(
            switchMap((results: any[]) => {
                let n = this.examTypes.length;
                let examsByType = results.slice(0, n) as any[][];
                let valueScores = results[n] as any[];
                let schoolDetails = (results[n + 1] as any[])?.[0];
                let reportSettings = results[n + 2] as any[];
                let classLeaders = results[n + 3] as any[];

                this.applyReportSettings(reportSettings);
                let classLeadersText = this.buildClassLeadersText(classLeaders);

                let examIds: number[] = [];
                examsByType.forEach((exams) => exams.forEach((e: any) => examIds.push(+e.id)));
                let results$ = examIds.length
                    ? forkJoin(examIds.map((id) => this.examResultSvc.get(`/examResults/byExamId/${id}`)))
                    : of([] as any[]);

                return results$.pipe(
                    map((allResults: any[]) => {
                        let resultsByExamId = new Map<number, any[]>();
                        examIds.forEach((id, i) => resultsByExamId.set(id, (allResults[i] as any[]) || []));
                        let totals = this.computeTotals(examsByType, resultsByExamId);
                        this.shared = {
                            examsByType, resultsByExamId, valueScores, schoolDetails, classLeadersText,
                            studentTotalsByType: totals.studentTotalsByType,
                            studentOverallTotals: totals.studentOverallTotals
                        };
                        return this.shared;
                    })
                );
            })
        );
    };

    private applyReportSettings = (settings: any[]) => {
        (settings || []).forEach((s) => {
            if (s.settingKey === 'DisplayMode') this.displayMode = s.settingValue;
            if (s.settingKey === 'ShowValues') this.showValues = s.settingValue === 'true';
            if (s.settingKey === 'ShowCoCurricular') this.showCoCurricular = s.settingValue === 'true';
            if (s.settingKey === 'ShowResponsibilities') this.showResponsibilities = s.settingValue === 'true';
            if (s.settingKey === 'ShowCommunityService') this.showCommunityService = s.settingValue === 'true';
            if (s.settingKey === 'ShowPosition') this.showPosition = s.settingValue === 'true';
            if (s.settingKey === 'ReportTypeLabel') this.reportTypeLabel = s.settingValue || 'SUMMATIVE';
            if (s.settingKey === 'ShowTermDates') this.showTermDates = s.settingValue !== 'false';
        });
    };

    private buildClassLeadersText = (classLeaders: any[]): string => {
        let teacherLeaders = (classLeaders || []).filter(
            (cl: any) => cl.classLeadershipRole?.personType === 1 || cl.classLeadershipRole?.personType === 'Teacher'
        );
        return teacherLeaders
            .map((cl: any) => `${cl.person?.fullName || ''} [${cl.classLeadershipRole?.name || ''}]`)
            .filter(Boolean)
            .join(', ') || '';
    };

    // Sum each student's marks (or points) per exam type and overall - once for
    // the whole class - so per-student ranking is a cheap lookup.
    private computeTotals = (examsByType: any[][], resultsByExamId: Map<number, any[]>) => {
        let usePoints = this.rankingMethod === 'mean_points';
        let studentTotalsByType: any = {};
        let studentOverallTotals: any = {};
        examsByType.forEach((exams, typeIdx) => {
            let etId = this.examTypes[typeIdx].id;
            if (!studentTotalsByType[etId]) studentTotalsByType[etId] = {};
            exams.forEach((exam: any) => {
                let results = resultsByExamId.get(+exam.id) || [];
                results.forEach((r: any) => {
                    let val = r.score || 0;
                    if (usePoints) {
                        let percent = exam.examMark > 0 ? (val / exam.examMark) * 100 : 0;
                        let grade = this.grades.find((g) => percent >= g.minScore && percent <= g.maxScore);
                        val = grade ? grade.points : 0;
                    }
                    studentTotalsByType[etId][r.studentId] = (studentTotalsByType[etId][r.studentId] || 0) + val;
                    studentOverallTotals[r.studentId] = (studentOverallTotals[r.studentId] || 0) + val;
                });
            });
        });
        return {studentTotalsByType, studentOverallTotals};
    };

    private computeRank = (totals: any, targetId: number) => {
        let sorted = Object.entries(totals).sort((a: any, b: any) => b[1] - a[1]);
        let total = sorted.length;
        let pos = 0;
        for (let i = 0; i < sorted.length; i++) {
            if (+sorted[i][0] == targetId) {
                pos = i + 1;
                if (i > 0 && sorted[i][1] === sorted[i - 1][1]) {
                    for (let j = i - 1; j >= 0; j--) {
                        if (sorted[j][1] === sorted[i][1]) pos = j + 1;
                        else break;
                    }
                }
                break;
            }
        }
        return {position: pos, totalStudents: total};
    };

    buildAndPrintReport = (student, session, schoolClass, year, subjects, subjectScores, valueScores, coCurrActivities, studentResponsibilities, communityService, school, classLeadersText: string, callback?: () => void, mode: string = 'preview', positionData?: any, expectedByType?: any) => {
        this.reportSvc.loadImageAsBase64('assets/img/shule-nova-logo-only.png').subscribe({
            next: (blob) => {
                const reader = new FileReader();
                reader.onloadend = () => {
                    const base64data: string = reader.result as string;

                    // Grading key table
                    let gradingKey = this.grades.map((g) => [
                        {text: `${g.name} (${g.abbr}) ${g.points}`, alignment: 'center', fontSize: 8},
                        {text: `${g.minScore}%-${g.maxScore}%`, alignment: 'center', fontSize: 8}
                    ]);
                    let gradingKeyTable = {
                        layout: 'lightHorizontalLines',
                        table: {
                            widths: this.grades.map(() => '*'),
                            body: [
                                this.grades.map((g) => ({text: `${g.name}\n(${g.abbr}) ${g.points}`, alignment: 'center', fontSize: 8, bold: true})),
                                this.grades.map((g) => ({text: `${g.minScore}%-${g.maxScore}%`, alignment: 'center', fontSize: 8}))
                            ]
                        },
                        marginBottom: 5
                    };

                    // Subject scores table - dynamic based on displayMode
                    let isTwoCol = this.displayMode === 'marks_grades' || this.displayMode === 'points_grades';
                    let subjectHeaders: any[] = [
                        {text: 'LEARNING AREAS', style: 'tableHeader', rowSpan: isTwoCol ? 2 : 1},
                    ];
                    let subHeaders: any[] = isTwoCol ? [{text: ''}] : null;
                    this.examTypes.forEach((et) => {
                        if (isTwoCol) {
                            subjectHeaders.push({text: et.abbreviation || et.name, style: 'tableHeader', colSpan: 2, alignment: 'center'});
                            subjectHeaders.push({text: ''});
                            if (this.displayMode === 'marks_grades') {
                                subHeaders.push({text: 'MKS', alignment: 'center', bold: true, fontSize: 8});
                                subHeaders.push({text: 'GRD', alignment: 'center', bold: true, fontSize: 8});
                            } else {
                                subHeaders.push({text: 'PTS', alignment: 'center', bold: true, fontSize: 8});
                                subHeaders.push({text: 'GRD', alignment: 'center', bold: true, fontSize: 8});
                            }
                        } else {
                            subjectHeaders.push({text: et.abbreviation || et.name, style: 'tableHeader', alignment: 'center'});
                        }
                    });

                    let subjectBody: any[] = isTwoCol ? [subjectHeaders, subHeaders] : [subjectHeaders];
                    let totalsByType: any = {};
                    let countsByType: any = {};
                    let outOfByType: any = {};
                    this.examTypes.forEach((et) => { totalsByType[et.id] = 0; countsByType[et.id] = 0; outOfByType[et.id] = 0; });

                    subjects.forEach(([key, subj]) => {
                        let row: any[] = [{text: subj.name, fontSize: 9}];
                        this.examTypes.forEach((et) => {
                            let entry = subjectScores[key]?.[et.id];
                            if (entry) {
                                let pctStr = entry.examMark > 0 ? Math.round((entry.score / entry.examMark) * 100) + '%' : entry.score;
                                if (this.displayMode === 'marks') {
                                    row.push({text: pctStr, alignment: 'center', fontSize: 9});
                                } else if (this.displayMode === 'grades') {
                                    row.push({text: entry.grade, alignment: 'center', fontSize: 9});
                                } else if (this.displayMode === 'points_grades') {
                                    row.push({text: entry.points, alignment: 'center', fontSize: 9});
                                    row.push({text: entry.grade, alignment: 'center', fontSize: 9});
                                } else {
                                    row.push({text: pctStr, alignment: 'center', fontSize: 9});
                                    row.push({text: entry.grade, alignment: 'center', fontSize: 9});
                                }
                                totalsByType[et.id] += entry.score;
                                outOfByType[et.id] += entry.examMark || 0;
                                countsByType[et.id]++;
                            } else {
                                row.push({text: '-', alignment: 'center', fontSize: 9});
                                if (isTwoCol) row.push({text: '', alignment: 'center', fontSize: 9});
                            }
                        });
                        subjectBody.push(row);
                    });

                    // Total Score row (e.g., 440/600)
                    let totalRow: any[] = [{text: 'Total Score', bold: true, fontSize: 9}];
                    this.examTypes.forEach((et) => {
                        let exp = expectedByType?.[et.id];
                        let outOf = exp ? exp.outOf : outOfByType[et.id];
                        // Keep '-' when the student has no marks at all; only switch
                        // the denominator to the expected (allocated) basis once at
                        // least one subject has been scored.
                        let totalText = countsByType[et.id] > 0
                            ? `${Math.round(totalsByType[et.id])}/${outOf}`
                            : '-';
                        if (isTwoCol) {
                            totalRow.push({text: totalText, alignment: 'center', bold: true, fontSize: 9, colSpan: 2});
                            totalRow.push({text: ''});
                        } else {
                            totalRow.push({text: totalText, alignment: 'center', bold: true, fontSize: 9});
                        }
                    });
                    subjectBody.push(totalRow);

                    // Average row
                    let avgRow: any[] = [{text: 'Average Score', bold: true, fontSize: 9}];
                    this.examTypes.forEach((et) => {
                        let exp = expectedByType?.[et.id];
                        let denomCount = exp ? exp.count : countsByType[et.id];
                        // No marks entered for this exam type -> leave it blank
                        // rather than showing a 0% / lowest-grade (e.g. BE 2).
                        let hasMarks = countsByType[et.id] > 0;
                        let avg = (hasMarks && denomCount > 0) ? Math.round(totalsByType[et.id] / denomCount) : 0;
                        let grade = hasMarks ? this.getGradeForPercent(avg) : null;
                        let avgStr = hasMarks ? avg + '%' : '-';
                        if (this.displayMode === 'marks') {
                            avgRow.push({text: avgStr, alignment: 'center', bold: true, fontSize: 9});
                        } else if (this.displayMode === 'grades') {
                            avgRow.push({text: grade?.abbr || '-', alignment: 'center', bold: true, fontSize: 9});
                        } else {
                            avgRow.push({text: avgStr, alignment: 'center', bold: true, fontSize: 9});
                            avgRow.push({text: grade?.abbr || '', alignment: 'center', bold: true, fontSize: 9});
                        }
                    });
                    subjectBody.push(avgRow);

                    // Position row (combined position/total, e.g. 4/13)
                    if (this.showPosition && positionData) {
                        let posRow: any[] = [{text: 'Position', bold: true, fontSize: 9}];
                        this.examTypes.forEach((et) => {
                            let etPos = positionData.positionsByType?.[et.id];
                            let posText = etPos && etPos.position > 0 && etPos.totalStudents > 0
                                ? `${etPos.position}/${etPos.totalStudents}`
                                : '-';
                            if (isTwoCol) {
                                posRow.push({text: posText, alignment: 'center', bold: true, fontSize: 9, colSpan: 2});
                                posRow.push({text: ''});
                            } else {
                                posRow.push({text: posText, alignment: 'center', bold: true, fontSize: 9});
                            }
                        });
                        subjectBody.push(posRow);

                        // Overall position row
                        if (positionData.overall && positionData.overall.totalStudents > 0) {
                            let overallRow: any[] = [{text: 'Overall Position', bold: true, fontSize: 9, fillColor: '#fff3cd'}];
                            let colCount = isTwoCol ? this.examTypes.length * 2 : this.examTypes.length;
                            overallRow.push({
                                text: `${positionData.overall.position}/${positionData.overall.totalStudents}`,
                                alignment: 'center', bold: true, fontSize: 9, colSpan: colCount, fillColor: '#fff3cd'
                            });
                            for (let c = 1; c < colCount; c++) overallRow.push({text: ''});
                            subjectBody.push(overallRow);
                        }
                    }

                    let subjectWidths: any[] = ['*'];
                    this.examTypes.forEach(() => {
                        if (isTwoCol) { subjectWidths.push(30, 30); }
                        else { subjectWidths.push(40); }
                    });

                    // Values section - only show values that have a rating
                    let studentValues = (valueScores as any[]).filter((vs) => vs.studentId == +student.id);
                    let valuesRichText: any[] = [];
                    let ratedCount = 0;
                    this.values.forEach((v) => {
                        let sv = studentValues.find((s) => s.valueId == +v.id);
                        if (!sv) return;
                        let score = this.valueScores.find((vs) => vs.id == sv.valueScoreId);
                        if (!score) return;
                        if (ratedCount > 0) valuesRichText.push({text: ', ', fontSize: 9});
                        valuesRichText.push({text: `${v.name}: ${score.name}`, fontSize: 9});
                        if (score.abbreviation) valuesRichText.push({text: ` (${score.abbreviation})`, fontSize: 9, bold: true});
                        ratedCount++;
                    });
                    let valuesContent: any = ratedCount > 0 ? {text: valuesRichText, fontSize: 9} : {text: '', fontSize: 9};

                    // Co-curricular section
                    let coCurrText = (coCurrActivities as any[]).map((a) => a.coCurriculumActivity?.name || '').filter(Boolean).join(', ');

                    // Responsibilities section
                    let respText = (studentResponsibilities as any[]).map((sr) => {
                        let item = sr.responsibilitySocialSkill;
                        return item ? `${item.name} (${item.category || ''})` : '';
                    }).filter(Boolean).join(', ');

                    // Community service section
                    let commText = (communityService as any[]).map((cs) => cs.communityServiceActivity?.name || '').filter(Boolean).join(', ');

                    let sessionName = session?.sessionName || '';
                    let termEndDate = session?.endDate ? new Date(session.endDate).toLocaleDateString('en-GB') : '..............................';
                    let nextSession = this.sessions.find((s) => s.rank == (session?.rank || 0) + 1);
                    let nextTermStartDate = nextSession?.startDate ? new Date(nextSession.startDate).toLocaleDateString('en-GB') : '..............................';
                    let gradeName = schoolClass?.learningLevel?.name || '';
                    let streamName = schoolClass?.schoolStream?.name || '';
                    let edLevelId = schoolClass?.learningLevel?.educationLevelId;
                    let educationLevel = this.educationLevels.find((el) => el.id == edLevelId);
                    let educationLevelName = educationLevel?.name || '';
                    let reportTitle = `${sessionName.toUpperCase()} ${(year?.name || '').toUpperCase()} ${(this.reportTypeLabel || 'SUMMATIVE').toUpperCase()} REPORT FOR ${educationLevelName.toUpperCase()}`;

                    const docDefinition: any = {
                        pageMargins: [25, 20, 25, 30],
                        pageSize: 'A4',
                        info: {
                            title: `Report Card - ${student.fullName}`,
                            author: (this.userSvc?.currentUser?.firstName || '') + ' ' + (this.userSvc?.currentUser?.lastName || '')
                        },
                        footer: this.reportSvc.getFooter('portrait'),
                        images: { systemLogo: base64data, schoolLogo: school?.logoAsBase64 },
                        styles: {
                            tableHeader: {bold: true, fontSize: 9, fillColor: '#d4edda', color: '#155724'}
                        },
                        content: [
                            {...this.reportSvc.getDIVIDER()},
                            this.reportSvc.getReportHeader(school),
                            {...this.reportSvc.getDIVIDER(), marginBottom: 1},
                            this.reportSvc.getReportTitle(reportTitle),
                            {...this.reportSvc.getDIVIDER(), marginBottom: 3},
                            // Student details
                            {
                                layout: {
                                    hLineWidth: (i) => (i === 0 || i === 2) ? 0.8 : 0.3,
                                    vLineWidth: (i, node) => (i === 0 || i === node.table.widths.length) ? 0.8 : 0,
                                    hLineColor: () => '#6fbf73',
                                    vLineColor: () => '#6fbf73',
                                    paddingLeft: () => 5,
                                    paddingRight: () => 3,
                                    paddingTop: () => 3,
                                    paddingBottom: () => 3
                                },
                                table: {
                                    widths: ['auto', 'auto', 'auto', '*', 'auto', 'auto', 'auto', 'auto', 'auto', 'auto'],
                                    body: [
                                        [
                                            {text: 'Adm No:', fontSize: 9, bold: true},
                                            {text: student.upi || '', fontSize: 10, bold: true},
                                            {text: 'Name:', fontSize: 9, bold: true},
                                            {text: student.fullName, fontSize: 10, bold: true, noWrap: true},
                                            {text: 'Grade:', fontSize: 9, bold: true},
                                            {text: gradeName, fontSize: 10},
                                            {text: 'Stream:', fontSize: 9, bold: true},
                                            {text: streamName, fontSize: 10},
                                            {text: 'Term:', fontSize: 9, bold: true},
                                            {text: sessionName, fontSize: 10}
                                        ],
                                        [
                                            {text: 'Year:', fontSize: 9, bold: true},
                                            {text: year?.name || '', fontSize: 10},
                                            {text: 'Class Leaders:', fontSize: 9, bold: true},
                                            {text: classLeadersText || '..............................', fontSize: 9, noWrap: true},
                                            {text: '', fontSize: 9},
                                            {text: '', fontSize: 9},
                                            {text: '', fontSize: 9},
                                            {text: '', fontSize: 9},
                                            {text: '', fontSize: 9},
                                            {text: '', fontSize: 9}
                                        ]
                                    ]
                                },
                                marginBottom: 8
                            },
                            // Grading key
                            gradingKeyTable,
                            // Subject scores
                            {
                                layout: {
                                    hLineWidth: () => 0.5,
                                    vLineWidth: () => 0.5,
                                    hLineColor: () => '#aaa',
                                    vLineColor: () => '#aaa'
                                },
                                table: {
                                    headerRows: 2,
                                    widths: subjectWidths,
                                    body: subjectBody
                                },
                                marginBottom: 8
                            },
                            // CBE Sections in boxes
                            ...(this.showValues ? [this.sectionBox('VALUES', valuesContent, '#6f42c1')] : []),
                            ...(this.showCoCurricular ? [this.sectionBox('CO-CURRICULAR ACTIVITIES', coCurrText, '#0d6efd')] : []),
                            ...(this.showResponsibilities ? [this.sectionBox('RESPONSIBILITIES & SOCIAL SKILLS', respText, '#198754')] : []),
                            ...(this.showCommunityService ? [this.sectionBox('COMMUNITY SERVICE', commText, '#fd7e14')] : []),
                            // Class teacher box
                            {
                                layout: {
                                    hLineWidth: (i) => (i === 0 || i === 2) ? 0.4 : 0,
                                    vLineWidth: () => 0.4,
                                    hLineColor: () => '#ccc',
                                    vLineColor: () => '#ccc',
                                    paddingLeft: () => 6,
                                    paddingRight: () => 6,
                                    paddingTop: () => 8,
                                    paddingBottom: () => 8
                                },
                                table: {
                                    widths: ['*'],
                                    body: [
                                        [{text: "Class teacher's comments: .............................................................................................................................................", fontSize: 9}],
                                        [{text: 'Signature: ..............................................                                        Date: ..............................................', fontSize: 9}]
                                    ]
                                },
                                marginTop: 5
                            },
                            // Head teacher box
                            {
                                layout: {
                                    hLineWidth: (i) => (i === 0 || i === 2) ? 0.4 : 0,
                                    vLineWidth: () => 0.4,
                                    hLineColor: () => '#ccc',
                                    vLineColor: () => '#ccc',
                                    paddingLeft: () => 6,
                                    paddingRight: () => 6,
                                    paddingTop: () => 8,
                                    paddingBottom: () => 8
                                },
                                table: {
                                    widths: ['*'],
                                    body: [
                                        [{text: "Head teacher's comments: ..............................................................................................................................................", fontSize: 9}],
                                        [{text: 'Signature: ..............................................                                        Date: ..............................................', fontSize: 9}]
                                    ]
                                },
                                marginTop: 3
                            },
                            // Parent row
                            {
                                layout: {
                                    hLineWidth: () => 0.4,
                                    vLineWidth: () => 0.4,
                                    hLineColor: () => '#ccc',
                                    vLineColor: () => '#ccc',
                                    paddingLeft: () => 6,
                                    paddingRight: () => 6,
                                    paddingTop: () => 8,
                                    paddingBottom: () => 8
                                },
                                table: {
                                    widths: ['*'],
                                    body: [
                                        [{text: "Parent/Guardian's signature: ..............................................                                        Date: ..............................................", fontSize: 9}]
                                    ]
                                },
                                marginTop: 3
                            },
                            // Term dates (toggle via the ShowTermDates setting)
                            ...(this.showTermDates ? [{
                                layout: 'noBorders',
                                table: {
                                    widths: ['auto', 'auto', '*', 'auto', 'auto'],
                                    body: [
                                        [
                                            {text: 'This term ends on:', fontSize: 9, bold: true},
                                            {text: termEndDate, fontSize: 9, decoration: 'underline'},
                                            {text: '', fontSize: 9},
                                            {text: 'Next term begins on:', fontSize: 9, bold: true},
                                            {text: nextTermStartDate, fontSize: 9, decoration: 'underline'}
                                        ]
                                    ]
                                },
                                marginTop: 5
                            }] : []),
                            // System generated note
                            {
                                text: `This is a system generated document. Printed on ${new Date().toLocaleString('en-GB')}`,
                                fontSize: 8,
                                color: '#999999',
                                italics: true,
                                alignment: 'center',
                                marginTop: 8
                            }
                        ]
                    };

                    // Bulk: stash this student's document and let the loop
                    // continue; emitBulkReport merges them into one PDF at the end.
                    if (this.bulkMode) {
                        this.bulkDocs.push(docDefinition);
                        if (callback) callback();
                        return;
                    }

                    let pdfDoc = pdfMake.createPdf(docDefinition);
                    if (mode === 'print') {
                        pdfDoc.print();
                        if (!callback) this.isGenerating = false;
                        if (callback) setTimeout(() => callback(), 500);
                    } else {
                        pdfDoc.getBlob((pdfBlob) => {
                            const pdfUrl = URL.createObjectURL(pdfBlob);
                            window.open(pdfUrl, '_blank');
                            if (!callback) this.isGenerating = false;
                            if (callback) callback();
                        });
                    }
                };
                reader.readAsDataURL(blob);
            },
            error: () => { this.isGenerating = false; this.bulkMode = false; this.toastr.error('Error loading logo.'); }
        });
    };
}
