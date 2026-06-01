import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {SchoolExam} from '../../models/school-exam';
import {SchoolExamService} from '../../services/school-exam.service';
import {ExamTypeService} from '../../services/exam-type.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';

/**
 * Manage the exam "event" headers (SchoolExam): create / edit the type, term
 * and schedule, and release the exam. Releasing publishes results to the
 * dashboard summary and (later) notifies parents. The per-class-per-subject
 * detail is registered separately under /cbe/exams/exams/register.
 */
@Component({
    selector: 'app-school-exams',
    templateUrl: './school-exams.component.html'
})
export class SchoolExamsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/cbe/exams/school-exams'], title: 'CBE Exams: School Exams'}
    ];
    dashboardTitle = 'CBE Exams: School Exams';

    // Dropdown sources
    curricula: any[] = [];
    academicYears: any[] = [];
    sessions: any[] = [];
    examTypes: any[] = [];

    // Filter state
    filterCurriculumId: any = null;
    filterAcademicYearId: any = null;
    filterSessionId: any = null;
    filterExamTypeId: any = null;

    // Data
    schoolExams: SchoolExam[] = [];
    isLoading: boolean = false;
    page = 1;
    pageSize = 20;

    // Create / edit modal
    editing: any = null;            // the row being edited, or {} for a new one
    isNew: boolean = false;
    form: {
        sessionId: any;
        examTypeId: any;
        examStartDate: string;
        examEndDate: string;
        examMarkEntryEndDate: string;
        description: string;
    } = {sessionId: null, examTypeId: null, examStartDate: '', examEndDate: '', examMarkEntryEndDate: '', description: ''};
    isSaving: boolean = false;

    constructor(
        private toastr: ToastrService,
        private schoolExamSvc: SchoolExamService,
        private examTypeSvc: ExamTypeService,
        private curriculaSvc: CurriculumService,
        private academicYearSvc: AcademicYearsService,
        private sessionsSvc: SessionsService
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

    // ---- Cascading filters ----------------------------------------------------

    onCurriculumChange = () => {
        this.sessions = [];
        this.filterAcademicYearId = this.filterSessionId = null;
        this.schoolExams = [];
    };

    onAcademicYearChange = () => {
        this.sessions = [];
        this.filterSessionId = null;
        this.schoolExams = [];
        if (!this.filterAcademicYearId || !this.filterCurriculumId) return;
        this.sessionsSvc.get(
            `/sessions/byCurriculumYearId?curriculumId=${this.filterCurriculumId}&academicYearId=${this.filterAcademicYearId}`
        ).subscribe({
            next: (sessions) => {
                this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                this.search();
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    onSessionChange = () => this.search();
    onExamTypeChange = () => this.search();

    search = () => {
        if (!this.filterCurriculumId || !this.filterAcademicYearId) {
            this.schoolExams = [];
            return;
        }
        this.isLoading = true;
        let url = `/schoolExams/examSearch?academicYearId=${this.filterAcademicYearId}&curriculumId=${this.filterCurriculumId}`;
        if (this.filterSessionId) url += `&sessionId=${this.filterSessionId}`;
        if (this.filterExamTypeId) url += `&examTypeId=${this.filterExamTypeId}`;
        this.schoolExamSvc.get(url).subscribe({
            next: (items) => {
                this.schoolExams = items;
                this.page = 1;
                this.isLoading = false;
            },
            error: (err) => {
                this.isLoading = false;
                this.toastr.error(err.error);
            }
        });
    };

    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; this.page = 1; };

    // ---- Create / edit --------------------------------------------------------

    openNew = () => {
        if (!this.filterAcademicYearId) {
            this.toastr.info('Pick a curriculum and year first so the term list is available.');
            return;
        }
        this.isNew = true;
        this.editing = {};
        this.form = {
            sessionId: this.filterSessionId ?? null,
            examTypeId: this.filterExamTypeId ?? null,
            examStartDate: '', examEndDate: '', examMarkEntryEndDate: '', description: ''
        };
    };

    openEdit = (item: any) => {
        this.isNew = false;
        this.editing = item;
        this.form = {
            sessionId: item.sessionId ?? item.session?.id ?? null,
            examTypeId: item.examTypeId ?? item.examType?.id ?? null,
            examStartDate: this.toDateInput(item.examStartDate),
            examEndDate: this.toDateInput(item.examEndDate),
            examMarkEntryEndDate: this.toDateInput(item.examMarkEntryEndDate),
            description: item.description ?? ''
        };
    };

    cancelEdit = () => {
        this.editing = null;
        this.isSaving = false;
    };

    onStartDateChange = () => {
        if (this.form.examStartDate && !this.form.examEndDate) {
            this.form.examEndDate = this.form.examStartDate;
        }
    };

    save = () => {
        if (!this.form.sessionId) { this.toastr.warning('Select the term.'); return; }
        if (!this.form.examTypeId) { this.toastr.warning('Select the exam type.'); return; }
        if (!this.form.examStartDate || !this.form.examEndDate) {
            this.toastr.warning('Start and end dates are required.'); return;
        }
        if (this.form.examEndDate < this.form.examStartDate) {
            this.toastr.warning('End date cannot be earlier than start date.'); return;
        }
        if (this.form.examMarkEntryEndDate && this.form.examMarkEntryEndDate < this.form.examEndDate) {
            this.toastr.warning('Mark entry deadline cannot be earlier than the exam end date.'); return;
        }

        let payload = new SchoolExam({
            sessionId: this.form.sessionId,
            examTypeId: this.form.examTypeId,
            examStartDate: this.form.examStartDate,
            examEndDate: this.form.examEndDate,
            examMarkEntryEndDate: this.form.examMarkEntryEndDate || this.form.examEndDate,
            description: this.form.description || null
        });

        this.isSaving = true;
        let req = this.isNew
            ? this.schoolExamSvc.create('/schoolExams', payload)
            : this.schoolExamSvc.update('/schoolExams', new SchoolExam({...payload, id: this.editing.id}));
        req.subscribe({
            next: () => {
                this.isSaving = false;
                this.toastr.success(this.isNew ? 'School exam created.' : 'School exam updated.');
                this.cancelEdit();
                this.search();
            },
            error: (err) => {
                this.isSaving = false;
                this.toastr.error(err.error?.message || err.error || 'Error saving school exam.');
            }
        });
    };

    // ---- Release --------------------------------------------------------------

    toggleRelease = (item: any) => {
        let releasing = !item.isReleased;
        Swal.fire({
            title: releasing ? 'Release this exam?' : 'Revert release?',
            text: releasing
                ? 'Released results become visible on the dashboard summary and will be sent to parents (once notifications are enabled).'
                : 'Reverting will hide the results from the dashboard summary again.',
            width: 460, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true,
            confirmButtonText: releasing ? 'Release' : 'Revert',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (!result.value) return;
            this.schoolExamSvc.release(parseInt(item.id), releasing).subscribe({
                next: (updated) => {
                    item.isReleased = updated.isReleased;
                    item.releasedBy = updated.releasedBy;
                    item.releasedDate = updated.releasedDate;
                    this.toastr.success(releasing ? 'Exam released.' : 'Release reverted.');
                },
                error: (err) => this.toastr.error(err.error?.message || err.error || 'Error updating release state.')
            });
        });
    };

    // ---- Delete ---------------------------------------------------------------

    deleteItem = (item: SchoolExam) => {
        Swal.fire({
            title: 'Delete this school exam?',
            text: 'You can only delete it once its registered class/subject exams have been removed.',
            width: 420, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete',
            cancelButtonText: 'Cancel', confirmButtonColor: '#d33'
        }).then((result) => {
            if (!result.value) return;
            this.schoolExamSvc.delete('/schoolExams', parseInt(item.id)).subscribe({
                next: () => {
                    this.search();
                    this.toastr.success('School exam deleted!');
                },
                error: (err) => this.toastr.error(err.error?.message || err.error || 'Error deleting school exam.')
            });
        });
    };

    private toDateInput(raw: any): string {
        if (!raw) return '';
        let d = new Date(raw);
        if (isNaN(d.getTime())) return '';
        let year = d.getFullYear();
        let month = String(d.getMonth() + 1).padStart(2, '0');
        let day = String(d.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    }
}
