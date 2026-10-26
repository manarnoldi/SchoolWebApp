import {Component, EventEmitter, Input, OnChanges, Output} from '@angular/core';
import {TeachingSession} from '@/teaching/models/teaching-models';

/**
 * Year, then term - so the term list stays short however many years there are.
 * Two-way bound to the selected term: [(sessionId)]. Choosing a year selects
 * that year's current term, or its first.
 */
@Component({
    selector: 'app-term-picker',
    template: `
        <div class="row g-2">
            <div class="col-6">
                <label class="form-label mb-0 small">Year</label>
                <select class="form-select form-select-sm" [ngModel]="yearId" (ngModelChange)="pickYear($event)">
                    <option *ngFor="let y of years" [ngValue]="y.id">{{ y.name }}</option>
                </select>
            </div>
            <div class="col-6">
                <label class="form-label mb-0 small">Term</label>
                <select class="form-select form-select-sm" [ngModel]="sessionId" (ngModelChange)="pickTerm($event)">
                    <option *ngFor="let s of terms" [ngValue]="s.id">{{ s.name }}{{ s.isCurrent ? ' (current)' : '' }}</option>
                </select>
            </div>
        </div>`
})
export class TermPickerComponent implements OnChanges {
    @Input() sessions: TeachingSession[] = [];
    @Input() sessionId: number | null = null;
    @Output() sessionIdChange = new EventEmitter<number>();

    years: {id: number; name: string}[] = [];
    yearId: number | null = null;

    ngOnChanges(): void {
        // Latest year first.
        let seen = new Map<number, {id: number; name: string; start: string}>();
        this.sessions.forEach((s) => {
            let y = seen.get(s.academicYearId);
            if (!y || s.startDate < y.start) seen.set(s.academicYearId, {id: s.academicYearId, name: s.academicYearName, start: s.startDate});
        });
        this.years = Array.from(seen.values()).sort((a, b) => b.start.localeCompare(a.start)).map(({id, name}) => ({id, name}));
        this.yearId = this.sessions.find((s) => s.id === this.sessionId)?.academicYearId ?? this.years[0]?.id ?? null;
    }

    // The year's terms in order.
    get terms(): TeachingSession[] {
        return this.sessions.filter((s) => s.academicYearId === this.yearId)
            .sort((a, b) => a.startDate.localeCompare(b.startDate));
    }

    pickYear(yearId: number) {
        this.yearId = yearId;
        let terms = this.terms;
        let term = terms.find((s) => s.isCurrent) ?? terms[0];
        if (term) this.pickTerm(term.id);
    }

    pickTerm(sessionId: number) {
        this.sessionId = sessionId;
        this.sessionIdChange.emit(sessionId);
    }
}
