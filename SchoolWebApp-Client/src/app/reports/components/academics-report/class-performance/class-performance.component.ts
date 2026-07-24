import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of, switchMap, map} from 'rxjs';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {EducationLevelTypesService} from '@/school/services/education-level-types.service';
import {GradesService} from '@/academics/services/grades.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
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

interface ClassRow {
    rank: number;
    className: string;
    classRank: number;
    studentCount: number;
    method: string;        // 'mean_marks' | 'mean_points'
    classValue: number;    // mean marks % or mean points, per the method
    classGrade: string;
    topStudent: string;
    classTeacher: string;
    educationLevelId: any; // used to group classes for ranking
}

// A ranking group (an education level type or an education level, per the
// ClassPerformanceRankingBasis setting). Rank restarts at 1 within each group.
interface ClassGroup {
    key: any;
    name: string;
    order: number;
    rows: ClassRow[];
}

@Component({
    selector: 'app-class-performance',
    templateUrl: './class-performance.component.html',
    styleUrl: './class-performance.component.scss'
})
export class ClassPerformanceComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/academics/class-performance'], title: 'Academics: Class Performance'}
    ];
    dashboardTitle = 'Academics: Class Performance';

    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    learningLevels: any[] = [];
    examTypes: any[] = [];
    schoolExams: any[] = [];
    allGrades: any[] = [];
    gradingSettings: any[] = [];
    averageMethod: string = 'students_with_scores';

    // Ranking-grouping basis + the level lookups used to resolve each class's
    // group. 'education_level_type' (default) ranks within each education level
    // type; 'education_level' ranks within each education level.
    rankingBasis: string = 'education_level_type';
    educationLevels: any[] = [];
    educationLevelTypes: any[] = [];
    groups: ClassGroup[] = [];

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterSchoolExamId: any = null;
    filterExamTypeId: any = null;

    classRows: ClassRow[] = [];
    loaded: boolean = false;
    isLoading: boolean = false;

    constructor(
        private toastr: ToastrService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private schoolClassesSvc: SchoolClassesService,
        private learningLevelSvc: LearningLevelsService,
        private educationLevelSvc: EducationLevelService,
        private educationLevelTypeSvc: EducationLevelTypesService,
        private gradesSvc: GradesService,
        private globalSettingSvc: GlobalSettingService,
        private examSvc: ExamService,
        private examTypeSvc: ExamTypeService,
        private schoolExamSvc: SchoolExamService,
        private examResultSvc: ExamResultService,
        private studentClassSvc: StudentClassService,
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
            this.globalSettingSvc.getByKey('General', 'AverageCalculation'),
            this.educationLevelSvc.get('/educationLevels'),
            this.educationLevelTypeSvc.get('/educationLevelTypes')
        ]).subscribe({
            next: ([curricula, academicYears, examTypes, allGrades, gradingSettings, avgSetting, edLevels, edLevelTypes]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort((a, b) => b.rank - a.rank);
                this.examTypes = examTypes.sort((a, b) => a.rank - b.rank);
                this.allGrades = allGrades;
                this.gradingSettings = (gradingSettings as any[]) || [];
                this.averageMethod = (avgSetting as any)?.settingValue || 'students_with_scores';
                this.educationLevels = (edLevels as any[]) || [];
                this.educationLevelTypes = (edLevelTypes as any[]) || [];
                this.rankingBasis = this.settingVal('ClassPerformanceRankingBasis') || 'education_level_type';
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    private settingVal = (key: string): string =>
        this.gradingSettings.find((s) => s.settingKey === key)?.settingValue || '';

    // Effective exam-results grading category for an education level.
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

    // Closest grade for a points value (exact match preferred).
    private gradeForPoints = (points: number, grades: any[]): any => {
        let g = grades.find((x) => x.points == points);
        if (g) return g;
        if (!grades.length) return null;
        return grades.reduce((p, c) => (Math.abs(c.points - points) < Math.abs(p.points - points) ? c : p));
    };

    // Resolve the ranking method for an education level: the level's own
    // override if set, else the global default, else mean_marks.
    private rankingMethodFor = (edLevelId: any): string => {
        let globalVal = this.settingVal('RankingMethod') || 'mean_marks';
        if (!edLevelId) return globalVal;
        return this.settingVal(`RankingMethod:${edLevelId}`) || globalVal;
    };

    // Max tied top students to list per class before "+N more". '0' = All.
    private get classTopStudentsCap(): number {
        let v = this.settingVal('ClassTopStudentsCap');
        if (v === '0') return Infinity;
        let n = parseInt(v, 10);
        return isNaN(n) || n < 1 ? 3 : n;
    }

    onFilterChange = () => { this.loaded = false; this.classRows = []; };

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

    // Build one class's summary row: class average, grade, top student (total
    // marks) and the class teacher / CEO.
    private computeClassRow = (cls: any, exams: any[], studentClasses: any[], classLeaders: any[], allResults: any[]): ClassRow => {
        let edLevelId = cls?.learningLevel?.educationLevelId
            ?? this.learningLevels.find((ll) => +ll.id === +(cls?.learningLevelId))?.educationLevelId;
        let grades = this.allGrades
            .filter((g) => g.category === this.examGradingCategoryFor(edLevelId))
            .sort((a, b) => a.rank - b.rank);
        let method = this.rankingMethodFor(edLevelId);
        let usePoints = method === 'mean_points';
        let studentCount = (studentClasses || []).filter((sc) => sc.student).length;

        let studentById = new Map<number, any>();
        (studentClasses || []).forEach((sc) => { if (sc.student) studentById.set(+sc.student.id, sc.student); });

        // Per-student aggregates: sum of %, sum of points, and subject count.
        let agg = new Map<number, {sumPct: number; sumPoints: number; count: number}>();
        exams.forEach((exam, idx) => {
            let results = (allResults[idx] as any[]) || [];
            let examMark = exam.examMark || 100;
            results.forEach((r) => {
                if (r.score == null) return;
                let a = agg.get(+r.studentId) || {sumPct: 0, sumPoints: 0, count: 0};
                let pct = examMark > 0 ? (r.score / examMark) * 100 : 0;
                a.sumPct += pct;
                a.sumPoints += this.pointsForPercent(Math.round(pct), grades);
                a.count++;
                agg.set(+r.studentId, a);
            });
        });

        let entries = [...agg.entries()];
        // Each student's metric per the ranking method (mean marks % or mean points).
        let studentMetric = (a: any) => (a.count > 0 ? (usePoints ? a.sumPoints / a.count : a.sumPct / a.count) : 0);

        // Class value = mean of student metrics. Under "all allocated", students
        // with no marks count as 0 (divide by class size).
        let sumMetric = entries.reduce((s, [, a]) => s + studentMetric(a), 0);
        let classValue = 0;
        if (this.averageMethod === 'all_allocated_students' && studentCount > 0) {
            classValue = sumMetric / studentCount;
        } else if (entries.length > 0) {
            classValue = sumMetric / entries.length;
        }
        classValue = Math.round(classValue * 10) / 10;
        let gradeObj = usePoints
            ? this.gradeForPoints(Math.round(classValue), grades)
            : this.gradeForPercent(Math.round(classValue), grades);
        let classGrade = gradeObj?.abbr || '';

        // Top student(s) by the same ranking metric; bracket shows that value.
        let topStudent = '';
        if (entries.length) {
            let metricRounded = (a: any) => Math.round(studentMetric(a) * 10) / 10;
            let maxMetric = Math.max(...entries.map(([, a]) => metricRounded(a)));
            let tops = entries.filter(([, a]) => metricRounded(a) === maxMetric);
            let names = tops
                .map(([sid]) => {
                    let st = studentById.get(+sid);
                    return `${st?.upi ? st.upi + '-' : ''}${st?.fullName || ''}`;
                })
                .sort((a, b) => a.localeCompare(b));
            let CAP = this.classTopStudentsCap;
            let shown = names.slice(0, CAP).join(', ');
            if (names.length > CAP) shown += ` +${names.length - CAP} more`;
            topStudent = `${shown} (${maxMetric}${usePoints ? '' : '%'})`;
        }

        // Class teacher / CEO: the class leaders whose role is a Teacher type.
        let teacherLeaders = (classLeaders || []).filter(
            (cl) => cl.classLeadershipRole?.personType === 1 || cl.classLeadershipRole?.personType === 'Teacher'
        );
        let classTeacher = teacherLeaders
            .map((cl) => `${cl.person?.fullName || ''} [${cl.classLeadershipRole?.name || ''}]`)
            .filter(Boolean)
            .join(', ');

        return {
            rank: 0,
            className: cls.name || '',
            classRank: cls.rank ?? 0,
            studentCount,
            method,
            classValue,
            classGrade,
            topStudent,
            classTeacher,
            educationLevelId: edLevelId ?? null
        };
    };

    // Partition the class rows into ranking groups per the configured basis,
    // rank within each group (desc by class value, ties share a rank), and
    // order the groups by the education level / level-type rank.
    private buildGroups = (list: ClassRow[]): ClassGroup[] => {
        let byType = this.rankingBasis === 'education_level_type';
        let elById = new Map<number, any>();
        this.educationLevels.forEach((el) => elById.set(+el.id, el));
        let typeById = new Map<number, any>();
        this.educationLevelTypes.forEach((t) => typeById.set(+t.id, t));

        // Resolve a row's group key/name/order from its education level.
        let groupOf = (r: ClassRow): {key: any; name: string; order: number} => {
            let el = r.educationLevelId != null ? elById.get(+r.educationLevelId) : null;
            if (byType) {
                let t = el ? typeById.get(+el.educationLevelTypeId) : null;
                return t
                    ? {key: `t${t.id}`, name: t.name || 'Unassigned', order: t.rank ?? 9999}
                    : {key: 'none', name: 'Unassigned', order: 9999};
            }
            return el
                ? {key: `l${el.id}`, name: el.name || 'Unassigned', order: el.rank ?? 9999}
                : {key: 'none', name: 'Unassigned', order: 9999};
        };

        let groupMap = new Map<any, ClassGroup>();
        list.forEach((r) => {
            let g = groupOf(r);
            let existing = groupMap.get(g.key);
            if (!existing) {
                existing = {key: g.key, name: g.name, order: g.order, rows: []};
                groupMap.set(g.key, existing);
            }
            existing.rows.push(r);
        });

        let groups = [...groupMap.values()];
        groups.forEach((g) => {
            g.rows.sort((a, b) => b.classValue - a.classValue);
            let currentRank = 1;
            g.rows.forEach((r, i) => {
                if (i > 0 && Math.round(r.classValue * 10) === Math.round(g.rows[i - 1].classValue * 10)) {
                    r.rank = g.rows[i - 1].rank;
                } else {
                    r.rank = currentRank;
                }
                currentRank = i + 2;
            });
        });
        // Groups ordered by the level / level-type rank, then name.
        groups.sort((a, b) => (a.order - b.order) || a.name.localeCompare(b.name));
        return groups;
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
        this.classRows = [];
        this.groups = [];

        // Re-read the Grading settings on every Load so a change made in another
        // window (ranking basis, or per-level ranking method) takes effect on the
        // next Load without a full page refresh. Falls back to the values read at
        // init if the fetch fails.
        this.globalSettingSvc.getByModule('Grading').subscribe({
            next: (gradingSettings) => {
                this.gradingSettings = (gradingSettings as any[]) || this.gradingSettings;
                this.rankingBasis = this.settingVal('ClassPerformanceRankingBasis') || 'education_level_type';
                this.runReportLoad();
            },
            error: () => this.runReportLoad()
        });
    };

    private runReportLoad = () => {
        let requests = this.schoolClasses.map((cls) => {
            let url = `/exams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${this.filterSessionId}&schoolClassId=${cls.id}&examTypeId=${this.filterExamTypeId}`;
            return forkJoin([
                this.examSvc.get(url),
                this.studentClassSvc.getBySchoolClassId(cls.id, Status.Active),
                this.schoolClassesSvc.get(`/schoolClassLeaders/bySchoolClassId/${cls.id}`)
            ]).pipe(
                switchMap(([exams, studentClasses, classLeaders]) => {
                    if (!exams.length) return of(null);
                    let resultReqs = exams.map((e) => this.examResultSvc.get(`/examResults/byExamId/${e.id}`));
                    return forkJoin(resultReqs).pipe(
                        map((allResults) => this.computeClassRow(cls, exams, studentClasses, classLeaders, allResults))
                    );
                })
            );
        });

        forkJoin(requests).subscribe({
            next: (rows) => {
                let list = (rows.filter(Boolean) as ClassRow[]);
                // Group by the configured basis and rank within each group.
                this.groups = this.buildGroups(list);
                this.classRows = list; // flat list retained for counts / print guard
                this.loaded = true;
                this.isLoading = false;
                if (!this.classRows.length) this.toastr.info('No exam results found for this selection.');
            },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error); }
        });
    };

    // Label describing the current grouping basis, shown in the report header
    // and print subtitle.
    get rankingBasisLabel(): string {
        return this.rankingBasis === 'education_level'
            ? 'ranked within each education level'
            : 'ranked within each education level type';
    }

    printReport = () => {
        if (!this.classRows.length) {
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
                            let reportTitle = `CLASS PERFORMANCE SUMMARY - ${this.getFilterName('year')} ${this.getFilterName('session')} - ${this.getFilterName('examType')}`.toUpperCase();

                            let body: any[] = [[
                                {text: 'Rank', style: 'tableHeader', alignment: 'center'},
                                {text: 'Class', style: 'tableHeader'},
                                {text: 'Students', style: 'tableHeader', alignment: 'center'},
                                {text: 'Class Avg', style: 'tableHeader', alignment: 'center'},
                                {text: 'Grade', style: 'tableHeader', alignment: 'center'},
                                {text: 'Top Student', style: 'tableHeader'},
                                {text: 'Class Teacher / CEO', style: 'tableHeader'}
                            ]];
                            // One section per ranking group, with a spanning
                            // subheading row and the group's ranked classes.
                            this.groups.forEach((g) => {
                                body.push([
                                    {text: `${g.name}  (${g.rows.length} class${g.rows.length === 1 ? '' : 'es'})`,
                                        colSpan: 7, bold: true, fontSize: 9, fillColor: '#e9ecef', margin: [0, 1, 0, 1]},
                                    {}, {}, {}, {}, {}, {}
                                ]);
                                g.rows.forEach((c) => body.push([
                                    {text: c.rank, alignment: 'center', bold: true, fontSize: 9},
                                    {text: c.className, fontSize: 9},
                                    {text: c.studentCount, alignment: 'center', fontSize: 9},
                                    {text: c.classValue + (c.method === 'mean_points' ? ' pts' : '%'), alignment: 'center', bold: true, fontSize: 9},
                                    {text: c.classGrade, alignment: 'center', bold: true, fontSize: 9},
                                    {text: c.topStudent, fontSize: 9},
                                    {text: c.classTeacher, fontSize: 9}
                                ]));
                            });

                            let content: any[] = [
                                {...this.reportSvc.getDIVIDER('landscape')},
                                this.reportSvc.getReportHeader(school[0]),
                                {...this.reportSvc.getDIVIDER('landscape'), marginBottom: 1},
                                this.reportSvc.getReportTitle(reportTitle),
                                {text: `Classes ${this.rankingBasisLabel} (mean marks or mean points, per education level)`, alignment: 'center', italics: true, fontSize: 8, color: '#555', marginBottom: 2},
                                {...this.reportSvc.getDIVIDER('landscape'), marginBottom: 2},
                                {
                                    layout: this.reportSvc.getTableLayout(),
                                    table: {headerRows: 1, widths: ['auto', 'auto', 'auto', 'auto', 'auto', '*', '*'], body},
                                    fontSize: 9, marginBottom: 4
                                },
                                {...this.reportSvc.getDIVIDER('landscape')},
                                this.reportSvc.getPrintDetails(
                                    (this.userSvc?.currentUser?.firstName || '') + ' ' + (this.userSvc?.currentUser?.lastName || ''),
                                    new Date().toLocaleString('en-GB')
                                )
                            ];

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
