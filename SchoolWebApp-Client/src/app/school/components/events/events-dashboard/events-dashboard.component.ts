import {SchoolEvent} from '@/school/models/schoolEvent';
import {EventsService} from '@/school/services/events.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
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
    eventsLimit: number = 4;

    constructor(
        private eventsSvc: EventsService,
        private toastr: ToastrService,
        private globalSettingSvc: GlobalSettingService
    ) {}

    ngOnInit(): void {
        this.globalSettingSvc.getByKey('General', 'UpcomingEventsCount').subscribe({
            next: (setting) => {
                if (setting?.settingValue) {
                    this.eventsLimit = parseInt(setting.settingValue) || 4;
                }
                this.refreshItems();
            },
            error: () => this.refreshItems()
        });
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
                    .slice(0, this.eventsLimit);
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    }
}
