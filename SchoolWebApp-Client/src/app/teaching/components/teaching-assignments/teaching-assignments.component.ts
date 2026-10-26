import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {
    DocStatus,
    STATUS_BADGES,
    STATUS_LABELS,
    TeachingAssignment,
    TeachingContext,
    TeachingSession
} from '@/teaching/models/teaching-models';
import {SchemesOfWorkService, TeachingDocumentsService} from '@/teaching/services/teaching-services';
import {blobError, safeFileName, saveBlob} from '@/teaching/utils/download';

export interface AssignmentOpened {
    assignment: TeachingAssignment;
    session: TeachingSession;
    context: TeachingContext;
}

const SESSION_KEY = 'teaching.sessionId';

/**
 * The subjects and classes a teacher is allocated for a term, with where each
 * scheme stands. Heads of department and whole-school staff can switch to
 * everything they oversee. The page decides what "Open" does.
 */
@Component({
    selector: 'app-teaching-assignments',
    templateUrl: './teaching-assignments.component.html'
})
export class TeachingAssignmentsComponent implements OnInit {
    @Input() purpose: 'schemes' | 'plans' | 'record' = 'schemes';
    @Output() opened = new EventEmitter<AssignmentOpened>();
    @Output() contextLoaded = new EventEmitter<TeachingContext>();

    context: TeachingContext | null = null;
    sessionId: number | null = null;
    scope: 'mine' | 'oversee' = 'mine';
    rows: TeachingAssignment[] = [];
    search = '';
    loading = false;

    // Filters; options come from the loaded rows.
    staffDetailsId: number | null = null;
    departmentId: number | null = null;
    learningLevelId: number | null = null;
    subjectId: number | null = null;
    status: DocStatus | null = null;
    page = 1;
    pageSize = 15;
    statusOptions: DocStatus[] = ['NotStarted', 'Draft', 'Submitted', 'Returned', 'Rejected', 'Approved'];

    statusLabels = STATUS_LABELS;
    statusBadges = STATUS_BADGES;

    constructor(private svc: TeachingDocumentsService, private schemeSvc: SchemesOfWorkService,
                private toastr: ToastrService) {}

