import {Component, Input, OnInit} from '@angular/core';
import {TeachingDocumentsService} from '@/teaching/services/teaching-services';
import {DocStatus, STATUS_BADGES, STATUS_LABELS, TeachingSession, TrackerRow} from '@/teaching/models/teaching-models';

/**
 * Staff page, Teaching tab: for a term, each subject and class the teacher
 * teaches - the scheme's status, lesson plan weeks approved, lessons taught.
 * The tracker decides what the viewer may see (own, department, whole school).
 */
@Component({
    selector: 'app-staff-teaching',
    template: `
        <div class="d-flex align-items-center gap-2 mb-2">
            <label class="small mb-0">Term</label>
            <select class="form-select form-select-sm" style="width: auto;" [(ngModel)]="sessionId" (ngModelChange)="load()">
                <option *ngFor="let s of sessions" [ngValue]="s.id">{{ s.name }} {{ s.academicYearName }}</option>
            </select>
            <!-- The tracker opens on this term and this teacher -->
            <a class="btn btn-sm btn-outline-primary ms-auto" [routerLink]="['/teaching/tracker']" [queryParams]="{term: sessionId, teacher: staffId}">
                <i class="fas fa-th"></i> Open the tracker</a>
        </div>
        <div *ngIf="loading" class="text-muted p-2"><i class="fas fa-spinner fa-spin"></i> Loading...</div>
        <div *ngIf="!loading && rows.length === 0" class="text-muted p-2">
            No subjects for this term, or their teaching documents are outside what you can see.</div>
        <table class="table table-sm table-striped mb-0 align-middle small" *ngIf="!loading && rows.length">
            <thead class="table-light"><tr><th>Subject</th><th>Class</th><th>Scheme of work</th>
                <th class="text-center">Lesson plan weeks approved</th><th class="text-center">Lessons taught</th></tr></thead>
            <tbody>
                <tr *ngFor="let r of rows">
                    <td>{{ r.subjectName }}</td>
                    <td>{{ r.className }}</td>
                    <td><span class="badge" [ngClass]="badges[r.scheme.status]">{{ labels[r.scheme.status] }}</span>
                        <span class="badge bg-danger ms-1" *ngIf="r.scheme.overdue">Overdue</span></td>
                    <td class="text-center">{{ approvedWeeks(r) }} / {{ applicableWeeks(r) }}</td>
                    <td class="text-center">{{ r.taughtLessons }} / {{ r.plannedLessons }}</td>
                </tr>
            </tbody>
        </table>`
})
export class StaffTeachingComponent implements OnInit {
    @Input() staffId!: number;
    sessions: TeachingSession[] = [];
    sessionId: number | null = null;
    rows: TrackerRow[] = [];
    loading = false;
    labels = STATUS_LABELS;
    badges = STATUS_BADGES;

    constructor(private svc: TeachingDocumentsService) {}

    ngOnInit(): void {
        this.svc.context().subscribe((c) => {
            this.sessions = c.sessions;
            this.sessionId = c.currentSessionId ?? c.sessions[0]?.id ?? null;
            this.load();
        });
    }

    load() {
        if (!this.sessionId) return;
        this.loading = true;
        this.svc.tracker({sessionId: this.sessionId, staffDetailsId: +this.staffId} as any).subscribe({
            next: (t) => {
                this.loading = false;
                this.rows = t.rows.filter((r) => r.staffDetailsId === +this.staffId);
            },
            error: () => {
                this.loading = false;
                this.rows = [];
            }
        });
    }

    applicableWeeks(r: TrackerRow): number {
        return r.weeks.filter((w) => w.status !== ('NotApplicable' as DocStatus)).length;
    }

    approvedWeeks(r: TrackerRow): number {
        return r.weeks.filter((w) => w.status === ('Approved' as DocStatus)).length;
    }
}
