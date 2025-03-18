import {Curriculum} from '@/academics/models/curriculum';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {Session} from '@/class/models/session';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SessionsService} from '@/class/services/sessions.service';
import {SessionType} from '@/settings/models/session-type';
import {SessionTypesService} from '@/settings/services/session-types.service';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {Subscription, forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {SessionFormComponent} from './session-form/session-form.component';

@Component({
    selector: 'app-sessions',
    templateUrl: './sessions.component.html',
    styleUrl: './sessions.component.scss'
})
export class SessionsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(SessionFormComponent) sessionForm: SessionFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    tableModel: string = 'educationLevelType';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/school/sessions'], title: 'School: Sessions'}
    ];
    dashboardTitle = 'School: Sessions';
    tableTitle: string = ' Sessions list';
    tableHeaders: string[] = [
        'Academic Year',
        'Name',
        'Abbreviation',
        'Rank',
        'Curriculum',
        'Session Type',
        'Start Date',
        'End Date',
        'Status',
        'Action'
    ];

    session: Session;
    sessions: Session[] = [];
    academicYears: AcademicYear[] = [];
    curricula: Curriculum[] = [];
    sessionTypes: SessionType[] = [];

    constructor(
        private toastr: ToastrService,
        private tableSettingsSvc: TableSettingsService,
        private sessionSvc: SessionsService,
        private acadYearSvc: AcademicYearsService,
        private curriculumSvc: CurriculumService,
        private sessionTypeSvc: SessionTypesService
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
        let sessionsReq = this.sessionSvc.get('/sessions');
        let acadYearsReq = this.acadYearSvc.get('/academicYears');
        let curriculaReq = this.curriculumSvc.get('/curricula');
        let sessionTypeReq = this.sessionTypeSvc.get('/sessionTypes');

        forkJoin([
            sessionsReq,
            acadYearsReq,
            curriculaReq,
            sessionTypeReq
        ]).subscribe(
            ([sessions, academicYears, curricular, sessionTypes]) => {
                this.collectionSize = sessions.length;
                this.sessions = sessions.sort((a, b) =>
                    b.academicYear.name.localeCompare(a.academicYear.name)
                );
                this.academicYears = academicYears;
                this.curricula = curricular;
                this.sessionTypes = sessionTypes;
                this.isAuthLoading = false;
                this.sessionForm.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    editItem(id: number) {
        this.sessionSvc.getById(id, '/sessions').subscribe(
            (res) => {
                let sessionId = res.id;
                this.session = new Session(res);
                this.session.id = sessionId;
                this.sessionForm.setFormControls(this.session);
                this.sessionForm.editMode = true;
                this.sessionForm.session = this.session;
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
                this.sessionSvc.delete('/sessions', id).subscribe(
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
        this.sessionForm.editMode = false;
        this.sessionForm.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addSession = (session: Session) => {
        Swal.fire({
            title: `${this.sessionForm.editMode ? 'Update' : 'Add'} session?`,
            text: `Confirm if you want to ${
                this.sessionForm.editMode ? 'update' : 'add'
            } session.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.sessionForm.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new Session(session);
                if (this.sessionForm.editMode) app.id = session.id;
                let reqToProcess = this.sessionForm.editMode
                    ? this.sessionSvc.update('/sessions', app)
                    : this.sessionSvc.create('/sessions', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.sessionForm.editMode = false;
                        this.sessionForm.refreshItems();
                        this.toastr.success('Session saved successfully');
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
