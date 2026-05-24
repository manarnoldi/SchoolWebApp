import {SchoolEvent} from '@/school/models/schoolEvent';
import {EventsService} from '@/school/services/events.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {Component, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {AuthService} from '@/core/services/auth.service';
import {MenuPermissionService} from '@/security/services/menu-permission.service';
import {of} from 'rxjs';
import {catchError} from 'rxjs/operators';

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

    // Drives visibility of the "+" add button in the card header. Mirrors the
    // School > Events menu permission so users who can't reach that page
    // don't see a button that would just send them to a 403/blank view.
    canManageEvents: boolean = false;

    constructor(
        private eventsSvc: EventsService,
        private toastr: ToastrService,
        private globalSettingSvc: GlobalSettingService,
        private authService: AuthService,
        private menuPermSvc: MenuPermissionService
    ) {}

    ngOnInit(): void {
        this.resolveCanManageEvents();
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

    /**
     * Admins and SuperAdmins always see the add button. Other roles only see
     * it when their menu permissions include /school/events — same path the
     * sidebar uses to decide whether to show the Events item, so the two
     * stay in sync without separate configuration.
     */
    private resolveCanManageEvents(): void {
        let user = this.authService.getCurrentUser();
        let isAdmin = !!user?.currentUserAdministrator
            || (user?.roles || []).some(
                (r: any) => String(r).toLowerCase() === 'superadministrator'
            );
        if (isAdmin) {
            this.canManageEvents = true;
            return;
        }
        this.menuPermSvc.getMyPermissions()
            .pipe(catchError(() => of({allAccess: false, paths: [] as string[]})))
            .subscribe((res) => {
                this.canManageEvents = !!res?.allAccess
                    || (res?.paths || []).includes('/school/events');
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
