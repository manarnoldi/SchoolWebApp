import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {EventsAddFormComponent} from './events-add-form/events-add-form.component';
import {forkJoin, Subscription} from 'rxjs';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {Session} from '@/class/models/session';
import {SchoolEvent} from '@/school/models/schoolEvent';
import {ToastrService} from 'ngx-toastr';
import {EventsService} from '@/school/services/events.service';
import {SessionsService} from '@/class/services/sessions.service';
import Swal from 'sweetalert2';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {ActivatedRoute} from '@angular/router';
import {CurriculumYearFilterFormComponent} from '@/shared/components/curriculum-year-filter-form/curriculum-year-filter-form.component';
import {CurriculumYearPerson} from '@/shared/models/curriculum-year-person';

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
    @ViewChild(CurriculumYearFilterFormComponent)
    cyfFormComponent: CurriculumYearFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    querySource!: string;

    
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/school/events'], title: 'School: Events'}
    ];

    dashboardTitle = 'School: Events';

    event: SchoolEvent;
    events: SchoolEvent[] = [];
    
    sessions: Session[] = [];
    academicYears: AcademicYear[] = [];

    constructor(
        private toastr: ToastrService,
        private eventsSvc: EventsService,
        private sessionsSvc: SessionsService,
        private academicYearsSvc: AcademicYearsService,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems() {
        let sessionsReq = this.sessionsSvc.get('/sessions');
        let academicYearsReq = this.academicYearsSvc.get('/academicYears');
        this.route.queryParams.subscribe((params) => {
            this.querySource = params['source'];
            forkJoin([sessionsReq, academicYearsReq]).subscribe(
                ([sessions, academicYears]) => {
                    this.sessions = sessions;
                    this.academicYears = academicYears.sort((a, b) =>
                        b.name.localeCompare(a.name)
                    );
                    const topYear = academicYears.find(y=>y.status == true);
                    let cysPass = new CurriculumYearPerson();
                    cysPass.academicYearId = parseInt(topYear.id);

                    this.cyfFormComponent.setFormControls(cysPass);
                    this.cyfFormComponent.onSubmit();
                    this.eventForm.editMode = false;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    }

    academicYearChanged = (id: number) => {
        this.events = [];
    };

    searchClicked = (cys: CurriculumYearPerson) => {
        let searchStr = `/events/byAcademicYearId?academicYearId=${cys.academicYearId ?? ''}`;
        this.eventsSvc.get(searchStr).subscribe({
            next: (events) => {
                this.events = events.sort(
                    (a, b) => +new Date(b.startDate) - +new Date(a.startDate)
                );
                this.isAuthLoading = false;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

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
