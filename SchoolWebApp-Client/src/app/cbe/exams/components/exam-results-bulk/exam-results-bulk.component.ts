import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {ExamResult} from '../../models/exam-result';
import {ExamResultService} from '../../services/exam-result.service';
import {ExamService} from '../../services/exam.service';
import {ExamTypeService} from '../../services/exam-type.service';
import {SchoolExamService} from '../../services/school-exam.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {EducationLevelSubjectService} from '@/academics/services/education-level-subject.service';
import {GradesService} from '@/academics/services/grades.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
import {Status} from '@/core/enums/status';

@Component({
    selector: 'app-exam-results-bulk',
    templateUrl: './exam-results-bulk.component.html',
    styleUrl: './exam-results-bulk.component.scss'
})
export class ExamResultsBulkComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/exams/exam-results-bulk'], title: 'CBE Exams: Bulk Results Entry'}
    ];
    dashboardTitle = 'CBE Exams: Bulk Results Entry';

    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    examTypes: any[] = [];
    grades: any[] = [];
    learningLevels: any[] = [];
    gradingCategory: string = '4-Point';

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterSchoolClassId: any = null;
    filterExamTypeId: any = null;
    filterSchoolExamId: any = null;
    schoolExams: any[] = [];

    // Grid data
    subjects: any[] = []; // {name, abbr, examId, examMark}
    studentRows: {
        studentId: number;
        studentName: string;
        scores: { [examId: number]: { score: number | null; existingId: string | null; disabled: boolean } };
    }[] = [];

    loaded: boolean = false;
    isLoading: boolean = false;
    isSaving: boolean = false;
    isDeleting: boolean = false;

    // Client-side paging for the bulk-grid; default 30 students per page.
    tablePage: number = 1;
    tablePageSize: number = 30;

    constructor(
        private toastr: ToastrService,
        private examResultSvc: ExamResultService,
        private examSvc: ExamService,
        private examTypeSvc: ExamTypeService,
        private schoolExamSvc: SchoolExamService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private schoolClassesSvc: SchoolClassesService,
        private learningLevelSvc: LearningLevelsService,
        private educationLevelSubjectSvc: EducationLevelSubjectService,
        private gradesSvc: GradesService,
        private globalSettingSvc: GlobalSettingService,
        private studentClassSvc: StudentClassService,
        private studentSubjectsSvc: StudentSubjectsService
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.academicYearSvc.get('/academicYears'),
            this.examTypeSvc.get('/examTypes'),
            this.gradesSvc.get('/grades'),
            this.globalSettingSvc.getByKey('Grading', 'ExamResults')
        ]).subscribe({
            next: ([curricula, academicYears, examTypes, allGrades, gradingSetting]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.filter((y) => y.status === true).sort((a, b) => a.rank - b.rank);
                this.examTypes = examTypes.sort((a, b) => a.rank - b.rank);
                let settingResponse = gradingSetting as any;
                this.gradingCategory = settingResponse?.settingValue || '4-Point';
                this.grades = allGrades.filter(g => g.category === this.gradingCategory).sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    onCurriculumChange = () => {
        this.sessions = this.schoolClasses = this.schoolExams = [];
        this.filterAcademicYearId = this.filterSessionId = this.filterSchoolClassId = null;
        this.filterSchoolExamId = this.filterExamTypeId = null;
        this.clearGrid();
        if (!this.filterCurriculumId) return;
        this.learningLevelSvc.getLearningLevelsByCurriculum(this.filterCurriculumId).subscribe({
            next: (levels) => { this.learningLevels = levels.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onAcademicYearChange = () => {
        this.sessions = this.schoolClasses = this.schoolExams = [];
        this.filterSessionId = this.filterSchoolClassId = null;
        this.filterSchoolExamId = this.filterExamTypeId = null;
        this.clearGrid();
        if (!this.filterAcademicYearId || !this.filterCurriculumId) return;
        forkJoin([
            this.sessionsSvc.get(`/sessions/byCurriculumYearId?curriculumId=${this.filterCurriculumId}&academicYearId=${this.filterAcademicYearId}`),
            this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${this.filterAcademicYearId}`)
        ]).subscribe({
            next: ([sessions, schoolClasses]) => {
                this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                let currLLIds = this.learningLevels.map((ll) => +ll.id);
                this.schoolClasses = schoolClasses.filter((sc) => currLLIds.includes(+sc.learningLevelId));
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    // Session drives the School Exam list (a school exam carries the exam type
    // the grid still filters by).
    onSessionChange = () => {
        this.clearGrid();
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
        this.clearGrid();
        let se = this.schoolExams.find((s) => s.id == this.filterSchoolExamId);
        this.filterExamTypeId = se?.examTypeId ?? se?.examType?.id ?? null;
    };

    // Changing the class (or any filter) invalidates the loaded grid - clear it
    // so stale marks aren't shown until the user clicks Load Grid again.
    onClassChange = () => this.clearGrid();

    private clearGrid() {
        this.loaded = false;
        this.subjects = [];
        this.studentRows = [];
        this.tablePage = 1;
    }

    getGradeForPercent = (percent: number): any => {
        return this.grades.find((g) => percent >= g.minScore && percent <= g.maxScore);
    };

    loadGrid = () => {
        if (!this.filterSessionId || !this.filterSchoolClassId || !this.filterSchoolExamId) {
            this.toastr.info('Please select Session, Class, and School Exam.');
            return;
        }

        this.isLoading = true;
        this.loaded = false;

        let url = `/exams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${this.filterSessionId}&schoolClassId=${this.filterSchoolClassId}&examTypeId=${this.filterExamTypeId}`;

        // Find education level for the selected class
        let selectedClass = this.schoolClasses.find((sc) => +sc.id === +this.filterSchoolClassId);
        let educationLevelId = selectedClass?.learningLevel?.educationLevelId;

        if (!educationLevelId) {
            this.isLoading = false;
            this.toastr.error('Could not determine education level for the selected class.');
            return;
        }

        forkJoin([
            this.examSvc.get(url),
            this.studentClassSvc.getBySchoolClassId(this.filterSchoolClassId, Status.Active),
            this.educationLevelSubjectSvc.getByEducationLevelAndAcademicYear(educationLevelId, this.filterAcademicYearId)
        ]).subscribe({
            next: ([exams, studentClasses, elSubjects]) => {
                if (exams.length === 0) {
                    this.isLoading = false;
                    this.toastr.info('No exams registered for this selection.');
                    return;
                }

                // Allowed subject ids from education level allocation
                let allowedSubjectIds = new Set(
                    (elSubjects || [])
                        .map((es) => es.subject)
                        .filter((s) => s && s.examinable !== false)
                        .map((s) => +s.id)
                );

                this.subjects = exams
                    .filter((e) => allowedSubjectIds.has(+e.subjectId))
                    .sort((a, b) => (a.subject?.rank || 0) - (b.subject?.rank || 0))
                    .map((e) => ({
                        name: e.subject?.name || '',
                        abbr: e.subject?.abbr || e.subject?.name || '',
                        subjectId: e.subjectId,
                        examId: +e.id,
                        examMark: e.examMark
                    }));

                if (this.subjects.length === 0) {
                    this.isLoading = false;
                    this.toastr.info('No exams found for subjects allocated to this class.');
                    return;
                }

                let studentClassList = studentClasses
                    .filter((sc) => sc.student)
                    .sort((a, b) => (a.student.fullName || '').localeCompare(b.student.fullName || ''));
                let students = studentClassList.map((sc) => sc.student);

                // Load results for all exams + student subject allocations
                let resultRequests = this.subjects.map((s) =>
                    this.examResultSvc.get(`/examResults/byExamId/${s.examId}`)
                );
                let subjectRequests = studentClassList.map((sc) =>
                    this.studentSubjectsSvc.get(`/studentSubjects/byStudentClassId/${sc.id}`)
                );

                forkJoin([...resultRequests, ...subjectRequests]).subscribe({
                    next: (allData) => {
                        let allResults = allData.slice(0, this.subjects.length);
                        let allStudentSubjects = allData.slice(this.subjects.length);

                        this.studentRows = students.map((student, sIdx) => {
                            let studentSubjects = allStudentSubjects[sIdx] as any[];
                            let allocatedSubjectIds = (studentSubjects || []).map((ss) => +ss.subjectId);
                            let scores: any = {};
                            this.subjects.forEach((subj, idx) => {
                                let existing = (allResults[idx] as any[]).find((r) => r.studentId == +student.id);
                                let isAllocated = allocatedSubjectIds.includes(+subj.subjectId);
                                scores[subj.examId] = {
                                    score: existing ? existing.score : null,
                                    existingId: existing ? existing.id : null,
                                    disabled: !isAllocated
                                };
                            });
                            return {
                                studentId: +student.id,
                                studentName: `${student.upi || ''}-${student.fullName || ''}`,
                                scores
                            };
                        });
                        this.loaded = true;
                        this.isLoading = false;
                    },
                    error: (err) => { this.isLoading = false; this.toastr.error(err.error); }
                });
            },
            error: (err) => { this.isLoading = false; this.toastr.error(err.error); }
        });
    };

    validateScore = (row: any, subj: any) => {
        let entry = row.scores[subj.examId];
        if (entry.score != null && entry.score > subj.examMark) {
            this.toastr.warning(`Score cannot exceed ${subj.examMark}. Entry cleared.`);
            entry.score = null;
        } else if (entry.score != null && entry.score < 0) {
            this.toastr.warning('Score cannot be negative. Entry cleared.');
            entry.score = null;
        }
    };

    saveAll = () => {
        // Collect all entries with scores
        let batchData: any[] = [];
        this.studentRows.forEach((row) => {
            this.subjects.forEach((subj) => {
                let entry = row.scores[subj.examId];
                if (entry.score != null) {
                    let pct = subj.examMark > 0 ? (entry.score / subj.examMark) * 100 : 0;
                    let grade = this.getGradeForPercent(pct);
                    batchData.push(new ExamResult({
                        studentId: row.studentId,
                        examId: subj.examId,
                        score: entry.score,
                        description: grade?.name || ''
                    }));
                }
            });
        });

        if (batchData.length === 0) {
            this.toastr.info('No scores to save.');
            return;
        }

        Swal.fire({
            title: 'Save all results?',
            text: `${batchData.length} result(s) will be saved.`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: 'Save', cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                this.examResultSvc.createBatch('/examResults/batch', batchData).subscribe(
                    () => {
                        this.isSaving = false;
                        this.toastr.success('All results saved!');
                        this.loadGrid();
                    },
                    (err) => {
                        this.isSaving = false;
                        this.toastr.error(err.error?.message || 'Error saving.');
                    }
                );
            }
        });
    };

    getScoredCount = (): number => {
        let count = 0;
        this.studentRows.forEach((row) => {
            this.subjects.forEach((subj) => {
                if (row.scores[subj.examId]?.score != null) count++;
            });
        });
        return count;
    };

    getSavedCount = (): number => {
        let count = 0;
        this.studentRows.forEach((row) => {
            this.subjects.forEach((subj) => {
                if (row.scores[subj.examId]?.existingId) count++;
            });
        });
        return count;
    };

    deleteCell = (row: any, examId: number) => {
        let cell = row.scores[examId];
        if (!cell?.existingId) return;
        this.examResultSvc.delete('/examResults', parseInt(cell.existingId)).subscribe({
            next: () => {
                cell.existingId = null;
                cell.score = null;
                this.toastr.success('Result deleted.');
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error deleting result.')
        });
    };

    deleteAllResults = () => {
        let savedCells: { examId: number; existingId: string }[] = [];
        this.studentRows.forEach((row) => {
            this.subjects.forEach((subj) => {
                let cell = row.scores[subj.examId];
                if (cell?.existingId) {
                    savedCells.push({ examId: subj.examId, existingId: cell.existingId });
                }
            });
        });
        if (savedCells.length === 0) return;

        Swal.fire({
            title: 'Delete all results?',
            text: `${savedCells.length} result(s) will be permanently deleted.`,
            width: 400, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete All',
            cancelButtonText: 'Cancel', confirmButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                this.isDeleting = true;
                let requests = savedCells.map((c) =>
                    this.examResultSvc.delete('/examResults', parseInt(c.existingId))
                );
                forkJoin(requests).subscribe(
                    () => {
                        this.isDeleting = false;
                        this.toastr.success(`${savedCells.length} result(s) deleted.`);
                        this.loadGrid();
                    },
                    (err) => {
                        this.isDeleting = false;
                        this.toastr.error(err.error?.message || 'Error deleting results.');
                    }
                );
            }
        });
    };
}
