import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {Exam} from '../../models/exam';
import {ExamService} from '../../services/exam.service';
import {ExamTypeService} from '../../services/exam-type.service';
import {SchoolExamService} from '../../services/school-exam.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {EducationLevelSubjectService} from '@/academics/services/education-level-subject.service';
import {ActivatedRoute} from '@angular/router';

@Component({
    selector: 'app-exams',
    templateUrl: './exams.component.html',
    styleUrl: './exams.component.scss'
})
export class ExamsComponent implements OnInit {
    isAuthLoading: boolean;
    querySource: string = '';

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/exams/exams'], title: 'CBE Exams: Register'},
        {link: ['/cbe/exams/exams/register'], title: 'New Exam(s)'}
    ];
    dashboardTitle = 'CBE Exams: Register New Exam(s)';

    page = 1;
    pageSize = 10;

    // Dropdown data
    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    subjects: any[] = [];
    examTypes: any[] = [];
    educationLevels: any[] = [];
    learningLevels: any[] = [];

    // educationLevelId -> Set of subjectIds registered for that level in the
    // current academic year. Used by saveExams() to drop (class, subject)
    // combinations where the subject isn't taught at the class's level -
    // matters most when the user picks "All Education Levels", because the
    // de-duplicated subject list above mixes subjects across all levels and
    // a naive class × subject cross-product would create irrelevant exams.
    validSubjectsByEducationLevel = new Map<number, Set<number>>();

    // Filter selections
    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterEducationLevelId: any = null;

    // Multi-select for batch creation
    selectedClassIds: number[] = [];
    selectedSubjectIds: number[] = [];
    examMark: number = 100;
    examDescription: string = '';

    // School exam (header) selection. The exam type and schedule now live on
    // the chosen SchoolExam; this form only attaches class/subject detail rows.
    schoolExams: any[] = [];
    selectedSchoolExamId: any = null;
    selectedSchoolExam: any = null;

    // Registered exams
    exams: Exam[] = [];
    isSaving: boolean = false;

    // Multi-select for bulk delete on the registered-exams table.
    selectedExamIds: number[] = [];
    isDeleting: boolean = false;

    constructor(
        private toastr: ToastrService,
        private examSvc: ExamService,
        private examTypeSvc: ExamTypeService,
        private schoolExamSvc: SchoolExamService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private schoolClassesSvc: SchoolClassesService,
        private learningLevelSvc: LearningLevelsService,
        private educationLevelSvc: EducationLevelService,
        private educationLevelSubjectSvc: EducationLevelSubjectService,
        private route: ActivatedRoute
    ) {}

    parseInt = parseInt;

    pageSizeChanged = (pageSize: number) => { this.pageSize = pageSize; };
    pageChanged = (page: number) => { this.page = page; };

    ngOnInit(): void {
        this.querySource = this.route.snapshot.queryParamMap.get('source') || '';
        this.refreshItems();
    }

    refreshItems() {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.academicYearSvc.get('/academicYears'),
            this.examTypeSvc.get('/examTypes')
        ]).subscribe({
            next: ([curricula, academicYears, examTypes]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears.filter((y) => y.status === true).sort((a, b) => a.rank - b.rank);
                this.examTypes = examTypes.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    onCurriculumChange = () => {
        this.sessions = this.schoolClasses = this.subjects = this.educationLevels = [];
        this.exams = [];
        this.filterAcademicYearId = this.filterSessionId = this.filterEducationLevelId = null;
        this.selectedClassIds = [];
        this.selectedSubjectIds = [];
        if (!this.filterCurriculumId) return;
        this.educationLevelSvc.get(`/educationLevels/byCurriculumId?curriculumId=${this.filterCurriculumId}`).subscribe({
            next: (levels) => { this.educationLevels = levels.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onAcademicYearChange = () => {
        this.sessions = this.schoolClasses = this.subjects = [];
        this.exams = [];
        this.filterSessionId = this.filterEducationLevelId = null;
        this.selectedClassIds = [];
        this.selectedSubjectIds = [];
        if (!this.filterAcademicYearId || !this.filterCurriculumId) return;
        this.sessionsSvc.get(`/sessions/byCurriculumYearId?curriculumId=${this.filterCurriculumId}&academicYearId=${this.filterAcademicYearId}`).subscribe({
            next: (sessions) => { this.sessions = sessions.sort((a, b) => a.rank - b.rank); },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onSessionChange = () => {
        this.exams = [];
        this.schoolExams = [];
        this.selectedSchoolExamId = null;
        this.selectedSchoolExam = null;
        if (!this.filterSessionId) return;
        // Load the school-exam headers for this term so the user can attach
        // class/subject detail rows to one of them.
        this.schoolExamSvc.get(
            `/schoolExams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${this.filterSessionId}`
        ).subscribe({
            next: (items) => { this.schoolExams = items; },
            error: (err) => this.toastr.error(err.error)
        });
        this.searchExams();
    };

    onSchoolExamChange = () => {
        this.selectedSchoolExam = this.schoolExams.find(
            (se) => se.id == this.selectedSchoolExamId
        ) || null;
    };

    onEducationLevelChange = () => {
        this.schoolClasses = this.subjects = [];
        this.selectedClassIds = [];
        this.selectedSubjectIds = [];
        this.exams = [];
        this.validSubjectsByEducationLevel = new Map<number, Set<number>>();
        if (!this.filterEducationLevelId || !this.filterAcademicYearId) return;

        if (this.filterEducationLevelId === 'all') {
            this.loadAllEducationLevelData();
            return;
        }

        forkJoin([
            this.schoolClassesSvc.get(`/schoolClasses/byEducationLevelYearId?educationLevelId=${this.filterEducationLevelId}&academicYearId=${this.filterAcademicYearId}`),
            this.educationLevelSubjectSvc.getByEducationLevelAndAcademicYear(this.filterEducationLevelId, this.filterAcademicYearId)
        ]).subscribe({
            next: ([schoolClasses, elSubjects]) => {
                this.schoolClasses = schoolClasses;
                this.subjects = elSubjects.map((es) => es.subject)
                    .filter((s) => s.examinable !== false)
                    .sort((a, b) => a.rank - b.rank);

                // Record the valid (level -> subjectIds) mapping so saveExams
                // can validate combinations. In single-level mode this is a
                // no-op safety net since every subject in the picker is
                // already correct for the level.
                let validIds = new Set<number>(
                    elSubjects
                        .filter((es) => es.subject?.examinable !== false)
                        .map((es) => parseInt(es.subject?.id))
                        .filter((id) => !isNaN(id))
                );
                this.validSubjectsByEducationLevel.set(parseInt(this.filterEducationLevelId), validIds);
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    loadAllEducationLevelData = () => {
        if (this.educationLevels.length === 0) return;

        let classRequests = this.educationLevels.map((el) =>
            this.schoolClassesSvc.get(`/schoolClasses/byEducationLevelYearId?educationLevelId=${el.id}&academicYearId=${this.filterAcademicYearId}`)
        );
        let subjectRequests = this.educationLevels.map((el) =>
            this.educationLevelSubjectSvc.getByEducationLevelAndAcademicYear(parseInt(el.id), this.filterAcademicYearId)
        );

        forkJoin([
            forkJoin(classRequests),
            forkJoin(subjectRequests)
        ]).subscribe({
            next: ([classResults, subjectResults]: any) => {
                // Flatten classes
                let allClasses: any[] = [];
                classResults.forEach((cls: any[]) => { allClasses = allClasses.concat(cls); });
                this.schoolClasses = allClasses.sort((a, b) =>
                    (a.learningLevel?.rank || 0) - (b.learningLevel?.rank || 0)
                );

                // Build the per-level subject lookup BEFORE flattening so the
                // (level -> subjectIds) association is preserved. saveExams
                // uses this to skip cross-level (class, subject) pairs that
                // would otherwise slip through the deduped subject list below.
                this.validSubjectsByEducationLevel = new Map<number, Set<number>>();
                this.educationLevels.forEach((el, idx) => {
                    let levelId = parseInt(el.id);
                    let levelSubjects = (subjectResults[idx] as any[]) || [];
                    let ids = new Set<number>(
                        levelSubjects
                            .filter((es) => es.subject?.examinable !== false)
                            .map((es) => parseInt(es.subject?.id))
                            .filter((id) => !isNaN(id))
                    );
                    if (!isNaN(levelId)) this.validSubjectsByEducationLevel.set(levelId, ids);
                });

                // Flatten subjects, dedupe by id, exclude non-examinable
                let allSubjects: any[] = [];
                subjectResults.forEach((els: any[]) => {
                    els.forEach((es) => {
                        if (es.subject && es.subject.examinable !== false) {
                            allSubjects.push(es.subject);
                        }
                    });
                });
                let seen = new Set<string>();
                this.subjects = allSubjects
                    .filter((s) => {
                        let key = s.id?.toString();
                        if (!key || seen.has(key)) return false;
                        seen.add(key);
                        return true;
                    })
                    .sort((a, b) => (a.rank || 0) - (b.rank || 0));
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    toggleClass = (classId: number) => {
        let idx = this.selectedClassIds.indexOf(classId);
        if (idx > -1) this.selectedClassIds.splice(idx, 1);
        else this.selectedClassIds.push(classId);
    };

    toggleSubject = (subjectId: number) => {
        let idx = this.selectedSubjectIds.indexOf(subjectId);
        if (idx > -1) this.selectedSubjectIds.splice(idx, 1);
        else this.selectedSubjectIds.push(subjectId);
    };

    selectAllClasses = () => {
        if (this.selectedClassIds.length === this.schoolClasses.length) {
            this.selectedClassIds = [];
        } else {
            this.selectedClassIds = this.schoolClasses.map((sc) => parseInt(sc.id));
        }
    };

    selectAllSubjects = () => {
        if (this.selectedSubjectIds.length === this.subjects.length) {
            this.selectedSubjectIds = [];
        } else {
            this.selectedSubjectIds = this.subjects.map((s) => parseInt(s.id));
        }
    };

    isClassSelected = (classId: number): boolean => {
        return this.selectedClassIds.includes(classId);
    };

    isSubjectSelected = (subjectId: number): boolean => {
        return this.selectedSubjectIds.includes(subjectId);
    };

    saveExams = () => {
        if (!this.filterSessionId) {
            this.toastr.info('Please select a session.');
            return;
        }
        if (!this.selectedSchoolExamId) {
            this.toastr.info('Please select a school exam (the type and schedule come from it).');
            return;
        }
        if (this.selectedClassIds.length === 0) {
            this.toastr.info('Please select at least one class.');
            return;
        }
        if (this.selectedSubjectIds.length === 0) {
            this.toastr.info('Please select at least one subject.');
            return;
        }
        if (this.examMark == null || this.examMark <= 0) {
            this.toastr.info('Please enter a positive exam mark.');
            return;
        }

        // Build the actual (class, subject) plan up-front. We drop any pair
        // whose subject isn't allocated to the class's education level - this
        // matters when "All Education Levels" is selected, where the subject
        // picker mixes subjects from every level and the naive cross-product
        // would create exams for subjects not taught at the class's level.
        let plan: {classId: number; subjectId: number}[] = [];
        let skipped = 0;
        for (let classId of this.selectedClassIds) {
            let schoolClass = this.schoolClasses.find((sc) => parseInt(sc.id) === classId);
            let levelId = parseInt(schoolClass?.learningLevel?.educationLevelId);
            let validForLevel = !isNaN(levelId) ? this.validSubjectsByEducationLevel.get(levelId) : null;
            for (let subjectId of this.selectedSubjectIds) {
                if (validForLevel && !validForLevel.has(subjectId)) {
                    skipped++;
                    continue;
                }
                plan.push({classId, subjectId});
            }
        }

        if (plan.length === 0) {
            this.toastr.info('No valid (class, subject) combinations to register - the selected subjects are not allocated to the selected education levels.');
            return;
        }

        let totalExams = plan.length;
        let skippedSuffix = skipped > 0
            ? ` (${skipped} skipped - subject not allocated to that education level)`
            : '';

        Swal.fire({
            title: 'Register exams?',
            text: `${totalExams} exam(s) will be created from ${this.selectedClassIds.length} class(es) × ${this.selectedSubjectIds.length} subject(s)${skippedSuffix}.`,
            width: 460, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: 'Save', cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                let requests = plan.map(({classId, subjectId}) => {
                    let exam = new Exam({
                        examMark: this.examMark,
                        description: this.examDescription || null,
                        schoolExamId: this.selectedSchoolExamId,
                        schoolClassId: classId,
                        subjectId: subjectId
                    });
                    return this.examSvc.create('/exams', exam);
                });
                forkJoin(requests).subscribe(
                    () => {
                        this.isSaving = false;
                        let msg = `${totalExams} exam(s) registered successfully!`;
                        if (skipped > 0) {
                            msg += ` ${skipped} combination(s) skipped - subject not allocated to that education level.`;
                        }
                        this.toastr.success(msg);
                        this.searchExams();
                        this.selectedClassIds = [];
                        this.selectedSubjectIds = [];
                    },
                    (err) => {
                        this.isSaving = false;
                        this.toastr.error(err.error?.message || 'Error saving exams.');
                    }
                );
            }
        });
    };

    searchExams = () => {
        if (!this.filterSessionId || !this.filterAcademicYearId || !this.filterCurriculumId) return;
        let url = `/exams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${this.filterSessionId}`;
        this.examSvc.get(url).subscribe({
            next: (exams) => {
                this.exams = exams;
                this.selectedExamIds = [];  // reset selection when the underlying list changes
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    // ---- Multi-select helpers for bulk delete -------------------------------
    isExamSelected = (examId: number): boolean => this.selectedExamIds.includes(examId);
    toggleExamSelection = (examId: number) => {
        const idx = this.selectedExamIds.indexOf(examId);
        if (idx > -1) this.selectedExamIds.splice(idx, 1);
        else this.selectedExamIds.push(examId);
    };
    allExamsSelected = (): boolean =>
        this.exams.length > 0 && this.selectedExamIds.length === this.exams.length;
    toggleAllExams = () => {
        this.selectedExamIds = this.allExamsSelected()
            ? []
            : this.exams.map((e) => parseInt(e.id));
    };

    deleteSelectedExams = () => {
        const ids = [...this.selectedExamIds];
        if (ids.length === 0) {
            this.toastr.info('Select at least one exam to delete.');
            return;
        }
        Swal.fire({
            title: `Delete ${ids.length} exam(s)?`,
            text: `This will permanently delete the selected exam(s) and any linked exam results. This cannot be undone.`,
            width: 460, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true, confirmButtonText: `Delete ${ids.length}`,
            cancelButtonText: 'Cancel', confirmButtonColor: '#d33'
        }).then((result) => {
            if (!result.value) return;
            this.isDeleting = true;
            const requests = ids.map((id) => this.examSvc.delete('/exams', id));
            forkJoin(requests).subscribe({
                next: () => {
                    this.isDeleting = false;
                    this.toastr.success(`${ids.length} exam(s) deleted.`);
                    this.searchExams();
                },
                error: (err) => {
                    this.isDeleting = false;
                    this.toastr.error(err.error?.message || 'Error deleting exams. Some may have linked exam results blocking the delete.');
                    // Refresh anyway so we show what's actually left.
                    this.searchExams();
                }
            });
        });
    };

    deleteItem(exam: Exam) {
        Swal.fire({
            title: 'Delete this exam?',
            text: 'This action cannot be undone.',
            width: 400, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete', cancelButtonText: 'Cancel', confirmButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                this.examSvc.delete('/exams', parseInt(exam.id)).subscribe(
                    () => { this.searchExams(); this.toastr.success('Exam deleted!'); },
                    (err) => this.toastr.error(err.error)
                );
            }
        });
    }
}