    ngOnInit(): void {
        this.svc.context().subscribe({
            next: (c) => {
                this.context = c;
                this.contextLoaded.emit(c);
                let saved: number | null = null;
                try { saved = +(sessionStorage.getItem(SESSION_KEY) || 0) || null; } catch { /* storage blocked */ }
                this.sessionId = c.sessions.some((s) => s.id === saved) ? saved : c.currentSessionId;
                // An account not linked to a staff record has no subjects of its own.
                this.scope = c.staffDetailsId ? 'mine' : 'oversee';
                this.load();
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading your teaching details.')
        });
    }

    get canOversee(): boolean {
        return !!this.context && (this.context.wholeSchool || this.context.hodDepartmentIds.length > 0);
    }

    get session(): TeachingSession | undefined {
        return this.context?.sessions.find((s) => s.id === this.sessionId);
    }

    load() {
        if (!this.sessionId) return;
        try { sessionStorage.setItem(SESSION_KEY, String(this.sessionId)); } catch { /* storage blocked */ }
        this.loading = true;
        this.svc.assignments(this.sessionId, this.scope).subscribe({
            next: (rows) => {
                this.rows = rows;
                this.loading = false;
                // Drop filters that no longer match anything in the new list.
                if (!rows.some((r) => r.staffDetailsId === this.staffDetailsId)) this.staffDetailsId = null;
                if (!rows.some((r) => r.departmentId === this.departmentId)) this.departmentId = null;
                if (!rows.some((r) => r.learningLevelId === this.learningLevelId)) this.learningLevelId = null;
                if (!rows.some((r) => r.subjectId === this.subjectId)) this.subjectId = null;
                this.page = 1;
            },
            error: (err) => { this.loading = false; this.toastr.error(err.error?.message || 'Error loading subject allocations.'); }
        });
    }

    get teachers() { return this.options((r) => r.staffDetailsId, (r) => r.staffName); }
    get departments() { return this.options((r) => r.departmentId, (r) => r.departmentName || '-'); }
    get grades() { return this.options((r) => r.learningLevelId, (r) => r.learningLevelName); }
    // Subjects in their set order (Subjects > Rank), narrowed to the chosen department.
    get subjects() {
        return this.options((r) => r.subjectId, (r) => r.subjectName,
            (r) => this.departmentId == null || r.departmentId === this.departmentId, (r) => r.subjectRank);
    }

    // Sorted by rank when one is given, else by name.
    private options(id: (r: TeachingAssignment) => number, name: (r: TeachingAssignment) => string,
                    where: (r: TeachingAssignment) => boolean = () => true,
                    rank?: (r: TeachingAssignment) => number): {id: number; name: string}[] {
        let map = new Map<number, {id: number; name: string; rank: number}>();
        this.rows.filter(where).forEach((r) => {
            if (!map.has(id(r))) map.set(id(r), {id: id(r), name: name(r), rank: rank ? rank(r) : 0});
        });
        return Array.from(map.values())
            .sort((a, b) => a.rank - b.rank || a.name.localeCompare(b.name, undefined, {numeric: true}))
            .map(({id, name}) => ({id, name}));
    }

    get filtered(): TeachingAssignment[] {
        let q = this.search.trim().toLowerCase();
        return this.rows.filter((r) =>
            (this.staffDetailsId == null || r.staffDetailsId === this.staffDetailsId)
            && (this.departmentId == null || r.departmentId === this.departmentId)
            && (this.learningLevelId == null || r.learningLevelId === this.learningLevelId)
            && (this.subjectId == null || r.subjectId === this.subjectId)
            && (this.status == null || r.schemeStatus === this.status)
            && (!q || [r.staffName, r.subjectName, r.className, r.departmentName]
                .some((v) => (v || '').toLowerCase().includes(q))));
    }

    get paged(): TeachingAssignment[] {
        return this.filtered.slice((this.page - 1) * this.pageSize, this.page * this.pageSize);
    }

    get hasFilters(): boolean {
        return this.staffDetailsId != null || this.departmentId != null || this.learningLevelId != null
            || this.subjectId != null || this.status != null || !!this.search.trim();
    }

    clearFilters() {
        this.staffDetailsId = this.departmentId = this.learningLevelId = this.subjectId = null;
        this.status = null;
        this.search = '';
        this.page = 1;
    }

    isMine(a: TeachingAssignment): boolean {
        return !!this.context && a.staffDetailsId === this.context.staffDetailsId;
    }

    // What the row's button says, or null when there is nothing to open.
    action(a: TeachingAssignment): string | null {
        if (a.schemeOfWorkId) {
            return this.purpose === 'schemes' ? 'Open scheme' : this.purpose === 'plans' ? 'Lesson plans' : 'Record of work';
        }
        if (this.purpose === 'schemes' && (this.isMine(a) || this.context?.isAdmin)) return 'Start scheme';
        return null;
    }

    // ---------- Excel workbook (schemes) ----------

    showImport = false;
    downloading = false;

    // Whose workbook: your own on "My subjects", the teacher picked in the filter otherwise.
    get workbookStaffId(): number | null {
        return this.scope === 'mine' ? this.context?.staffDetailsId ?? null : this.staffDetailsId;
    }

    get workbookTitle(): string {
        if (this.workbookStaffId) return 'Every subject and class this term, one sheet each, to fill in Excel';
        return 'Pick a teacher in the filter first - the workbook holds one teacher\'s subjects';
    }

    downloadWorkbook() {
        let staffId = this.workbookStaffId;
        let session = this.session;
        if (!staffId || !session) return;
        let teacher = this.rows.find((r) => r.staffDetailsId === staffId)?.staffName || this.context?.staffName || 'teacher';
        this.downloading = true;
        this.schemeSvc.teacherWorkbook(session.id, this.scope === 'mine' ? null : staffId).subscribe({
            next: (blob) => {
                this.downloading = false;
                saveBlob(blob, safeFileName(`Schemes of work - ${teacher} - ${session!.name} ${session!.academicYearName}`) + '.xlsx');
            },
            error: async (err) => {
                this.downloading = false;
                this.toastr.error(await blobError(err, 'Error preparing the workbook.'));
            }
        });
    }

    onImportClosed(saved: boolean) {
        this.showImport = false;
        if (saved) this.load();
    }

    open(a: TeachingAssignment) {
        if (!this.context || !this.session) return;
        this.opened.emit({assignment: a, session: this.session, context: this.context});
    }
}
