import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {EducationLevelSubjectService} from '@/academics/services/education-level-subject.service';
import {GradesService} from '@/academics/services/grades.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {StrandService} from '@/cbe/assessments/services/strand.service';
import {SubStrandService} from '@/cbe/assessments/services/sub-strand.service';
import {SpecificOutcomeService} from '@/cbe/assessments/services/specific-outcome.service';
import {StudentAssessmentService} from '@/cbe/assessments/services/student-assessment.service';
import {AuthService} from '@/core/services/auth.service';
import {ReportsService} from '@/reports/services/reports.service';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
import {Status} from '@/core/enums/status';

import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
(pdfMake as any).vfs = pdfFonts;

@Component({
    selector: 'app-assessment-report',
    templateUrl: './assessment-report.component.html',
    styleUrl: './assessment-report.component.scss'
})
export class AssessmentReportComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/academics/assessment-report'], title: 'Academics: Assessment Report'}
    ];
    dashboardTitle = 'Academics: CBE Assessment Report';

    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    grades: any[] = [];
    learningLevels: any[] = [];
    educationLevels: any[] = [];
    subjects: any[] = [];
    gradingCategory: string = '4-Point';
    gradingCategories: string[] = ['4-Point', '8-Point'];
    selectedGradingCategory: string = '4-Point';
    allGrades: any[] = [];

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterSchoolClassId: any = null;

    isLoading: boolean = false;
    isGenerating: boolean = false;
    studentsLoaded: boolean = false;

    // When printing several students, each report's content is collected here
    // and emitted as ONE PDF (one tab / one print job) instead of one per
    // student. bulkMode tells the per-student builder to collect rather than
    // render immediately.
    private bulkMode: boolean = false;
    private bulkDocs: any[] = [];

    // Client-side paging for the students table.
    page: number = 1;
    pageSize: number = 20;

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
        private educationLevelSubjectSvc: EducationLevelSubjectService,
        private gradesSvc: GradesService,
        private globalSettingSvc: GlobalSettingService,
        private schoolSvc: SchoolDetailsService,
        private studentClassSvc: StudentClassService,
        private strandSvc: StrandService,
        private subStrandSvc: SubStrandService,
        private specificOutcomeSvc: SpecificOutcomeService,
        private studentAssessmentSvc: StudentAssessmentService,
        private userSvc: AuthService,
        private reportSvc: ReportsService,
        private studentSubjectsSvc: StudentSubjectsService
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.academicYearSvc.get('/academicYears'),
            this.gradesSvc.get('/grades'),
            this.globalSettingSvc.getByKey('Grading', 'StudentAssessment')
        ]).subscribe({
            next: ([curricula, academicYears, allGrades, gradingSetting]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.sort((a, b) => b.rank - a.rank);
                let settingResponse = gradingSetting as any;
                this.gradingCategory = settingResponse?.settingValue || '4-Point';
                this.selectedGradingCategory = this.gradingCategory;
                this.allGrades = allGrades;
                this.grades = allGrades.filter(g => g.category === this.gradingCategory).sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    onGradingCategoryChange = () => {
        this.grades = this.allGrades.filter(g => g.category === this.selectedGradingCategory).sort((a, b) => a.rank - b.rank);
    };

    onCurriculumChange = () => {
        this.sessions = this.schoolClasses = this.subjects = [];
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
        this.sessions = this.schoolClasses = this.subjects = [];
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
        this.subjects = [];
        this.studentsLoaded = false;
        if (!this.filterSchoolClassId || !this.filterAcademicYearId) return;
        let selectedClass = this.schoolClasses.find((sc) => sc.id == this.filterSchoolClassId);
        if (!selectedClass?.learningLevel?.educationLevelId) return;
        this.educationLevelSubjectSvc
            .getByEducationLevelAndAcademicYear(selectedClass.learningLevel.educationLevelId, this.filterAcademicYearId)
            .subscribe({
                next: (elSubjects) => {
                    this.subjects = elSubjects.map((es) => es.subject).sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
    };

    loadStudents = () => {
        if (!this.filterSessionId || !this.filterSchoolClassId) {
            this.toastr.info('Please select Session and Class.');
            return;
        }
        this.isLoading = true;
        this.studentsLoaded = false;
        this.studentClassSvc.getBySchoolClassId(this.filterSchoolClassId, Status.Active).subscribe({
            next: (studentClasses) => {
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

    pageChanged = (page: number) => { this.page = page; };
    pageSizeChanged = (pageSize: number) => { this.pageSize = pageSize; this.page = 1; };

    toggleSelectAll = () => {
        let all = this.studentRows.every((r) => r.selected);
        this.studentRows.forEach((r) => r.selected = !all);
    };
    allSelected = (): boolean => this.studentRows.length > 0 && this.studentRows.every((r) => r.selected);
    getSelectedCount = (): number => this.studentRows.filter((r) => r.selected).length;

    previewStudent = (row: any, mode: string = 'preview') => { this.bulkMode = false; this.generateForStudent(row.student, null, mode); };

    printSelected = (mode: string = 'preview') => {
        let selected = this.studentRows.filter((r) => r.selected);
        if (selected.length === 0) { this.toastr.info('Select at least one student.'); return; }
        this.isGenerating = true;
        this.bulkMode = true;
        this.bulkDocs = [];
        let idx = 0;
        let next = () => {
            if (idx >= selected.length) { this.bulkMode = false; this.emitBulkReport(mode, selected.length); return; }
            this.generateForStudent(selected[idx].student, () => { idx++; next(); }, mode);
        };
        next();
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
        base.info = {...(base.info || {}), title: `Assessment Reports (${count})`};

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

    generateForStudent = (student: any, callback?: () => void, mode: string = 'preview') => {
        this.isGenerating = true;
        let selectedClass = this.schoolClasses.find((sc) => sc.id == this.filterSchoolClassId);
        let session = this.sessions.find((s) => s.id == this.filterSessionId);
        let year = this.academicYears.find((y) => y.id == this.filterAcademicYearId);
        let learningLevel = selectedClass?.learningLevel;
        let studentId = +student.id;

        // Load student's allocated subjects, assessments, school details and class leaders
        forkJoin([
            this.studentSubjectsSvc.get(`/studentSubjects/bySchoolClassId/${this.filterSchoolClassId}/${studentId}`),
            this.studentAssessmentSvc.get(`/studentAssessments/bySessionIdAndParams/${this.filterSessionId}?studentId=${studentId}&schoolClassId=${this.filterSchoolClassId}`),
            this.schoolSvc.get('/schooldetails'),
            this.schoolClassesSvc.get(`/schoolClassLeaders/bySchoolClassId/${this.filterSchoolClassId}`)
        ]).subscribe({
            next: ([studentSubjects, assessments, schoolDetails, classLeaders]) => {
                // Get the student's own subjects only
                let studentSubjectList = (studentSubjects as any[])
                    .map((ss) => ss.subject)
                    .filter(Boolean)
                    .sort((a, b) => (a.rank || 0) - (b.rank || 0));

                let teacherLeaders = (classLeaders as any[] || []).filter(
                    (cl) => cl.classLeadershipRole?.personType === 1 || cl.classLeadershipRole?.personType === 'Teacher'
                );
                let classLeadersText = teacherLeaders
                    .map((cl) => `${cl.person?.fullName || ''} [${cl.classLeadershipRole?.name || ''}]`)
                    .filter(Boolean)
                    .join(', ') || '';
                // For each of the student's subjects, load strands → substrands → specific outcomes
                let subjectRequests = studentSubjectList.map((subj) =>
                    this.strandSvc.get(`/strands/bySubjectId?subjectId=${subj.id}&learningLvlId=${learningLevel?.id}&academicYearId=${this.filterAcademicYearId}`)
                );

                forkJoin(subjectRequests).subscribe({
                    next: (allStrands) => {
                        // For each strand, load sub-strands
                        let allStrandsFlat: any[] = [];
                        studentSubjectList.forEach((subj, sIdx) => {
                            (allStrands[sIdx] as any[]).forEach((strand) => {
                                allStrandsFlat.push({...strand, subjectName: subj.name, subjectId: subj.id,
                                    strandCode: strand.code, themeName: strand.theme?.name, themeCode: strand.theme?.code});
                            });
                        });

                        if (allStrandsFlat.length === 0) {
                            this.buildPdf(student, session, year, selectedClass, schoolDetails[0], assessments as any[], [], classLeadersText, callback, mode);
                            return;
                        }

                        let ssRequests = allStrandsFlat.map((strand) =>
                            this.subStrandSvc.get(`/subStrands/byStrandId/${strand.id}`)
                        );

                        forkJoin(ssRequests).subscribe({
                            next: (allSubStrands) => {
                                let allSubStrandsFlat: any[] = [];
                                allStrandsFlat.forEach((strand, idx) => {
                                    (allSubStrands[idx] as any[]).forEach((ss) => {
                                        allSubStrandsFlat.push({...ss, strandName: strand.name, strandCode: strand.strandCode,
                                            subjectName: strand.subjectName, subjectId: strand.subjectId,
                                            themeName: strand.themeName, themeCode: strand.themeCode,
                                            ssCode: ss.code});
                                    });
                                });

                                if (allSubStrandsFlat.length === 0) {
                                    this.buildPdf(student, session, year, selectedClass, schoolDetails[0], assessments as any[], [], classLeadersText, callback, mode);
                                    return;
                                }

                                let soRequests = allSubStrandsFlat.map((ss) =>
                                    this.specificOutcomeSvc.get(`/specificOutcomes/bySubStrandId/${ss.id}`)
                                );

                                forkJoin(soRequests).subscribe({
                                    next: (allSOs) => {
                                        // Build hierarchical structure
                                        let structure: any[] = [];
                                        let subjectMap: any = {};

                                        // Build a richer structure with codes and themes
                                        let subjectData: any = {};

                                        allSubStrandsFlat.forEach((ss, idx) => {
                                            let outcomes = (allSOs[idx] as any[]).sort((a, b) => a.rank - b.rank);
                                            let subjKey = ss.subjectName;
                                            let strandKey = ss.strandName;

                                            if (!subjectData[subjKey]) subjectData[subjKey] = { themes: new Set(), strands: {} };
                                            if (ss.themeName) subjectData[subjKey].themes.add(ss.themeCode ? `${ss.themeCode} - ${ss.themeName}` : ss.themeName);

                                            if (!subjectData[subjKey].strands[strandKey]) {
                                                subjectData[subjKey].strands[strandKey] = { code: ss.strandCode, subStrands: {} };
                                            }
                                            subjectData[subjKey].strands[strandKey].subStrands[ss.name] = { code: ss.ssCode, outcomes };
                                        });

                                        // Convert to array
                                        studentSubjectList.forEach((subj) => {
                                            let data = subjectData[subj.name];
                                            if (!data) return;
                                            let strandArr: any[] = [];
                                            Object.keys(data.strands).forEach((strandName) => {
                                                let strandInfo = data.strands[strandName];
                                                let ssArr: any[] = [];
                                                Object.keys(strandInfo.subStrands).forEach((ssName) => {
                                                    let ssInfo = strandInfo.subStrands[ssName];
                                                    ssArr.push({name: ssName, code: ssInfo.code, outcomes: ssInfo.outcomes});
                                                });
                                                strandArr.push({name: strandName, code: strandInfo.code, subStrands: ssArr});
                                            });
                                            let themesList = Array.from(data.themes);
                                            structure.push({subjectName: subj.name, themes: themesList, strands: strandArr});
                                        });

                                        this.buildPdf(student, session, year, selectedClass, schoolDetails[0], assessments as any[], structure, classLeadersText, callback, mode);
                                    },
                                    error: (err) => { this.isGenerating = false; this.bulkMode = false; this.toastr.error(err.error); }
                                });
                            },
                            error: (err) => { this.isGenerating = false; this.bulkMode = false; this.toastr.error(err.error); }
                        });
                    },
                    error: (err) => { this.isGenerating = false; this.bulkMode = false; this.toastr.error(err.error); }
                });
            },
            error: (err) => { this.isGenerating = false; this.bulkMode = false; this.toastr.error(err.error); }
        });
    };

    buildPdf = (student, session, year, schoolClass, school, assessments: any[], structure: any[], classLeadersText: string, callback?: () => void, mode: string = 'preview') => {
        this.reportSvc.loadImageAsBase64('assets/img/shule-nova-logo-only.png').subscribe({
            next: (blob) => {
                const reader = new FileReader();
                reader.onloadend = () => {
                    const base64data: string = reader.result as string;
                    let sessionName = session?.sessionName || '';
                    let gradeName = schoolClass?.learningLevel?.name || '';
                    let streamName = schoolClass?.schoolStream?.name || '';
                    let edLevel = this.educationLevels.find((el) => el.id == schoolClass?.learningLevel?.educationLevelId);

                    // Grading key
                    let buildGradingKey = () => ({
                        layout: 'lightHorizontalLines',
                        table: {
                            widths: this.grades.map(() => '*'),
                            body: [
                                this.grades.map((g) => ({text: `${g.abbr} (${g.points})`, alignment: 'center', fontSize: 7, bold: true})),
                                this.grades.map((g) => ({text: g.name, alignment: 'center', fontSize: 6, italics: true}))
                            ]
                        },
                        marginBottom: 5
                    });

                    // Helper to build student details box with subject
                    let buildStudentBox = (subjectName: string) => ({
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
                                    {text: 'Adm No:', fontSize: 8, bold: true},
                                    {text: student.upi || '', fontSize: 9, bold: true},
                                    {text: 'Name:', fontSize: 8, bold: true},
                                    {text: student.fullName, fontSize: 9, bold: true, noWrap: true},
                                    {text: 'Grade:', fontSize: 8, bold: true},
                                    {text: gradeName, fontSize: 9},
                                    {text: 'Stream:', fontSize: 8, bold: true},
                                    {text: streamName, fontSize: 9},
                                    {text: 'Term:', fontSize: 8, bold: true},
                                    {text: sessionName, fontSize: 9}
                                ],
                                [
                                    {text: 'Year:', fontSize: 8, bold: true},
                                    {text: year?.name || '', fontSize: 9},
                                    {text: 'Class Leaders:', fontSize: 8, bold: true},
                                    {text: classLeadersText || '..............................', fontSize: 8, noWrap: true},
                                    {text: 'Subject:', fontSize: 8, bold: true},
                                    {text: subjectName, fontSize: 9, bold: true, colSpan: 5},
                                    {}, {}, {}, {}
                                ]
                            ]
                        },
                        marginBottom: 5
                    });

                    let reportTitle = `${sessionName.toUpperCase()} ${(year?.name || '')} ASSESSMENT REPORT FOR ${(edLevel?.name || '').toUpperCase()}`;

                    // Build content per subject
                    let subjectPages: any[] = [];
                    structure.forEach((subj, sIdx) => {
                        if (sIdx === 0) {
                            // First subject: full school header
                            subjectPages.push({...this.reportSvc.getDIVIDER()});
                            subjectPages.push(this.reportSvc.getReportHeader(school));
                            subjectPages.push({...this.reportSvc.getDIVIDER(), marginBottom: 1});
                            subjectPages.push(this.reportSvc.getReportTitle(reportTitle));
                            subjectPages.push({...this.reportSvc.getDIVIDER(), marginBottom: 3});
                            subjectPages.push(buildStudentBox(subj.subjectName));
                            subjectPages.push({text: 'Key:', fontSize: 8, bold: true});
                            subjectPages.push(buildGradingKey());
                        } else {
                            // Subsequent subjects: separator + subject header only
                            subjectPages.push({canvas: [{type: 'line', x1: 0, y1: 0, x2: 515, y2: 0, lineWidth: 0.5, lineColor: '#999'}], marginTop: 8, marginBottom: 4});
                            subjectPages.push({text: `Subject: ${subj.subjectName}`, fontSize: 9, bold: true, marginBottom: 3});
                        }

                        // Assessment table
                        let tableBody: any[] = [
                            [
                                {text: '', style: 'tableHeader'},
                                {text: "Rate learner's ability to:", style: 'tableHeader'},
                                {text: 'Fill in appropriately with: ' + this.grades.map((g) => g.points).join(', '), style: 'tableHeader', alignment: 'center'},
                                {text: 'REFLECTION/\nCOMMENT', style: 'tableHeader', alignment: 'center'}
                            ]
                        ];

                        // Insert theme rows at the top if they exist
                        if (subj.themes && subj.themes.length > 0) {
                            subj.themes.forEach((theme: string) => {
                                tableBody.push([
                                    {text: '', bold: true, fontSize: 8},
                                    {text: theme.toUpperCase(), bold: true, fontSize: 8, colSpan: 3},
                                    {}, {}
                                ]);
                            });
                        }

                        let rowNum = 1;
                        subj.strands.forEach((strand, stIdx) => {
                            // Strand header row
                            let strandCode = strand.code || `${rowNum}`;
                            let strandLabel = strand.code ? `${strand.name}` : strand.name;
                            tableBody.push([
                                {text: strandCode, bold: true, fontSize: 8},
                                {text: strandLabel.toUpperCase(), bold: true, fontSize: 8, colSpan: 3},
                                {}, {}
                            ]);
                            let strandNum = rowNum;
                            rowNum++;

                            strand.subStrands.forEach((ss, ssIdx) => {
                                // SubStrand header row
                                let ssCode = ss.code || `${strandNum}.${ssIdx + 1}`;
                                tableBody.push([
                                    {text: ssCode, bold: true, fontSize: 7},
                                    {text: ss.name, bold: true, fontSize: 7, italics: true, colSpan: 3},
                                    {}, {}
                                ]);

                                // Specific outcomes
                                ss.outcomes.forEach((so) => {
                                    let assessment = assessments.find((a) => a.specificOutcomeId == +so.id);
                                    let grade = assessment ? this.grades.find((g) => g.id == assessment.gradeId) : null;
                                    tableBody.push([
                                        {text: '', fontSize: 7},
                                        {text: '• ' + so.name, fontSize: 7},
                                        {text: grade ? grade.points : '', alignment: 'center', fontSize: 8, bold: true},
                                        {text: grade ? grade.abbr : '', alignment: 'center', fontSize: 7, bold: true}
                                    ]);
                                });
                            });
                        });

                        subjectPages.push({
                            layout: {
                                hLineWidth: () => 0.5,
                                vLineWidth: () => 0.5,
                                hLineColor: () => '#aaa',
                                vLineColor: () => '#aaa'
                            },
                            table: {
                                headerRows: 1,
                                widths: [30, '*', 30, 50],
                                body: tableBody
                            },
                            marginBottom: 5
                        });
                    });

                    if (subjectPages.length === 0) {
                        subjectPages.push({text: 'No assessment data found.', fontSize: 10, alignment: 'center', margin: [0, 20]});
                    }

                    const docDefinition: any = {
                        pageMargins: [25, 20, 25, 30],
                        pageSize: 'A4',
                        info: {
                            title: `Assessment Report - ${student.fullName}`,
                            author: (this.userSvc?.currentUser?.firstName || '') + ' ' + (this.userSvc?.currentUser?.lastName || '')
                        },
                        footer: this.reportSvc.getFooter('portrait'),
                        images: {systemLogo: base64data, schoolLogo: school?.logoAsBase64},
                        styles: {
                            tableHeader: {bold: true, fontSize: 8, fillColor: '#d4edda', color: '#155724'}
                        },
                        content: [
                            ...subjectPages,
                            // Timestamp
                            {
                                text: `This is a system generated document. Printed on ${new Date().toLocaleString('en-GB')}`,
                                fontSize: 7, color: '#999999', italics: true, alignment: 'center', marginTop: 10
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
