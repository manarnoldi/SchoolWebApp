import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {ExamResult} from '../../models/exam-result';
import {ExamResultService} from '../../services/exam-result.service';
import {ExamService} from '../../services/exam.service';
import {ExamTypeService} from '../../services/exam-type.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
import {GradesService} from '@/academics/services/grades.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {Status} from '@/core/enums/status';

@Component({
    selector: 'app-exam-results-classwise',
    templateUrl: './exam-results-classwise.component.html',
    styleUrl: './exam-results-classwise.component.scss'
})
export class ExamResultsClasswiseComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/exams/exam-results-classwise'], title: 'CBE Exams: Results (Classwise)'}
    ];
    dashboardTitle = 'CBE Exams: Results (Classwise)';

    parseInt = parseInt;

    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    examTypes: any[] = [];
    grades: any[] = [];
    allGrades: any[] = [];
    gradingSettings: any[] = [];
    gradingCategory: string = '4-Point';
    studentClasses: any[] = [];
    students: any[] = [];
    learningLevels: any[] = [];

    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterSchoolClassId: any = null;
    filterExamTypeId: any = null;
    filterStudentId: any = null;

    // Rows: one per subject/exam
    scoringRows: {
        examId: number;
        subjectName: string;
        examMark: number;
        score: number | null;
        description: string;
        existingId: string | null;
    }[] = [];

    studentLoaded: boolean = false;
    isSaving: boolean = false;
    selectedStudentName: string = '';

    constructor(
        private toastr: ToastrService,
        private examResultSvc: ExamResultService,
        private examSvc: ExamService,
        private examTypeSvc: ExamTypeService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private schoolClassesSvc: SchoolClassesService,
        private learningLevelSvc: LearningLevelsService,
        private studentClassSvc: StudentClassService,
        private studentSubjectsSvc: StudentSubjectsService,
        private gradesSvc: GradesService,
        private globalSettingSvc: GlobalSettingService
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems() {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.academicYearSvc.get('/academicYears'),
            this.examTypeSvc.get('/examTypes'),
            this.gradesSvc.get('/grades'),
            this.globalSettingSvc.getByModule('Grading')
        ]).subscribe({
            next: ([curricula, academicYears, examTypes, allGrades, gradingSettings]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.filter((y) => y.status === true).sort((a, b) => a.rank - b.rank);
                this.examTypes = examTypes.sort((a, b) => a.rank - b.rank);
                this.gradingSettings = (gradingSettings as any[]) || [];
                this.allGrades = allGrades;
                // Global default until a class (hence education level) is chosen;
                // loadStudentResults re-resolves it for the selected class.
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

    private applyExamGrading = (edLevelId: any) => {
        this.gradingCategory = this.examGradingCategoryFor(edLevelId);
        this.grades = this.allGrades.filter((g) => g.category === this.gradingCategory).sort((a, b) => a.rank - b.rank);
    };

    onCurriculumChange = () => {
        this.sessions = this.schoolClasses = this.students = [];
        this.filterAcademicYearId = this.filterSessionId = this.filterSchoolClassId = null;
        this.filterExamTypeId = this.filterStudentId = null;
        this.studentLoaded = false;
        if (!this.filterCurriculumId) return;
        this.learningLevelSvc.getLearningLevelsByCurriculum(this.filterCurriculumId).subscribe({
            next: (levels) => { this.learningLevels = levels.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onAcademicYearChange = () => {
        this.sessions = this.schoolClasses = this.students = [];
        this.filterSessionId = this.filterSchoolClassId = this.filterExamTypeId = this.filterStudentId = null;
        this.studentLoaded = false;
        if (!this.filterAcademicYearId || !this.filterCurriculumId) return;
        forkJoin([
            this.sessionsSvc.get(`/sessions/byCurriculumYearId?curriculumId=${this.filterCurriculumId}&academicYearId=${this.filterAcademicYearId}`),
            this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${this.filterAcademicYearId}`)
        ]).subscribe({
            next: ([sessions, schoolClasses]) => {
                this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                let currLLIds = this.learningLevels.map((ll) => parseInt(ll.id));
                this.schoolClasses = schoolClasses.filter((sc) => currLLIds.includes(parseInt(sc.learningLevelId)));
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onClassChange = () => {
        this.studentClasses = this.students = [];
        this.filterStudentId = null;
        this.studentLoaded = false;
        if (!this.filterSchoolClassId) return;
        this.studentClassSvc.getBySchoolClassId(this.filterSchoolClassId, Status.Active).subscribe({
            next: (studentClasses) => {
                this.studentClasses = studentClasses;
                this.students = studentClasses
                    .map((sc) => sc.student)
                    .filter(Boolean)
                    .sort((a, b) => (a.fullName || '').localeCompare(b.fullName || ''));
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    getExamName = (): string => {
        let year = this.academicYears.find((y) => y.id == this.filterAcademicYearId);
        let session = this.sessions.find((s) => s.id == this.filterSessionId);
        let examType = this.examTypes.find((et) => et.id == this.filterExamTypeId);
        return `${year?.name || ''}-${session?.sessionName || ''}-${examType?.name || ''}`;
    };

    loadStudentResults = () => {
        if (!this.filterSessionId || !this.filterSchoolClassId || !this.filterExamTypeId || !this.filterStudentId) {
            this.toastr.info('Please select Session, Class, Exam Type, and Student.');
            return;
        }

        // Use the selected class's education-level grading scale (4/8-Point).
        let selClass = this.schoolClasses.find((sc) => +sc.id === +this.filterSchoolClassId);
        let edLevelId = selClass?.learningLevel?.educationLevelId
            ?? this.learningLevels.find((ll) => +ll.id === +(selClass?.learningLevelId))?.educationLevelId;
        this.applyExamGrading(edLevelId);

        let selectedStudent = this.students.find((s) => parseInt(s.id) == this.filterStudentId);
        this.selectedStudentName = selectedStudent ? `${selectedStudent.upi || ''}-${selectedStudent.fullName || ''}` : '';

        // Find the studentClassId for this student
        let studentClass = this.studentClasses.find((sc) => sc.student && parseInt(sc.student.id) == this.filterStudentId);
        if (!studentClass) {
            this.toastr.error('Student class record not found.');
            return;
        }

        // Load exams and student's allocated subjects in parallel
        let url = `/exams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${this.filterSessionId}&schoolClassId=${this.filterSchoolClassId}&examTypeId=${this.filterExamTypeId}`;

        forkJoin([
            this.examSvc.get(url),
            this.studentSubjectsSvc.get(`/studentSubjects/byStudentClassId/${studentClass.id}`)
        ]).subscribe({
            next: ([exams, studentSubjects]) => {
                // Get subject IDs allocated to this student (ensure consistent number type)
                let allocatedSubjectIds = studentSubjects.map((ss) => +ss.subjectId);

                // Filter exams to only show subjects allocated to this student
                let filteredExams = exams.filter((exam) => allocatedSubjectIds.includes(+exam.subjectId));

                if (filteredExams.length === 0) {
                    this.toastr.info('No exams found for the student\'s allocated subjects.');
                    this.studentLoaded = false;
                    return;
                }

                // For each exam, check if the student has an existing result
                let resultRequests = filteredExams.map((exam) =>
                    this.examResultSvc.get(`/examResults/byExamId/${exam.id}`)
                );

                forkJoin(resultRequests).subscribe({
                    next: (allResults) => {
                        this.scoringRows = filteredExams.map((exam, idx) => {
                            let results = allResults[idx];
                            let existing = results.find((r) => r.studentId == this.filterStudentId);
                            return {
                                examId: parseInt(exam.id),
                                subjectName: exam.subject?.name || '',
                                subjectRank: exam.subject?.rank || 0,
                                examMark: exam.examMark,
                                score: existing ? existing.score : null,
                                description: existing ? (existing.description || '') : '',
                                existingId: existing ? existing.id : null
                            };
                        }).sort((a, b) => a.subjectRank - b.subjectRank);

                        this.studentLoaded = true;
                    },
                    error: (err) => this.toastr.error(err.error)
                });
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    getGradeForPercent = (percent: number): string => {
        let grade = this.grades.find((g) => percent >= g.minScore && percent <= g.maxScore);
        return grade ? `${grade.abbr}-${grade.points}` : '';
    };

    validateScore = (row: any) => {
        if (row.score != null && row.score > row.examMark) {
            this.toastr.warning(`Score cannot exceed exam mark of ${row.examMark} for ${row.subjectName}. Entry cleared.`);
            row.score = null;
            row.description = '';
        } else if (row.score != null && row.score < 0) {
            this.toastr.warning('Score cannot be negative. Entry cleared.');
            row.score = null;
            row.description = '';
        } else if (row.score != null && row.examMark > 0) {
            let percent = (row.score / row.examMark) * 100;
            let grade = this.grades.find((g) => percent >= g.minScore && percent <= g.maxScore);
            row.description = grade ? grade.name : '';
        } else {
            row.description = '';
        }
    };

    saveAll = () => {
        let rowsToSave = this.scoringRows.filter((r) => r.score != null);
        if (rowsToSave.length === 0) {
            this.toastr.info('Please enter at least one score before saving.');
            return;
        }
        let invalidRows = rowsToSave.filter((r) => r.score > r.examMark);
        if (invalidRows.length > 0) {
            this.toastr.error(`Score for ${invalidRows[0].subjectName} exceeds exam mark of ${invalidRows[0].examMark}. Please correct before saving.`);
            return;
        }

        Swal.fire({
            title: 'Save all results?',
            text: `${rowsToSave.length} result(s) will be saved for ${this.selectedStudentName}.`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: 'Save', cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                let batchData = rowsToSave.map((row) => new ExamResult({
                    studentId: this.filterStudentId,
                    examId: row.examId,
                    score: row.score,
                    description: row.description
                }));

                this.examResultSvc.createBatch('/examResults/batch', batchData).subscribe(
                    () => {
                        this.isSaving = false;
                        this.toastr.success(`${rowsToSave.length} result(s) saved!`);
                        this.loadStudentResults();
                    },
                    (err) => {
                        this.isSaving = false;
                        this.toastr.error(err.error?.message || 'Error saving results.');
                    }
                );
            }
        });
    };

    deleteResult = (row: any) => {
        if (!row.existingId) return;
        Swal.fire({
            title: 'Delete result?',
            text: `Remove ${row.subjectName} result?`,
            width: 400, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete', cancelButtonText: 'Cancel',
            confirmButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                this.examResultSvc.delete('/examResults', parseInt(row.existingId)).subscribe({
                    next: () => {
                        this.toastr.success(`${row.subjectName} result deleted.`);
                        row.existingId = null;
                        row.score = null;
                        row.description = '';
                    },
                    error: (err) => this.toastr.error(err.error?.message || 'Error deleting result.')
                });
            }
        });
    };

    getTotalScore = (): number => {
        return this.scoringRows.filter((r) => r.score != null).reduce((sum, r) => sum + (r.score || 0), 0);
    };

    getTotalMark = (): number => {
        return this.scoringRows.filter((r) => r.score != null).reduce((sum, r) => sum + r.examMark, 0);
    };

    getScoredCount = (): number => {
        return this.scoringRows.filter((r) => r.score != null).length;
    };

    getExistingCount = (): number => {
        return this.scoringRows.filter((r) => r.existingId != null).length;
    };

    deleteAll = () => {
        let existingRows = this.scoringRows.filter((r) => r.existingId != null);
        if (existingRows.length === 0) return;

        Swal.fire({
            title: 'Delete all results?',
            text: `${existingRows.length} result(s) for ${this.selectedStudentName} will be permanently deleted.`,
            width: 400, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete All', cancelButtonText: 'Cancel',
            confirmButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                let requests = existingRows.map((row) =>
                    this.examResultSvc.delete('/examResults', parseInt(row.existingId))
                );
                forkJoin(requests).subscribe(
                    () => {
                        this.isSaving = false;
                        this.toastr.success(`${existingRows.length} result(s) deleted.`);
                        this.loadStudentResults();
                    },
                    (err) => {
                        this.isSaving = false;
                        this.toastr.error(err.error?.message || 'Error deleting results.');
                    }
                );
            }
        });
    };
}
