import {Component, OnInit} from '@angular/core';
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
import {StudentCoCurriculumActivityService} from '@/cbe/cocurriculum/services/student-co-curriculum-activity.service';
import {GenderService} from '@/settings/services/gender.service';
import {AuthService} from '@/core/services/auth.service';
import {ReportsService} from '@/reports/services/reports.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {Status} from '@/core/enums/status';

import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
(pdfMake as any).vfs = pdfFonts;

/**
 * Annual (end-of-year) report form: one page per learner summarising the
 * whole academic year. For every learning area it shows each term's average
 * - the mean percentage across that term's report-form exams (opener, mid and
 * end term; types not flagged "Show on report form", e.g. marathons, are left
 * out) - with its performance level (P.L), then the overall mean of the terms.
 *
 * Sits beside the per-term report form (ReportFormComponent) rather than
 * replacing it, and reuses its look and the ReportForm/Grading settings.
 */
@Component({
    selector: 'app-annual-report-form',
    templateUrl: './annual-report-form.component.html'
})
export class AnnualReportFormComponent implements OnInit {
    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    learningLevels: any[] = [];
    educationLevels: any[] = [];
    genders: any[] = [];
    allGrades: any[] = [];
    grades: any[] = [];
    gradingSettings: any[] = [];
    // Report-form exam types only (ShowOnReportForm), by rank.
    reportExamTypes: any[] = [];

    showPosition: boolean = false;
    showCoCurricular: boolean = true;
    cbeSectionDisplay: string = 'ratings';
    rankingMethod: string = 'mean_points';

    // Class-wide data fetched once per Load: each term's exams and results,
    // school details, class teacher and the class ranking.
    private shared: any = null;

    genCurrent: number = 0;
    genTotal: number = 0;
    page: number = 1;
    pageSize: number = 20;

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSchoolClassId: any = null;

    isGenerating: boolean = false;
    isLoading: boolean = false;
    studentsLoaded: boolean = false;

    private bulkMode: boolean = false;
    private bulkDocs: any[] = [];

