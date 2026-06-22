import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of, switchMap, map} from 'rxjs';
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
import {StaffSubjectsService} from '@/staff/services/staff-subjects.service';
import {AuthService} from '@/core/services/auth.service';
import {ReportsService} from '@/reports/services/reports.service';
import {Status} from '@/core/enums/status';

import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
(pdfMake as any).vfs = pdfFonts;

interface SubjectRow {
    rank: number;
    name: string;
    abbr: string;
    meanMark: number;
    meanPoints: number;
    grade: string;
    entries: number;
    topStudent: string;
    teacher: string;
}
interface ClassReport {
    className: string;
    studentCount: number;
    subjects: SubjectRow[];
}

@Component({
    selector: 'app-subject-performance',
    templateUrl: './subject-performance.component.html',
    styleUrl: './subject-performance.component.scss'
})
export class SubjectPerformanceComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/academics/subject-performance'], title: 'Academics: Subject Performance'}
    ];
    dashboardTitle = 'Academics: Subject Performance';

    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    learningLevels: any[] = [];
    examTypes: any[] = [];
    schoolExams: any[] = [];
    allGrades: any[] = [];
    gradingSettings: any[] = [];
    rankingMethod: string = 'mean_marks';
    averageMethod: string = 'students_with_scores';

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterSchoolExamId: any = null;
    filterExamTypeId: any = null;

    classReports: ClassReport[] = [];
    loaded: boolean = false;
    isLoading: boolean = false;

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
        private staffSubjectsSvc: StaffSubjectsService,
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
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort((a, b) => b.rank - a.rank);
                this.examTypes = examTypes.sort((a, b) => a.rank - b.rank);
                this.allGrades = allGrades;
                this.gradingSettings = (gradingSettings as any[]) || [];
                this.rankingMethod = this.settingVal('RankingMethod') || 'mean_marks';
                this.averageMethod = (avgSetting as any)?.settingValue || 'students_with_scores';
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    get rankBasisLabel(): string {
        return this.rankingMethod === 'mean_points' ? 'Mean Points' : 'Mean Marks (%)';
    }

    private settingVal = (key: string): string =>
        this.gradingSettings.find((s) => s.settingKey === key)?.settingValue || '';

    // Max tied top students to list per subject before "+N more". '0' = All.
    private get topStudentsCap(): number {
        let v = this.settingVal('SubjectTopStudentsCap');
        if (v === '0') return Infinity;
        let n = parseInt(v, 10);
        return isNaN(n) || n < 1 ? 3 : n;
    }

    // Effective exam-results grading category for an education level: the level's
    // own override if set, else the global default, else 4-Point.
    private examGradingCategoryFor = (edLevelId: any): string => {
        let globalVal = this.settingVal('ExamResults') || '4-Point';
        if (!edLevelId) return globalVal;
        return this.settingVal(`ExamResults:${edLevelId}`) || globalVal;
    };

    private gradeForPercent = (percent: number, grades: any[]): any =>
        grades.find((g) => percent >= g.minScore && percent <= g.maxScore);

    private pointsForPercent = (percent: number, grades: any[]): number => {
        let g = this.gradeForPercent(percent, grades);
        return g ? g.points : 0;
    };

    onFilterChange = () => { this.loaded = false; this.classReports = []; };

    onCurriculumChange = () => {
        this.sessions = this.schoolClasses = this.schoolExams = [];
        this.filterAcademicYearId = this.filterSessionId = this.filterSchoolExamId = this.filterExamTypeId = null;
        this.onFilterChange();
        if (!this.filterCurriculumId) return;
        this.learningLevelSvc.getLearningLevelsByCurriculum(this.filterCurriculumId).subscribe({
            next: (levels) => { this.learningLevels = levels.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onAcademicYearChange = () => {
        this.sessions = this.schoolClasses = this.schoolExams = [];
        this.filterSessionId = this.filterSchoolExamId = this.filterExamTypeId = null;
        this.onFilterChange();
        if (!this.filterAcademicYearId || !this.filterCurriculumId) return;
        forkJoin([
            this.sessionsSvc.get(`/sessions/byCurriculumYearId?curriculumId=${this.filterCurriculumId}&academicYearId=${this.filterAcademicYearId}`),
            this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${this.filterAcademicYearId}`)
        ]).subscribe({
            next: ([sessions, schoolClasses]) => {
                this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                let currLLIds = this.learningLevels.map((ll) => +ll.id);
                this.schoolClasses = schoolClasses
                    .filter((sc) => currLLIds.includes(+sc.learningLevelId))
                    .sort((a, b) => (a.rank || 0) - (b.rank || 0));
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

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

    getFilterName = (type: string): string => {
        if (type === 'year') return this.academicYears.find((y) => y.id == this.filterAcademicYearId)?.name || '';
        if (type === 'session') return this.sessions.find((s) => s.id == this.filterSessionId)?.sessionName || '';
        if (type === 'examType') return this.examTypes.find((et) => et.id == this.filterExamTypeId)?.name || '';
        return '';
    };

    // Build one class's ranked subject summary from its exams + results.
    private computeClassReport = (cls: any, exams: any[], studentClasses: any[], staffSubjects: any[], allResults: any[]): ClassReport => {
        let edLevelId = cls?.learningLevel?.educationLevelId
            ?? this.learningLevels.find((ll) => +ll.id === +(cls?.learningLevelId))?.educationLevelId;
        let grades = this.allGrades
            .filter((g) => g.category === this.examGradingCategoryFor(edLevelId))
            .sort((a, b) => a.rank - b.rank);
        let studentCount = (studentClasses || []).filter((sc) => sc.student).length;

        // studentId -> name, for the top-performer lookup per subject.
        let studentById = new Map<number, any>();
        (studentClasses || []).forEach((sc) => { if (sc.student) studentById.set(+sc.student.id, sc.student); });

        // subjectId -> allocated teacher name(s) in this class.
        let teacherBySubject = new Map<number, string>();
        (staffSubjects || []).forEach((ss) => {
            let name = ss.staffDetails?.fullName || '';
            if (!name) return;
            let existing = teacherBySubject.get(+ss.subjectId);
            teacherBySubject.set(+ss.subjectId, existing ? `${existing}, ${name}` : name);
        });

        let subjects: SubjectRow[] = exams.map((exam, idx) => {
            let results = (allResults[idx] as any[]) || [];
            let scored = results.filter((r) => r.score != null);
            let examMark = exam.examMark || 100;
            let sumPct = 0, sumPoints = 0;
            scored.forEach((r) => {
                let pct = examMark > 0 ? (r.score / examMark) * 100 : 0;
                sumPct += pct;
                // Round the mark to a whole number before the points lookup.
                sumPoints += this.pointsForPercent(Math.round(pct), grades);
            });
            // Mirror the broadsheet/dashboard averaging policy.
            let divisor = this.averageMethod === 'all_allocated_students'
                ? (studentCount || scored.length) : scored.length;
            let meanMark = divisor > 0 ? Math.round((sumPct / divisor) * 10) / 10 : 0;
            let meanPoints = divisor > 0 ? Math.round((sumPoints / divisor) * 10) / 10 : 0;
            // Round the mean mark to a whole number before looking up the grade.
            let grade = this.gradeForPercent(Math.round(meanMark), grades);

            // Top student(s) = everyone sharing the highest raw score for this
            // subject, shown as "ADM-Name, ADM-Name (xx%)". Ties list all who
            // share the top score; capped so the cell stays readable.
            let topStudent = '';
            if (scored.length) {
                let maxScore = Math.max(...scored.map((r) => +r.score));
                let topNames = scored
                    .filter((r) => +r.score === maxScore)
                    .map((r) => {
                        let st = studentById.get(+r.studentId);
                        return `${st?.upi ? st.upi + '-' : ''}${st?.fullName || ''}`;
                    })
                    .sort((a, b) => a.localeCompare(b));
                let topPct = examMark > 0 ? Math.round((maxScore / examMark) * 100) : maxScore;
                let CAP = this.topStudentsCap;
                let shown = topNames.slice(0, CAP).join(', ');
                if (topNames.length > CAP) shown += ` +${topNames.length - CAP} more`;
                topStudent = `${shown} (${topPct}%)`;
            }

            return {
                rank: 0,
                name: exam.subject?.name || '',
                abbr: exam.subject?.abbr || exam.subject?.name || '',
                meanMark, meanPoints,
                grade: grade?.abbr || '',
                entries: scored.length,
                topStudent,
                teacher: teacherBySubject.get(+exam.subjectId) || ''
            };
        });

        // Rank subjects by the configured ranking method, ties share a rank.
        let rankField: 'meanMark' | 'meanPoints' = this.rankingMethod === 'mean_points' ? 'meanPoints' : 'meanMark';
        let rv = (s: SubjectRow) => Math.round((s[rankField] || 0) * 10);
        subjects.sort((a, b) => rv(b) - rv(a));
        let currentRank = 1;
        subjects.forEach((s, i) => {
            if (i > 0 && rv(s) === rv(subjects[i - 1])) s.rank = subjects[i - 1].rank;
            else s.rank = currentRank;
            currentRank = i + 2;
        });

        return {className: cls.name || '', studentCount, subjects};
    };

    loadReport = () => {
        if (!this.filterSessionId || !this.filterSchoolExamId) {
            this.toastr.info('Please select Session and School Exam.');
            return;
        }
        if (!this.schoolClasses.length) {
            this.toastr.info('No classes found for the selected year/curriculum.');
            return;
        }

        this.isLoading = true;
        this.loaded = false;
        this.classReports = [];

        let requests = this.schoolClasses.map((cls) => {
            let url = `/exams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${this.filterSessionId}&schoolClassId=${cls.id}&examTypeId=${this.filterExamTypeId}`;
            return forkJoin([
                this.examSvc.get(url),
                this.studentClassSvc.getBySchoolClassId(cls.id, Status.Active),
                this.staffSubjectsSvc.get(`/staffSubjects/bySchoolClassId/${cls.id}`)
            ]).pipe(
                switchMap(([exams, studentClasses, staffSubjects]) => {
                    if (!exams.length) return of(null);
                    let resultReqs = exams.map((e) => this.examResultSvc.get(`/examResults/byExamId/${e.id}`));
                    return forkJoin(resultReqs).pipe(
                        map((allResults) => this.computeClassReport(cls, exams, studentClasses, staffSubjects, allResults))
                    );
                })
            );
        });

        forkJoin(requests).subscribe({
            next: (reports) => {
                this.classReports = (reports.filter(Boolean) as ClassReport[])
                    .filter((r) => r.subjects.length > 0);
                this.loaded = true;
                this.isLoading = false;
                if (!this.classReports.length) this.toastr.info('No exam results found for this selection.');
            },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error); }
        });
    };

    printReport = () => {
        if (!this.classReports.length) {
            this.toastr.info('Load the report first.');
            return;
        }
        this.schoolSvc.get('/schooldetails').subscribe({
            next: (school) => {
                this.reportSvc.loadImageAsBase64('assets/img/shule-nova-logo-only.png').subscribe({
                    next: (blob) => {
                        const reader = new FileReader();
                        reader.onloadend = () => {
                            const base64data: string = reader.result as string;
                            let reportTitle = `SUBJECT PERFORMANCE - ${this.getFilterName('year')} ${this.getFilterName('session')} - ${this.getFilterName('examType')}`.toUpperCase();

                            let content: any[] = [
                                {...this.reportSvc.getDIVIDER('landscape')},
                                this.reportSvc.getReportHeader(school[0]),
                                {...this.reportSvc.getDIVIDER('landscape'), marginBottom: 1},
                                this.reportSvc.getReportTitle(reportTitle),
                                {text: `Subjects ranked by ${this.rankBasisLabel}`, alignment: 'center', italics: true, fontSize: 8, color: '#555', marginBottom: 2},
                                {...this.reportSvc.getDIVIDER('landscape'), marginBottom: 2}
                            ];

                            this.classReports.forEach((cr) => {
                                content.push({
                                    text: `${cr.className}   (${cr.studentCount} learners)`,
                                    bold: true, fontSize: 10, color: '#002D62', marginTop: 6, marginBottom: 2
                                });
                                let body: any[] = [[
                                    {text: 'Rank', style: 'tableHeader', alignment: 'center'},
                                    {text: 'Subject', style: 'tableHeader'},
                                    {text: 'Entries', style: 'tableHeader', alignment: 'center'},
                                    {text: 'Mean %', style: 'tableHeader', alignment: 'center'},
                                    {text: 'Grade', style: 'tableHeader', alignment: 'center'},
                                    {text: 'Mean Pts', style: 'tableHeader', alignment: 'center'},
                                    {text: 'Top Student', style: 'tableHeader'},
                                    {text: 'Subject Teacher', style: 'tableHeader'}
                                ]];
                                cr.subjects.forEach((s) => body.push([
                                    {text: s.rank, alignment: 'center', bold: true, fontSize: 8},
                                    {text: s.name, fontSize: 8},
                                    {text: s.entries, alignment: 'center', fontSize: 8},
                                    {text: s.meanMark + '%', alignment: 'center', fontSize: 8},
                                    {text: s.grade, alignment: 'center', bold: true, fontSize: 8},
                                    {text: s.meanPoints, alignment: 'center', fontSize: 8},
                                    {text: s.topStudent, fontSize: 8},
                                    {text: s.teacher, fontSize: 8}
                                ]));
                                content.push({
                                    layout: this.reportSvc.getTableLayout(),
                                    table: {headerRows: 1, widths: ['auto', '*', 'auto', 'auto', 'auto', 'auto', '*', '*'], body},
                                    fontSize: 8, marginBottom: 4, unbreakable: true
                                });
                            });

                            content.push(
                                {...this.reportSvc.getDIVIDER('landscape')},
                                this.reportSvc.getPrintDetails(
                                    (this.userSvc?.currentUser?.firstName || '') + ' ' + (this.userSvc?.currentUser?.lastName || ''),
                                    new Date().toLocaleString('en-GB')
                                )
                            );

                            const docDefinition: any = {
                                pageSize: 'A4',
                                pageOrientation: 'landscape',
                                pageMargins: [15, 15, 15, 30],
                                info: {
                                    title: reportTitle,
                                    author: (this.userSvc?.currentUser?.firstName || '') + ' ' + (this.userSvc?.currentUser?.lastName || ''),
                                    subject: reportTitle
                                },
                                watermark: this.reportSvc.getWatermark('ShuleNova - ' + school[0]?.name),
                                footer: this.reportSvc.getFooter('landscape'),
                                images: {systemLogo: base64data, schoolLogo: school[0]?.logoAsBase64},
                                styles: {tableHeader: this.reportSvc.getHEADER_STYLE()},
                                content
                            };

                            pdfMake.createPdf(docDefinition).getBlob((pdfBlob) => {
                                window.open(URL.createObjectURL(pdfBlob), '_blank');
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
}
