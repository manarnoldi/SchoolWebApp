import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {
    STATUS_BADGES,
    STATUS_LABELS,
    TeachingAssignment,
    TeachingContext,
    TeachingSession
} from '@/teaching/models/teaching-models';
import {TeachingDocumentsService} from '@/teaching/services/teaching-services';

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

    statusLabels = STATUS_LABELS;
    statusBadges = STATUS_BADGES;

    constructor(private svc: TeachingDocumentsService, private toastr: ToastrService) {}

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
            next: (rows) => { this.rows = rows; this.loading = false; },
            error: (err) => { this.loading = false; this.toastr.error(err.error?.message || 'Error loading subject allocations.'); }
        });
    }

    get filtered(): TeachingAssignment[] {
        let q = this.search.trim().toLowerCase();
        if (!q) return this.rows;
        return this.rows.filter((r) => [r.staffName, r.subjectName, r.className, r.departmentName]
            .some((v) => (v || '').toLowerCase().includes(q)));
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

    open(a: TeachingAssignment) {
        if (!this.context || !this.session) return;
        this.opened.emit({assignment: a, session: this.session, context: this.context});
    }
}