    studentRows: {studentId: number; upi: string; fullName: string; selected: boolean; student: any}[] = [];

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
        private studentCoCurrActivitySvc: StudentCoCurriculumActivityService,
        private gendersSvc: GenderService,
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
            this.gendersSvc.get('/genders'),
            this.globalSettingSvc.getByModule('ReportForm'),
            this.globalSettingSvc.getByModule('Grading')
        ]).subscribe({
            next: ([curricula, academicYears, examTypes, allGrades, genders, reportSettings, gradingSettings]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort((a, b) => b.rank - a.rank);
                this.reportExamTypes = examTypes.filter((et) => et.showOnReportForm).sort((a, b) => a.rank - b.rank);
                this.allGrades = allGrades;
                this.genders = genders || [];
                this.gradingSettings = (gradingSettings as any[]) || [];
                this.applyReportSettings(reportSettings as any[]);
                this.applyExamGrading(null);
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    private applyReportSettings = (settings: any[]) => {
        (settings || []).forEach((s) => {
            if (s.settingKey === 'ShowPosition') this.showPosition = s.settingValue === 'true';
            if (s.settingKey === 'ShowCoCurricular') this.showCoCurricular = s.settingValue === 'true';
            if (s.settingKey === 'CbeSectionDisplay') this.cbeSectionDisplay = s.settingValue || 'ratings';
        });
    };

    private settingVal = (key: string): string =>
        this.gradingSettings.find((s) => s.settingKey === key)?.settingValue || '';

    // Same resolution as the term report form: the education level's override,
    // else the global default.
    private applyExamGrading = (edLevelId: any) => {
        let globalCat = this.settingVal('ExamResults') || '4-Point';
        let category = (edLevelId && this.settingVal(`ExamResults:${edLevelId}`)) || globalCat;
        this.grades = this.allGrades.filter((g) => g.category === category).sort((a, b) => a.rank - b.rank);
        let globalRank = this.settingVal('RankingMethod') || 'mean_points';
        this.rankingMethod = (edLevelId && this.settingVal(`RankingMethod:${edLevelId}`)) || globalRank;
    };

    private gradeFor = (percent: number): any =>
        this.grades.find((g) => percent >= g.minScore && percent <= g.maxScore);

    onCurriculumChange = () => {
        this.sessions = this.schoolClasses = [];
        this.filterAcademicYearId = this.filterSchoolClassId = null;
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
        this.filterSchoolClassId = null;
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

    onClassChange = () => { this.studentsLoaded = false; };

    loadStudents = () => {
        if (!this.filterAcademicYearId || !this.filterSchoolClassId) {
            this.toastr.info('Please select Year and Class.');
            return;
        }
        if (this.sessions.length === 0) {
            this.toastr.info('No terms are set up for the selected year.');
            return;
        }
        this.isLoading = true;
        this.studentsLoaded = false;
        this.shared = null;
        this.studentClassSvc.getBySchoolClassId(this.filterSchoolClassId, Status.Active).subscribe({
            next: (studentClasses) => {
                this.studentRows = studentClasses
                    .map((sc) => sc.student)
                    .filter(Boolean)
                    .sort((a, b) => (a.fullName || '').localeCompare(b.fullName || ''))
                    .map((s) => ({studentId: +s.id, upi: s.upi || '', fullName: s.fullName || '', selected: false, student: s}));
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
    allSelected = (): boolean => this.studentRows.length > 0 && this.studentRows.every((r) => r.selected);
    getSelectedCount = (): number => this.studentRows.filter((r) => r.selected).length;

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
            this.generateReportForStudent(selected[idx].student, () => { idx++; generateNext(); }, mode);
        };
        generateNext();
    };

    // One PDF for the whole selection, each learner starting on a new page.
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
        base.info = {...(base.info || {}), title: `Annual Report Cards (${count})`};
        this.outputPdf(base, mode, () => { this.isGenerating = false; this.toastr.success(`${count} report(s) generated.`); });
    };

    private outputPdf = (docDefinition: any, mode: string, done: () => void) => {
        let pdfDoc = pdfMake.createPdf(docDefinition);
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

    private currentEdLevelId = () => {
        let schoolClass = this.schoolClasses.find((sc) => sc.id == this.filterSchoolClassId);
        return schoolClass?.learningLevel?.educationLevelId
            ?? this.learningLevels.find((ll) => +ll.id === +(schoolClass?.learningLevelId))?.educationLevelId;
    };

    // Class-wide data for every report: for each term, the exams of each
    // report-form type that has a school exam registered in that term, plus
    // their results; school details; class leaders; and the class ranking.
    private loadSharedData = () => {
        let base = `academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}`;
        return forkJoin(this.sessions.map((s) =>
            this.schoolExamSvc.get(`/schoolExams/examSearch?${base}&sessionId=${s.id}`)
        )).pipe(
            switchMap((schoolExamsBySession: any[]) => {
                // [sessionIdx, examType] pairs that actually have an exam registered.
                let pairs: {sIdx: number; et: any}[] = [];
                this.sessions.forEach((s, sIdx) => {
                    let registered = new Set(((schoolExamsBySession[sIdx] as any[]) || [])
                        .map((se: any) => +(se.examTypeId ?? se.examType?.id)).filter((id: number) => !!id));
                    this.reportExamTypes.filter((et) => registered.has(+et.id)).forEach((et) => pairs.push({sIdx, et}));
                });
                let examReqs = pairs.map((p) =>
                    this.examSvc.get(`/exams/examSearch?${base}&sessionId=${this.sessions[p.sIdx].id}&schoolClassId=${this.filterSchoolClassId}&examTypeId=${p.et.id}`)
                );
                return forkJoin([
                    examReqs.length ? forkJoin(examReqs) : of([] as any[]),
                    this.schoolSvc.get('/schooldetails'),
                    this.globalSettingSvc.getByModule('ReportForm'),
                    this.schoolClassesSvc.get(`/schoolClassLeaders/bySchoolClassId/${this.filterSchoolClassId}`)
                ]).pipe(
                    switchMap(([examLists, schoolDetails, reportSettings, classLeaders]: any[]) => {
                        this.applyReportSettings(reportSettings);
                        // examsBySession[sIdx] = every report-form exam (all subjects) of that term.
                        let examsBySession: any[][] = this.sessions.map(() => []);
                        pairs.forEach((p, i) => examsBySession[p.sIdx].push(...((examLists[i] as any[]) || [])));
                        let examIds: number[] = [];
                        examsBySession.forEach((exams) => exams.forEach((e: any) => examIds.push(+e.id)));
                        let results$ = examIds.length
                            ? forkJoin(examIds.map((id) => this.examResultSvc.get(`/examResults/byExamId/${id}`)))
                            : of([] as any[]);
                        return results$.pipe(map((allResults: any[]) => {
                            let resultsByExamId = new Map<number, any[]>();
                            examIds.forEach((id, i) => resultsByExamId.set(id, (allResults[i] as any[]) || []));
                            this.shared = {
                                examsBySession, resultsByExamId,
                                schoolDetails: (schoolDetails as any[])?.[0],
                                classTeacherText: this.buildClassTeacherText(classLeaders as any[])
                            };
                            this.shared.ranking = this.computeClassRanking();
                            return this.shared;
                        }));
                    })
                );
            })
        );
    };

    // Teacher-type class leaders; a role named "class teacher" is preferred so
    // the field reads like the paper form, otherwise every teacher leader.
    private buildClassTeacherText = (classLeaders: any[]): string => {
        let teachers = (classLeaders || []).filter(
            (cl: any) => cl.classLeadershipRole?.personType === 1 || cl.classLeadershipRole?.personType === 'Teacher'
        );
        let classTeachers = teachers.filter((cl: any) => /class\s*teacher/i.test(cl.classLeadershipRole?.name || ''));
        return (classTeachers.length ? classTeachers : teachers)
            .map((cl: any) => cl.person?.fullName || '').filter(Boolean).join(', ');
    };

    /**
     * Per-subject term averages for one learner.
     * Returns {subjectKey: {name, rank, terms: (number|null)[], overall: number|null}}
     * where each term value is the rounded mean percentage across that term's
     * report-form exams the learner sat, and overall is the rounded mean of the
     * terms that have marks. Rounding first keeps the printed totals adding up.
     */
    private computeStudentScores = (studentId: number, allocatedSubjectIds?: Set<number>) => {
        let subjects: any = {};
        this.shared.examsBySession.forEach((exams: any[], sIdx: number) => {
            let pctsBySubject: any = {};
            exams.forEach((exam: any) => {
                if (!exam.subject) return;
                if (allocatedSubjectIds && allocatedSubjectIds.size > 0 && !allocatedSubjectIds.has(+exam.subjectId)) return;
                let key = exam.subject.abbr || exam.subject.name;
                if (!subjects[key]) {
                    subjects[key] = {name: exam.subject.name, rank: exam.subject.rank || 0, terms: this.sessions.map(() => null), overall: null};
                }
                let result = (this.shared.resultsByExamId.get(+exam.id) || []).find((r: any) => r.studentId == studentId);
                if (!result || !(exam.examMark > 0)) return;
                (pctsBySubject[key] = pctsBySubject[key] || []).push((result.score / exam.examMark) * 100);
            });
            Object.keys(pctsBySubject).forEach((key) => {
                let pcts = pctsBySubject[key] as number[];
                subjects[key].terms[sIdx] = Math.round(pcts.reduce((s, p) => s + p, 0) / pcts.length);
            });
        });
        Object.values(subjects).forEach((subj: any) => {
            let done = subj.terms.filter((t: any) => t !== null) as number[];
            subj.overall = done.length ? Math.round(done.reduce((s, t) => s + t, 0) / done.length) : null;
        });
        return subjects;
    };

    // Class ranking on the overall column, by the education level's ranking
    // method: total P.L points (mean_points) or total overall marks.
    private computeClassRanking = () => {
        let studentIds = new Set<number>();
        this.shared.examsBySession.forEach((exams: any[]) => exams.forEach((e: any) =>
            (this.shared.resultsByExamId.get(+e.id) || []).forEach((r: any) => studentIds.add(+r.studentId))));
        let usePoints = this.rankingMethod === 'mean_points';
        let totals: {[id: number]: number} = {};
        studentIds.forEach((id) => {
            let scores = this.computeStudentScores(id);
            totals[id] = Object.values(scores).reduce((sum: number, s: any) =>
                s.overall === null ? sum : sum + (usePoints ? (this.gradeFor(s.overall)?.points || 0) : s.overall), 0) as number;
        });
        return totals;
    };

    private computeRank = (totals: any, targetId: number) => {
        let sorted = Object.entries(totals).sort((a: any, b: any) => b[1] - a[1]);
        let idx = sorted.findIndex((e) => +e[0] === targetId);
        if (idx < 0) return {position: 0, totalStudents: sorted.length};
        // Ties share the higher position.
        let firstIdx = sorted.findIndex((e) => e[1] === sorted[idx][1]);
        return {position: firstIdx + 1, totalStudents: sorted.length};
    };

    generateReportForStudent = (student: any, callback?: () => void, mode: string = 'preview') => {
        this.isGenerating = true;
        this.applyExamGrading(this.currentEdLevelId());
        let studentId = +student.id;
        let fail = (err: any) => { this.isGenerating = false; this.bulkMode = false; this.toastr.error(err?.error || 'Error generating report.'); };

        let proceed = () => {
            forkJoin([
                this.studentSubjectsSvc.get(`/studentSubjects/byStudentId/${studentId}`),
                this.studentCoCurrActivitySvc.get(`/studentCoCurriculumActivities/byStudentId/${studentId}`)
            ]).subscribe({
                next: ([studentSubjects, coCurrActivities]) => {
                    let allocated = new Set<number>(((studentSubjects as any[]) || []).map((ss: any) => +ss.subjectId));
                    let scores = this.computeStudentScores(studentId, allocated);
                    let position = this.computeRank(this.shared.ranking, studentId);
                    this.buildAndPrintReport(student, scores, coCurrActivities as any[], position, callback, mode);
                },
                error: fail
            });
        };

        if (this.shared) proceed();
        else this.loadSharedData().subscribe({next: () => proceed(), error: fail});
    };

    private renderCbeItems = (items: {rating: string; desc: string}[]): any => {
        let list = (items || []).filter((i) => i.rating || i.desc);
        if (!list.length) return {text: '', fontSize: 9};
        if (this.cbeSectionDisplay === 'ratings') {
            return {text: list.map((i) => i.rating).filter(Boolean).join(', '), fontSize: 9};
        }
        let lines = list
            .map((i) => this.cbeSectionDisplay === 'descriptions' ? (i.desc || i.rating) : i.rating + (i.desc ? ' - ' + i.desc : ''))
            .filter(Boolean);
        return {stack: lines.map((l) => ({text: l, fontSize: 9, margin: [0, 0, 0, 1]}))};
    };

    private ageOf = (dob: any): string => {
        if (!dob) return '';
        let birth = new Date(dob);
        if (isNaN(birth.getTime()) || birth.getFullYear() < 1900) return '';
        let today = new Date();
        let age = today.getFullYear() - birth.getFullYear();
        if (today.getMonth() < birth.getMonth() || (today.getMonth() === birth.getMonth() && today.getDate() < birth.getDate())) age--;
        return age > 0 ? `${age} yrs` : '';
    };

    buildAndPrintReport = (student: any, scores: any, coCurrActivities: any[], position: any, callback?: () => void, mode: string = 'preview') => {
        this.reportSvc.loadImageAsBase64('assets/img/shule-nova-logo-only.png').subscribe({
            next: (blob) => {
                const reader = new FileReader();
                reader.onloadend = () => {
                    const base64data: string = reader.result as string;
                    let school = this.shared.schoolDetails;
                    let schoolClass = this.schoolClasses.find((sc) => sc.id == this.filterSchoolClassId);
                    let year = this.academicYears.find((y) => y.id == this.filterAcademicYearId);
                    let educationLevel = this.educationLevels.find((el) => el.id == this.currentEdLevelId());
                    let gender = this.genders.find((g) => +g.id === +student.genderId)?.name || '';
                    let className = schoolClass?.name || [schoolClass?.learningLevel?.name, schoolClass?.schoolStream?.name].filter(Boolean).join(' ');

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

                    // Learning areas: TERM 1..n and OVERALL, each as % and P.L.
                    let groups = [...this.sessions.map((s) => (s.sessionName || '').toUpperCase()), 'OVERALL'];
                    let headRow: any[] = [{text: 'LEARNING AREAS', style: 'tableHeader', rowSpan: 2}];
                    let subHeadRow: any[] = [{text: ''}];
                    groups.forEach((g) => {
                        headRow.push({text: g, style: 'tableHeader', colSpan: 2, alignment: 'center'}, {text: ''});
                        subHeadRow.push({text: '%', alignment: 'center', bold: true, fontSize: 8}, {text: 'P.L', alignment: 'center', bold: true, fontSize: 8});
                    });
                    let body: any[] = [headRow, subHeadRow];

                    // Per column (terms then overall): running total and subjects with marks.
                    let totals = groups.map(() => 0);
                    let counts = groups.map(() => 0);
                    let cell = (pct: number | null, extra: any = {}) => pct === null
                        ? [{text: '-', alignment: 'center', fontSize: 9, ...extra}, {text: '', fontSize: 9, ...extra}]
                        : [{text: `${pct}`, alignment: 'center', fontSize: 9, ...extra},
                           {text: this.gradeFor(pct)?.abbr || '', alignment: 'center', fontSize: 9, ...extra}];

                    let subjects = Object.values(scores).sort((a: any, b: any) => a.rank - b.rank) as any[];
                    subjects.forEach((subj) => {
                        let row: any[] = [{text: subj.name, fontSize: 9}];
                        [...subj.terms, subj.overall].forEach((pct: number | null, i: number) => {
                            row.push(...cell(pct, i === groups.length - 1 ? {bold: true} : {}));
                            if (pct !== null) { totals[i] += pct; counts[i]++; }
                        });
                        body.push(row);
                    });

                    let totalRow: any[] = [{text: 'Total Marks', bold: true, fontSize: 9}];
                    let avgRow: any[] = [{text: 'Average', bold: true, fontSize: 9}];
                    groups.forEach((_, i) => {
                        totalRow.push({text: counts[i] ? `${totals[i]}/${counts[i] * 100}` : '-', alignment: 'center', bold: true, fontSize: 9, colSpan: 2}, {text: ''});
                        avgRow.push(...cell(counts[i] ? Math.round(totals[i] / counts[i]) : null, {bold: true}));
                    });
                    body.push(totalRow, avgRow);

                    let overallIdx = groups.length - 1;
                    let overallAvg = counts[overallIdx] ? Math.round(totals[overallIdx] / counts[overallIdx]) : null;
                    let overallGrade = overallAvg !== null ? this.gradeFor(overallAvg) : null;

                    let label = (text: string) => ({text, fontSize: 9, bold: true, fillColor: '#e8f5e9'});
                    let summaryRows: any[] = [
                        [label('Total Marks (Overall):'),
                         {text: counts[overallIdx] ? `${totals[overallIdx]} out of ${counts[overallIdx] * 100}` : '-', fontSize: 10, bold: true}],
                        [label('Overall Average:'),
                         {text: overallAvg !== null ? `${overallAvg}%  ${overallGrade ? '- ' + overallGrade.name + ' (' + overallGrade.abbr + ')' : ''}` : '-', fontSize: 10, bold: true}]
                    ];
                    if (this.showPosition && position?.position > 0) {
                        summaryRows.push([label('Overall Position:'), {text: `${position.position} out of ${position.totalStudents}`, fontSize: 10, bold: true}]);
                    }
                    if (this.showCoCurricular) {
                        summaryRows.push([label('Co-Curricular Activities:'),
                            this.renderCbeItems((coCurrActivities || []).map((a) => ({rating: a.coCurriculumActivity?.name || '', desc: a.description || ''})))]);
                    }

                    let boxLayout = {
                        hLineWidth: () => 0.5, vLineWidth: () => 0.5,
                        hLineColor: () => '#aaa', vLineColor: () => '#aaa',
                        paddingLeft: () => 5, paddingRight: () => 5, paddingTop: () => 3, paddingBottom: () => 3
                    };
                    let reportTitle = `${(year?.name || '').toUpperCase()} ANNUAL SUMMATIVE REPORT FOR ${(educationLevel?.name || '').toUpperCase()}`;

                    const docDefinition: any = {
                        pageMargins: [25, 20, 25, 30],
                        pageSize: 'A4',
                        info: {
                            title: `Annual Report Card - ${student.fullName}`,
                            author: (this.userSvc?.currentUser?.firstName || '') + ' ' + (this.userSvc?.currentUser?.lastName || '')
                        },
                        footer: this.reportSvc.getFooter('portrait'),
                        images: {systemLogo: base64data, schoolLogo: school?.logoAsBase64},
                        styles: {
                            tableHeader: {bold: true, fontSize: 9, fillColor: '#d4edda', color: '#155724'}
                        },
                        content: [
                            {...this.reportSvc.getDIVIDER()},
                            this.reportSvc.getReportHeader(school),
                            {...this.reportSvc.getDIVIDER(), marginBottom: 1},
                            this.reportSvc.getReportTitle(reportTitle),
                            {...this.reportSvc.getDIVIDER(), marginBottom: 3},
                            // Learner details
                            {
                                layout: {
                                    hLineWidth: (i, node) => (i === 0 || i === node.table.body.length) ? 0.8 : 0.3,
                                    vLineWidth: (i, node) => (i === 0 || i === node.table.widths.length) ? 0.8 : 0,
                                    hLineColor: () => '#6fbf73',
                                    vLineColor: () => '#6fbf73',
                                    paddingLeft: () => 5, paddingRight: () => 3, paddingTop: () => 3, paddingBottom: () => 3
                                },
                                table: {
                                    widths: ['auto', '*', 'auto', 'auto', 'auto', 'auto'],
                                    body: [
                                        [
                                            {text: 'Name:', fontSize: 9, bold: true},
                                            {text: student.fullName || '', fontSize: 10, bold: true},
                                            {text: 'Adm No:', fontSize: 9, bold: true},
                                            {text: student.upi || '', fontSize: 10, bold: true},
                                            {text: 'Class:', fontSize: 9, bold: true},
                                            {text: className, fontSize: 10}
                                        ],
                                        [
                                            {text: 'Gender:', fontSize: 9, bold: true},
                                            {text: gender, fontSize: 10},
                                            {text: 'Age:', fontSize: 9, bold: true},
                                            {text: this.ageOf(student.dateOfBirth), fontSize: 10},
                                            {text: 'Class Teacher:', fontSize: 9, bold: true},
                                            {text: this.shared.classTeacherText || '..............................', fontSize: 9}
                                        ]
                                    ]
                                },
                                marginBottom: 8
                            },
                            gradingKeyTable,
                            {
                                layout: {hLineWidth: () => 0.5, vLineWidth: () => 0.5, hLineColor: () => '#aaa', vLineColor: () => '#aaa'},
                                table: {
                                    headerRows: 2,
                                    widths: ['*', ...groups.flatMap(() => [32, 30])],
                                    body
                                },
                                marginBottom: 8
                            },
                            {
                                layout: boxLayout,
                                table: {widths: [130, '*'], body: summaryRows},
                                marginBottom: 6
                            },
                            // Signatures
                            {
                                layout: {...boxLayout, paddingTop: () => 6, paddingBottom: () => 6},
                                table: {
                                    widths: ['*', '*', '*'],
                                    body: [
                                        [
                                            {text: "Class Teacher's Signature:", fontSize: 9, bold: true, fillColor: '#e8f5e9'},
                                            {text: "Head Teacher's Signature:", fontSize: 9, bold: true, fillColor: '#e8f5e9'},
                                            {text: "Parent / Guardian's Signature:", fontSize: 9, bold: true, fillColor: '#e8f5e9'}
                                        ],
                                        [0, 1, 2].map(() => ({text: '\n\nDate: ..............................', fontSize: 9}))
                                    ]
                                }
                            },
                            {
                                text: `This is a system generated document. Printed on ${new Date().toLocaleString('en-GB')}`,
                                fontSize: 8, color: '#999999', italics: true, alignment: 'center', marginTop: 8
                            }
                        ]
                    };

                    if (this.bulkMode) {
                        this.bulkDocs.push(docDefinition);
                        if (callback) callback();
                        return;
                    }
                    this.outputPdf(docDefinition, mode, () => {
                        if (!callback) this.isGenerating = false;
                        if (callback) callback();
                    });
                };
                reader.readAsDataURL(blob);
            },
            error: () => { this.isGenerating = false; this.bulkMode = false; this.toastr.error('Error loading logo.'); }
        });
    };
}
