import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {Exam} from '../../models/exam';
import {ExamService} from '../../services/exam.service';
import {ExamTypeService} from '../../services/exam-type.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';

/**
 * Browse / search / delete page for registered exams. Replaces the embedded
 * list that used to live underneath the registration form. The form itself
 * lives at /cbe/exams/exams/register and is reached via the "Register New
 * Exam(s)" button below.
 */
@Component({
    selector: 'app-exam-register-list',
    templateUrl: './exam-register-list.component.html'
})
export class ExamRegisterListComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/exams/exams'], title: 'CBE Exams: Register'}
    ];
    dashboardTitle = 'CBE Exams: Register';

    // Dropdown sources
    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    schoolClasses: any[] = [];
    examTypes: any[] = [];

    // Filter state
    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterSchoolClassId: any = null;
    filterExamTypeId: any = null;
    searchText: string = '';

    // Data
    exams: Exam[] = [];
    isLoading: boolean = false;

    // Selection / paging
    selectedExamIds: number[] = [];
    isDeleting: boolean = false;
    page = 1;
    pageSize = 20;

    // Edit modal state. We only allow editing the schedule dates and the mark
    // out of (i.e. examMark). Everything else (type, class, subject, session)
    // stays read-only because changing them would silently re-key existing
    // ExamResult rows to the wrong exam.
    editingExam: any = null;
    editForm: {
        examMark: number;
        examStartDate: string;
        examEndDate: string;
        examMarkEntryEndDate: string;
        description: string;
    } = {examMark: 100, examStartDate: '', examEndDate: '', examMarkEntryEndDate: '', description: ''};
    isSavingEdit: boolean = false;

    constructor(
        private toastr: ToastrService,
        private examSvc: ExamService,
        private examTypeSvc: ExamTypeService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService,
        private schoolClassesSvc: SchoolClassesService
    ) {}

    parseInt = parseInt;

    ngOnInit(): void {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.academicYearSvc.get('/academicYears'),
            this.examTypeSvc.get('/examTypes')
        ]).subscribe({
            next: ([curricula, academicYears, examTypes]) => {
                this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                this.academicYears = academicYears
                    .filter((y) => y.status === true)
                    .sort((a, b) => a.rank - b.rank);
                this.examTypes = examTypes.sort((a, b) => a.rank - b.rank);
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    // ---- Cascading filter handlers --------------------------------------------

    onCurriculumChange = () => {
        this.sessions = this.schoolClasses = [];
        this.filterAcademicYearId = this.filterSessionId = this.filterSchoolClassId = null;
        this.exams = [];
        this.selectedExamIds = [];
    };

    onAcademicYearChange = () => {
        this.sessions = this.schoolClasses = [];
        this.filterSessionId = this.filterSchoolClassId = null;
        this.exams = [];
        this.selectedExamIds = [];
        if (!this.filterAcademicYearId || !this.filterCurriculumId) return;

        forkJoin([
            this.sessionsSvc.get(
                `/sessions/byCurriculumYearId?curriculumId=${this.filterCurriculumId}&academicYearId=${this.filterAcademicYearId}`
            ),
            this.schoolClassesSvc.get(`/schoolClasses/byAcademicYearId/${this.filterAcademicYearId}`)
        ]).subscribe({
            next: ([sessions, schoolClasses]) => {
                this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                this.schoolClasses = schoolClasses.sort((a, b) => {
                    let nameA = (a.learningLevel?.rank ?? 999);
                    let nameB = (b.learningLevel?.rank ?? 999);
                    return nameA - nameB;
                });
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onSessionChange = () => {
        this.searchExams();
    };

    onClassChange = () => {
        this.searchExams();
    };

    onExamTypeChange = () => {
        // No reload needed — exam type filter is applied client-side after the
        // session-scoped fetch.
    };

    /**
     * Pulls all exams for the selected (curriculum, year, session) and stores
     * them locally. The class / exam-type / search-text filters are then
     * applied in-memory via `filteredExams`.
     */
    searchExams = () => {
        if (!this.filterCurriculumId || !this.filterAcademicYearId || !this.filterSessionId) {
            this.exams = [];
            return;
        }
        this.isLoading = true;
        let url = `/exams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}&sessionId=${this.filterSessionId}`;
        this.examSvc.get(url).subscribe({
            next: (exams) => {
                this.exams = exams;
                this.selectedExamIds = [];
                this.page = 1;
                this.isLoading = false;
            },
            error: (err) => {
                this.isLoading = false;
                this.toastr.error(err.error);
            }
        });
    };

    /**
     * Applies the in-memory filters (class, exam type, search) on top of
     * `exams`. Search is case-insensitive substring match against exam type
     * name, subject name, class name, and description.
     */
    get filteredExams(): Exam[] {
        let list = this.exams;
        if (this.filterSchoolClassId) {
            list = list.filter((e) => (e.schoolClassId ?? (e as any).schoolClass?.id) == this.filterSchoolClassId);
        }
        if (this.filterExamTypeId) {
            list = list.filter((e) => (e.examTypeId ?? (e as any).examType?.id) == this.filterExamTypeId);
        }
        let q = (this.searchText || '').trim().toLowerCase();
        if (q) {
            list = list.filter((e: any) => {
                let typeName = (e.examType?.name || '').toLowerCase();
                let subjectName = (e.subject?.name || '').toLowerCase();
                let className = (e.schoolClass?.name || '').toLowerCase();
                let desc = (e.description || '').toLowerCase();
                return typeName.includes(q)
                    || subjectName.includes(q)
                    || className.includes(q)
                    || desc.includes(q);
            });
        }
        return list;
    }

    // ---- Selection + paging ---------------------------------------------------

    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; this.page = 1; };
    onSearchChanged = () => { this.page = 1; };

    isExamSelected = (id: number) => this.selectedExamIds.includes(id);
    toggleExamSelection = (id: number) => {
        let idx = this.selectedExamIds.indexOf(id);
        if (idx > -1) this.selectedExamIds.splice(idx, 1);
        else this.selectedExamIds.push(id);
    };
    allFilteredSelected = (): boolean => {
        let visible = this.filteredExams.map((e) => parseInt(e.id));
        return visible.length > 0 && visible.every((id) => this.selectedExamIds.includes(id));
    };
    toggleAllFiltered = () => {
        let visibleIds = this.filteredExams.map((e) => parseInt(e.id));
        if (this.allFilteredSelected()) {
            this.selectedExamIds = this.selectedExamIds.filter((id) => !visibleIds.includes(id));
        } else {
            for (let id of visibleIds) {
                if (!this.selectedExamIds.includes(id)) this.selectedExamIds.push(id);
            }
        }
    };

    // ---- Delete ---------------------------------------------------------------

    deleteSelectedExams = () => {
        let ids = [...this.selectedExamIds];
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
            let requests = ids.map((id) => this.examSvc.delete('/exams', id));
            forkJoin(requests).subscribe({
                next: () => {
                    this.isDeleting = false;
                    this.toastr.success(`${ids.length} exam(s) deleted.`);
                    this.searchExams();
                },
                error: (err) => {
                    this.isDeleting = false;
                    this.toastr.error(err.error?.message
                        || 'Error deleting exams. Some may have linked exam results blocking the delete.');
                    this.searchExams();
                }
            });
        });
    };

    // ---- Edit modal ----------------------------------------------------------

    /** Opens the edit modal for the given exam, pre-filling the editable fields. */
    openEdit = (exam: any) => {
        this.editingExam = exam;
        this.editForm = {
            examMark: exam.examMark ?? 100,
            examStartDate: this.toDateInput(exam.examStartDate),
            examEndDate: this.toDateInput(exam.examEndDate),
            examMarkEntryEndDate: this.toDateInput(exam.examMarkEntryEndDate),
            description: exam.description ?? ''
        };
    };

    cancelEdit = () => {
        this.editingExam = null;
        this.isSavingEdit = false;
    };

    saveEdit = () => {
        if (!this.editingExam) return;

        // Local validation: enforce date ordering (start <= end <= deadline)
        // and a positive examMark so we never submit nonsense to the API.
        if (this.editForm.examMark == null || this.editForm.examMark <= 0) {
            this.toastr.warning('Exam mark must be a positive number.');
            return;
        }
        if (!this.editForm.examStartDate || !this.editForm.examEndDate) {
            this.toastr.warning('Start and end dates are required.');
            return;
        }
        if (this.editForm.examEndDate < this.editForm.examStartDate) {
            this.toastr.warning('End date cannot be earlier than start date.');
            return;
        }
        if (this.editForm.examMarkEntryEndDate
            && this.editForm.examMarkEntryEndDate < this.editForm.examEndDate) {
            this.toastr.warning('Mark entry deadline cannot be earlier than the exam end date.');
            return;
        }

        // Build the payload the way the API expects: keep the immutable
        // foreign-key fields (examTypeId, schoolClassId, subjectId, sessionId)
        // from the original record so the server-side update only changes the
        // fields the user is allowed to touch.
        let payload = new Exam({
            id: this.editingExam.id,
            examMark: this.editForm.examMark,
            examStartDate: this.editForm.examStartDate,
            examEndDate: this.editForm.examEndDate,
            examMarkEntryEndDate: this.editForm.examMarkEntryEndDate || null,
            description: this.editForm.description || null,
            examTypeId: this.editingExam.examTypeId ?? this.editingExam.examType?.id,
            schoolClassId: this.editingExam.schoolClassId ?? this.editingExam.schoolClass?.id,
            subjectId: this.editingExam.subjectId ?? this.editingExam.subject?.id,
            sessionId: this.editingExam.sessionId ?? this.editingExam.session?.id
        });

        this.isSavingEdit = true;
        this.examSvc.update('/exams', payload).subscribe({
            next: () => {
                this.isSavingEdit = false;
                this.toastr.success('Exam updated.');
                this.cancelEdit();
                this.searchExams();
            },
            error: (err) => {
                this.isSavingEdit = false;
                this.toastr.error(err.error?.message || 'Error updating exam.');
            }
        });
    };

    /**
     * Coerces server-side date strings (ISO, with or without time) into the
     * `yyyy-MM-dd` format required by `<input type="date">`.
     */
    private toDateInput(raw: any): string {
        if (!raw) return '';
        let d = new Date(raw);
        if (isNaN(d.getTime())) return '';
        let year = d.getFullYear();
        let month = String(d.getMonth() + 1).padStart(2, '0');
        let day = String(d.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }

    deleteItem = (exam: Exam) => {
        Swal.fire({
            title: 'Delete this exam?',
            text: 'This action cannot be undone.',
            width: 400, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete',
            cancelButtonText: 'Cancel', confirmButtonColor: '#d33'
        }).then((result) => {
            if (!result.value) return;
            this.examSvc.delete('/exams', parseInt(exam.id)).subscribe({
                next: () => {
                    this.searchExams();
                    this.toastr.success('Exam deleted!');
                },
                error: (err) => this.toastr.error(err.error)
            });
        });
    };
}
