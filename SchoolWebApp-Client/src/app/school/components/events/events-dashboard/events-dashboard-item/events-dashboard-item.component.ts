import {SchoolEvent} from '@/school/models/schoolEvent';
import {DatePipe} from '@angular/common';
import {Component, Input} from '@angular/core';

@Component({
    selector: 'app-events-dashboard-item',
    templateUrl: './events-dashboard-item.component.html',
    styleUrl: './events-dashboard-item.component.scss'
})
export class EventsDashboardItemComponent {
    @Input() event: SchoolEvent;

    showFullText = false;
    toggleText() {
        this.showFullText = !this.showFullText;
    }
    constructor(private datePipe: DatePipe) {}
}
