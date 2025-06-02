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
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {SessionFormComponent} from './session-form/session-form.component';
import {CurriculumYearPerson} from '@/shared/models/curriculum-year-person';
import {CurriculumYearFilterFormComponent} from '@/shared/components/curriculum-year-filter-form/curriculum-year-filter-form.component';

@Component({
    selector: 'app-sessions',
    templateUrl: './sessions.component.html',
    styleUrl: './sessions.component.scss'
})
export class SessionsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(SessionFormComponent) sessionForm: SessionFormComponent;
    @ViewChild(CurriculumYearFilterFormComponent)
    cyfFormComponent: CurriculumYearFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

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
        private sessionSvc: SessionsService,
        private acadYearSvc: AcademicYearsService,
        private curriculumSvc: CurriculumService,
        private sessionTypeSvc: SessionTypesService
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    refreshItems() {
        let acadYearsReq = this.acadYearSvc.get('/academicYears');
        let curriculaReq = this.curriculumSvc.get('/curricula');
        let sessionTypeReq = this.sessionTypeSvc.get('/sessionTypes');

        forkJoin([acadYearsReq, curriculaReq, sessionTypeReq]).subscribe(
            ([academicYears, curricular, sessionTypes]) => {
                this.academicYears = academicYears.sort(
                    (a, b) => b.rank - a.rank
                );
                this.curricula = curricular.sort((a, b) => a.rank - b.rank);
                this.sessionTypes = sessionTypes;
                const topCurriculum = this.curricula[0];
                const topYear = this.academicYears[0];
                let cysPass = new CurriculumYearPerson();
                cysPass.academicYearId = parseInt(topYear.id);
                cysPass.curriculumId = parseInt(topCurriculum.id);
                this.cyfFormComponent.setFormControls(cysPass);

                this.searchClicked(cysPass);
                this.isAuthLoading = false;
                this.sessionForm.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    academicYearChanged = (acadId: number) => {
        this.sessions = [];
    };

    curriculumChanged = (currId: number) => {
        this.sessions = [];
    };

    searchClicked = (cys: CurriculumYearPerson) => {
        this.sessionSvc
            .getByCurriculumYear(cys.curriculumId, cys.academicYearId)
            .subscribe({
                next: (sessions) => {
                    this.sessions = sessions.sort((a, b) => a.rank - b.rank);
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

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
