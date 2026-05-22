import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {ExamResult} from '../../models/exam-result';
import {ExamResultService} from '../../services/exam-result.service';
import {ExamService} from '../../services/exam.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {SubjectsService} from '@/academics/services/subjects.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {EducationLevelSubjectService} from '@/academics/services/education-level-subject.service';
import {StudentClassService} from '@/students/services/student-class.service';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
import {GradesService} from '@/academics/services/grades.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {Status} from '@/core/enums/status';
import {AuthService} from '@/core/services/auth.service';
import {StaffSubjectsService} from '@/staff/services/staff-subjects.service';
import {StaffSubject} from '@/staff/models/staff-subject';

@Component({
    selector: 'app-exam-results',
    templateUrl: './exam-results.component.html',
    styleUrl: './exam-results.component.scss'
})
export class ExamResultsComponent implements OnInit {
    isAuthLoading: boolean;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/exams/exam-results'], title: 'CBE Exams: Exam Results'}
    ];
    dashboardTitle = 'CBE Exams: Exam Results';

    // Dropdown data
    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    subjects: any[] = [];
    exams: any[] = [];
    grades: any[] = [];
    learningLevels: any[] = [];
    gradingCategory: string = '4-Point';

    // Filter selections
    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterSchoolClassId: any = null;
    filterSubjectId: any = null;
    filterExamId: any = null;

    selectedExam: any = null;

    // Batch scoring rows
    scoringRows: {
        studentId: number;
        upi: string;
        fullName: string;
        score: number | null;
        description: string;
        existingId: string | null;
    }[] = [];

    studentsLoaded: boolean = false;
    isSaving: boolean = false;
    averageMethod: string = 'students_with_scores';

    // Teachers can only enter results for subjects they are allocated to in the
    // selected class. Administrators (and other non-teacher roles) bypass this
    // check and use the bulk entry / per-subject screen freely.
    currentUserIsTeacher: boolean = false;
    currentUserStaffId: number | null = null;
    teacherAllocations: StaffSubject[] = [];
    isAllocatedToSubject: boolean = true;

    /**
     * True when today (date-only, no time) is strictly AFTER the selected
     * exam's `examMarkEntryEndDate`. The per-subject results page is the
     * teacher-facing entry point, so we lock marks entry/edits/deletes here
     * once the deadline has passed. Administrators can still make corrections
     * via the Results (Bulk) page.
     */
    get isPastMarkEntryDeadline(): boolean {
        let raw = this.selectedExam?.examMarkEntryEndDate;
        if (!raw) return false;
        let deadline = new Date(raw);
        if (isNaN(deadline.getTime())) return false;
        deadline.setHours(0, 0, 0, 0);
        let today = new Date();
        today.setHours(0, 0, 0, 0);
        return today.getTime() > deadline.getTime();
    }

    // Client-side paging for the per-student scoring rows; default 30/page.
    tablePage: number = 1;
    tablePageSize: number = 30;

    constructor(
        private toastr: ToastrService,
        private examResultSvc: ExamResultService,
        private examSvc: ExamService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private schoolClassesSvc: SchoolClassesService,
        private subjectsSvc: SubjectsService,
        private learningLevelSvc: LearningLevelsService,
        private educationLevelSubjectSvc: EducationLevelSubjectService,
        private studentClassSvc: StudentClassService,
        private studentSubjectsSvc: StudentSubjectsService,
        private gradesSvc: GradesService,
        private globalSettingSvc: GlobalSettingService,
        private authService: AuthService,
        private staffSubjectsSvc: StaffSubjectsService
    ) {}

    ngOnInit(): void {
        let user = this.authService.getCurrentUser();
        this.currentUserIsTeacher = !!user?.currentUserTeacher;
        this.currentUserStaffId = user?.staffId ?? null;

        this.globalSettingSvc.getByKey('General', 'AverageCalculation').subscribe({
            next: (setting) => {
                this.averageMethod = setting?.settingValue || 'students_with_scores';
            },
            error: () => {}
        });
        this.refreshItems();
    }

    /**
     * Pulls the current user's StaffSubject allocations for the selected academic
     * year. Cached on the component so the per-subject lookup is a local filter,
     * not a server roundtrip on every dropdown change.
     */
    private loadTeacherAllocations() {
        if (!this.currentUserIsTeacher || !this.currentUserStaffId || !this.filterAcademicYearId) {
            this.teacherAllocations = [];
            this.updateAllocationStatus();
            return;
        }
        this.staffSubjectsSvc
            .getByStaffYearId(this.currentUserStaffId, this.filterAcademicYearId)
            .subscribe({
                next: (allocations) => {
                    this.teacherAllocations = allocations || [];
                    this.updateAllocationStatus();
                },
                error: () => {
                    this.teacherAllocations = [];
                    this.updateAllocationStatus();
                }
            });
    }

    /**
     * Re-evaluates whether the current user is permitted to enter results for
     * the currently selected (class, subject) tuple. Non-teachers always pass.
     * Teachers must have a StaffSubject matching the selection.
     */
    private updateAllocationStatus() {
        if (!this.currentUserIsTeacher) {
            this.isAllocatedToSubject = true;
            return;
        }
        // Not yet a complete selection — don't show the banner prematurely.
        if (!this.filterSubjectId || !this.filterSchoolClassId) {
            this.isAllocatedToSubject = true;
            return;
        }
        this.isAllocatedToSubject = this.teacherAllocations.some(
            (sa) => sa.subjectId == this.filterSubjectId && sa.schoolClassId == this.filterSchoolClassId
        );
    }

    refreshItems() {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.academicYearSvc.get('/academicYears'),
            this.gradesSvc.get('/grades'),
            this.globalSettingSvc.getByKey('Grading', 'ExamResults')
        ]).subscribe({
            next: ([curricula, academicYears, allGrades, gradingSetting]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.filter((y) => y.status === true).sort((a, b) => a.rank - b.rank);
                let settingResponse = gradingSetting as any;
                this.gradingCategory = settingResponse?.settingValue || '4-Point';
                this.grades = allGrades.filter(g => g.category === this.gradingCategory).sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    getExamName = (exam: any): string => {
        let year = this.academicYears.find((y) => y.id == this.filterAcademicYearId);
        let yearName = year?.name || '';
        let sessionName = exam.session?.sessionName || '';
        let typeName = exam.examType?.name || '';
        return `${yearName}-${sessionName}-${typeName}`;
    };

    onCurriculumChange = () => {
        this.sessions = this.schoolClasses = this.subjects = this.exams = [];
        this.filterAcademicYearId = this.filterSessionId = this.filterSchoolClassId = null;
        this.filterSubjectId = this.filterExamId = null;
        this.studentsLoaded = false;
        if (!this.filterCurriculumId) return;
        this.learningLevelSvc.getLearningLevelsByCurriculum(this.filterCurriculumId).subscribe({
            next: (levels) => { this.learningLevels = levels.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onAcademicYearChange = () => {
        this.sessions = this.schoolClasses = this.subjects = this.exams = [];
        this.filterSessionId = this.filterSchoolClassId = this.filterSubjectId = this.filterExamId = null;
        this.studentsLoaded = false;
        this.loadTeacherAllocations();
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

    onSessionChange = () => {
        this.exams = [];
        this.filterExamId = null;
        this.studentsLoaded = false;
        this.loadExams();
    };

    onClassChange = () => {
        this.subjects = this.exams = [];
        this.filterSubjectId = this.filterExamId = null;
        this.studentsLoaded = false;
        this.updateAllocationStatus();
        if (!this.filterSchoolClassId || !this.filterAcademicYearId) return;
        let selectedClass = this.schoolClasses.find((sc) => sc.id == this.filterSchoolClassId);
        if (!selectedClass?.learningLevel?.educationLevelId) return;
        this.educationLevelSubjectSvc
            .getByEducationLevelAndAcademicYear(selectedClass.learningLevel.educationLevelId, this.filterAcademicYearId)
            .subscribe({
                next: (elSubjects) => {
                    this.subjects = elSubjects.map((es) => es.subject)
                        .filter((s) => s.examinable !== false)
                        .sort((a, b) => a.rank - b.rank);
                },
                error: (err) => this.toastr.error(err.error)
            });
    };

    onSubjectChange = () => {
        this.exams = [];
        this.filterExamId = null;
        this.studentsLoaded = false;
        this.updateAllocationStatus();
        this.loadExams();
    };

    loadExams = () => {
        if (!this.filterSessionId || !this.filterAcademicYearId || !this.filterCurriculumId) return;
        let url = `/exams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${this.filterSessionId}`;
        if (this.filterSchoolClassId) url += `&schoolClassId=${this.filterSchoolClassId}`;
        if (this.filterSubjectId) url += `&subjectId=${this.filterSubjectId}`;
        this.examSvc.get(url).subscribe({
            next: (exams) => { this.exams = exams; },
            error: (err) => this.toastr.error(err.error)
        });
    };

    loadStudents = () => {
        if (!this.filterExamId || !this.filterSchoolClassId) {
            this.toastr.info('Please select a class and an exam.');
            return;
        }
        if (!this.isAllocatedToSubject) {
            this.toastr.warning('You are not allocated to this subject for the selected class. Contact your administrator to be assigned.');
            return;
        }

        this.selectedExam = this.exams.find((e) => e.id == this.filterExamId);
        this.scoringRows = [];
        this.studentsLoaded = false;

        // Load students allocated to this subject in this class, plus existing results
        forkJoin([
            this.studentSubjectsSvc.get(
                `/studentSubjects/bySchoolClassSubjectId/${this.filterSchoolClassId}/${this.selectedExam?.subjectId}`
            ),
            this.examResultSvc.get(`/examResults/byExamId/${this.filterExamId}`)
        ]).subscribe({
            next: ([studentSubjects, existingResults]) => {
                let students = studentSubjects
                    .map((ss: any) => ss.studentClass?.student)
                    .filter(Boolean)
                    .sort((a, b) => (a.fullName || '').localeCompare(b.fullName || ''));

                this.scoringRows = students.map((student) => {
                    let existing = existingResults.find(
                        (er) => er.studentId == parseInt(student.id)
                    );
                    return {
                        studentId: parseInt(student.id),
                        upi: student.upi || '',
                        fullName: `${student.upi || ''}-${student.fullName || ''}`,
                        score: existing ? existing.score : null,
                        description: existing ? (existing.description || '') : '',
                        existingId: existing ? existing.id : null
                    };
                });

                this.studentsLoaded = true;
                if (this.scoringRows.length === 0) {
                    this.toastr.info('No students found in the selected class.');
                }
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    getGradeForPercent = (percent: number): string => {
        let grade = this.grades.find((g) => percent >= g.minScore && percent <= g.maxScore);
        return grade ? `${grade.abbr} (${grade.points})` : '';
    };

    validateScore = (row: any, maxMark: number) => {
        if (row.score != null && row.score > maxMark) {
            this.toastr.warning(`Score cannot exceed exam mark of ${maxMark}. Entry cleared.`);
            row.score = null;
            row.description = '';
        } else if (row.score != null && row.score < 0) {
            this.toastr.warning('Score cannot be negative. Entry cleared.');
            row.score = null;
            row.description = '';
        } else if (row.score != null && maxMark > 0) {
            let percent = (row.score / maxMark) * 100;
            let grade = this.grades.find((g) => percent >= g.minScore && percent <= g.maxScore);
            row.description = grade ? grade.name : '';
        } else {
            row.description = '';
        }
    };

    saveAll = () => {
        if (!this.isAllocatedToSubject) {
            this.toastr.warning('You are not allocated to this subject for the selected class. Marks cannot be saved.');
            return;
        }
        if (this.isPastMarkEntryDeadline) {
            this.toastr.warning('The mark entry deadline for this exam has passed. Contact an administrator if changes are needed.');
            return;
        }
        let rowsToSave = this.scoringRows.filter((r) => r.score != null);
        if (rowsToSave.length === 0) {
            this.toastr.info('Please enter at least one score before saving.');
            return;
        }
        let invalidRows = rowsToSave.filter((r) => r.score > this.selectedExam?.examMark);
        if (invalidRows.length > 0) {
            this.toastr.error(`${invalidRows.length} student(s) have scores exceeding the exam mark of ${this.selectedExam?.examMark}. Please correct before saving.`);
            return;
        }

        Swal.fire({
            title: 'Save all exam results?',
            text: `${rowsToSave.length} result(s) will be saved.`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: 'Save', cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                let batchData = rowsToSave.map((row) => new ExamResult({
                    studentId: row.studentId,
                    examId: this.filterExamId,
                    score: row.score,
                    description: row.description
                }));

                this.examResultSvc.createBatch('/examResults/batch', batchData).subscribe(
                    () => {
                        this.isSaving = false;
                        this.toastr.success(`${rowsToSave.length} result(s) saved successfully!`);
                        this.loadStudents();
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
        if (this.isPastMarkEntryDeadline) {
            this.toastr.warning('The mark entry deadline for this exam has passed. Contact an administrator if changes are needed.');
            return;
        }
        Swal.fire({
            title: 'Delete result?',
            text: `Remove result for ${row.fullName}?`,
            width: 400, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete', cancelButtonText: 'Cancel',
            confirmButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                this.examResultSvc.delete('/examResults', parseInt(row.existingId)).subscribe({
                    next: () => {
                        this.toastr.success(`Result for ${row.fullName} deleted.`);
                        row.existingId = null;
                        row.score = null;
                        row.description = '';
                    },
                    error: (err) => this.toastr.error(err.error?.message || 'Error deleting result.')
                });
            }
        });
    };

    getAverage = (): number => {
        let scored = this.scoringRows.filter((r) => r.score != null);
        if (scored.length === 0) return 0;
        let total = scored.reduce((sum, r) => sum + (r.score || 0), 0);
        let divisor = this.averageMethod === 'all_allocated_students'
            ? this.scoringRows.length
            : scored.length;
        if (divisor === 0) return 0;
        return Math.round((total / divisor) * 100) / 100;
    };

    getScoredCount = (): number => {
        return this.scoringRows.filter((r) => r.score != null).length;
    };

    getExistingCount = (): number => {
        return this.scoringRows.filter((r) => r.existingId != null).length;
    };

    deleteAll = () => {
        if (this.isPastMarkEntryDeadline) {
            this.toastr.warning('The mark entry deadline for this exam has passed. Contact an administrator if changes are needed.');
            return;
        }
        let existingRows = this.scoringRows.filter((r) => r.existingId != null);
        if (existingRows.length === 0) return;

        Swal.fire({
            title: 'Delete all results?',
            text: `${existingRows.length} result(s) will be permanently deleted.`,
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
                        this.loadStudents();
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
