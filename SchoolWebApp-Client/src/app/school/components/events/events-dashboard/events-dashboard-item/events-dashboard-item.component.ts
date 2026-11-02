import {SchoolEvent} from '@/school/models/schoolEvent';
import {Component, Input, OnChanges} from '@angular/core';
import {DateTime} from 'luxon';
import {eventDay} from '../events-dashboard.component';

/**
 * One upcoming event: its date in a circle, then the name, how soon it is, the
 * description (longer ones fold), the place and the days it runs.
 */
@Component({
    selector: 'app-events-dashboard-item',
    templateUrl: './events-dashboard-item.component.html',
    styleUrl: './events-dashboard-item.component.scss'
})
export class EventsDashboardItemComponent implements OnChanges {
    @Input() event: SchoolEvent;

    start: DateTime | null = null;
    end: DateTime | null = null;
    // "Today", "On now", "Tomorrow", "In 5 days"; none further ahead.
    when = '';
    whenClass = '';
    // "Wed 4 Nov 2026", or "Wed 4 - Fri 6 Nov 2026" for an event over several days.
    days = '';

    showFullText = false;
    readonly foldAt = 110;

    toggleText() {
        this.showFullText = !this.showFullText;
    }

    ngOnChanges(): void {
        const s = eventDay(this.event?.startDate);
        const e = eventDay(this.event?.endDate) || s;
        this.start = s ? DateTime.fromISO(s) : null;
        this.end = e ? DateTime.fromISO(e) : this.start;
        if (!this.start) return;

        const today = DateTime.now().startOf('day');
        const away = Math.round(this.start.diff(today, 'days').days);
        if (away <= 0) {
            this.when = away === 0 ? 'Today' : 'On now';
            this.whenClass = 'text-bg-success';
        } else if (away === 1) {
            this.when = 'Tomorrow';
            this.whenClass = 'text-bg-warning';
        } else if (away <= 14) {
            this.when = `In ${away} days`;
            this.whenClass = 'bg-info-subtle text-info-emphasis';
        } else {
            this.when = '';
        }

        const end = this.end!;
        if (end <= this.start) this.days = this.start.toFormat('ccc d LLL yyyy');
        else if (end.hasSame(this.start, 'month')) this.days = `${this.start.toFormat('ccc d')} - ${end.toFormat('ccc d LLL yyyy')}`;
        else this.days = `${this.start.toFormat('ccc d LLL')} - ${end.toFormat('ccc d LLL yyyy')}`;
    }
}
