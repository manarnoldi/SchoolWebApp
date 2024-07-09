import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {EventsAddFormComponent} from './events-add-form/events-add-form.component';
import {forkJoin, Subscription} from 'rxjs';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {Session} from '@/school/models/session';
import {SchoolEvent} from '@/school/models/schoolEvent';
import {ToastrService} from 'ngx-toastr';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {EventsService} from '@/school/services/events.service';
import {SessionsService} from '@/school/services/sessions.service';
import Swal from 'sweetalert2';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';

@Component({
    selector: 'app-events',
    templateUrl: './events.component.html',
    styleUrl: './events.component.scss'
})
export class EventsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(EventsAddFormComponent)
    eventForm: EventsAddFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    tableModel: string = 'event';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/school/events'], title: 'School: Events'}
    ];
    dashboardTitle = 'School: Events';
    tableTitle: string = ' Events list';
    tableHeaders: string[] = [
        'Academic year',
        'Session',
        'Event name',
        'Event location',
        'Start date',
        'End date',
        'Description',
        'Action'
    ];

    event: SchoolEvent;
    events: SchoolEvent[] = [];
    sessions: Session[] = [];
    academicYears: AcademicYear[] = [];

    constructor(
        private toastr: ToastrService,
        private tableSettingsSvc: TableSettingsService,
        private eventsSvc: EventsService,
        private sessionsSvc: SessionsService,
        private academicYearsSvc: AcademicYearsService
    ) {}

    ngOnInit(): void {
        this.refreshItems();
        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );
    }

    refreshItems() {
        let eventsReq = this.eventsSvc.get('/events');
        let sessionsReq = this.sessionsSvc.get('/sessions');
        let academicYearsReq = this.academicYearsSvc.get('/academicYears');

        forkJoin([eventsReq, sessionsReq, academicYearsReq]).subscribe(
            ([events, sessions, academicYears]) => {
                this.collectionSize = events.length;
                this.events = events.sort(
                    (a, b) => +new Date(b.startDate) - +new Date(a.startDate)
                );
                this.sessions = sessions;
                this.academicYears = academicYears.sort((a, b) =>
                    b.name.localeCompare(a.name)
                );
                this.isAuthLoading = false;
                this.eventForm.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    editItem(id: number) {
        this.eventsSvc.getById(id, '/events').subscribe(
            (res) => {
                let eventId = res.id;
                this.event = new SchoolEvent(res);
                this.event.id = eventId;
                this.eventForm.setFormControls(this.event);
                this.eventForm.editMode = true;
                this.eventForm.event = this.event;
                this.tableButton.onClick();
            },
            (err) => {
                this.toastr.error(err);
            }
        );
    }

    deleteItem(id: number) {
        Swal.fire({
            title: `Delete record?`,
            text: `Confirm if you want to delete record.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `Delete`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.eventsSvc.delete('/events', id).subscribe(
                    (res) => {
                        this.refreshItems();
                        this.toastr.success('Record deleted successfully!');
                    },
                    (err) => {
                        this.toastr.error(err);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    resetForm = () => {
        this.eventForm.editMode = false;
        this.eventForm.resetFormControls();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addEvent = (event: SchoolEvent) => {
        Swal.fire({
            title: `${this.eventForm.editMode ? 'Update' : 'Add'} event?`,
            text: `Confirm if you want to ${
                this.eventForm.editMode ? 'update' : 'add'
            } event.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.eventForm.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new SchoolEvent(event);
                if (this.eventForm.editMode) app.id = event.id;
                let reqToProcess = this.eventForm.editMode
                    ? this.eventsSvc.update('/events', app)
                    : this.eventsSvc.create('/events', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.eventForm.editMode = false;
                        this.eventForm.resetFormControls();
                        this.toastr.success('Event saved successfully');
                        this.refreshItems();
                        this.closeButton.nativeElement.click();
                    },
                    (err) => {
                        this.toastr.error(err.error);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };
}
