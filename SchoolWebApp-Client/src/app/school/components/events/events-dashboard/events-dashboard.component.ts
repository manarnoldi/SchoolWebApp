import {SchoolEvent} from '@/school/models/schoolEvent';
import {EventsService} from '@/school/services/events.service';
import {Component, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';

@Component({
    selector: 'app-events-dashboard',
    templateUrl: './events-dashboard.component.html',
    styleUrl: './events-dashboard.component.scss'
})
export class EventsDashboardComponent implements OnInit {
    events: SchoolEvent[] = [];
    now = new Date();
    upcomingEvents: SchoolEvent[] = [];

    constructor(
        private eventsSvc: EventsService,
        private toastr: ToastrService
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems() {
        this.eventsSvc.get('/events').subscribe({
            next: (events) => {
                this.events = events.sort(
                    (a, b) => +new Date(b.startDate) - +new Date(a.startDate)
                );
                this.upcomingEvents = this.events
                    .filter((event) => new Date(event.startDate) >= this.now)
                    .sort(
                        (a, b) =>
                            new Date(a.startDate).getTime() -
                            new Date(b.startDate).getTime()
                    )
                    .slice(0, 4);
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    }
}
