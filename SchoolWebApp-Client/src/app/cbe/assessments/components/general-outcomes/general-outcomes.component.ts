import {BreadCrumb} from '@/core/models/bread-crumb';
import {SchoolSoftFilterFormComponent} from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {GeneralOutcomeAddFormComponent} from './general-outcome-add-form/general-outcome-add-form.component';
import {EducationLevelType} from '@/school/models/education-level-types';
import {GeneralOutcome} from '../../models/general-outcome';
import {ToastrService} from 'ngx-toastr';
import {GeneralOutcomeService} from '../../services/general-outcome.service';
import {EducationLevelTypesService} from '@/school/services/education-level-types.service';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';

@Component({
    selector: 'app-general-outcomes',
    templateUrl: './general-outcomes.component.html',
    styleUrl: './general-outcomes.component.scss'
})
export class GeneralOutcomesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(GeneralOutcomeAddFormComponent)
    generalOutcomeForm: GeneralOutcomeAddFormComponent;
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

    tableModel: string = 'generalOutcome';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/class/generalOutcomes'], title: 'Class: General Outcome'}
    ];
    dashboardTitle = 'Class: General Outcome';
    tableTitle: string = ' General Outcomes list';
    tableHeaders: string[] = [
        'Ref#',
        'Name',
        'Education level type',
        'Rank',
        'Description',
        'Action'
    ];

    generalOutcome: GeneralOutcome;
    generalOutcomes: GeneralOutcome[] = [];
    educationLevelTypes: EducationLevelType[] = [];

    firstLoad: boolean = true;

    constructor(
        private toastr: ToastrService,
        private generalOutcomeSvc: GeneralOutcomeService,
        private educationLevelTypesSvc: EducationLevelTypesService
    ) {}

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems() {
        let educationLevelTypeReq = this.educationLevelTypesSvc.get(
            '/educationLevelTypes'
        );
        forkJoin([educationLevelTypeReq]).subscribe(
            ([educationLevelTypes]) => {
                this.educationLevelTypes = educationLevelTypes.sort(
                    (a, b) => a.rank - b.rank
                );

                const topEducationLevelType = this.educationLevelTypes[0];

                let cysPass = new SchoolSoftFilter();
                cysPass.educationLevelTypeId = parseInt(
                    topEducationLevelType.id
                );

                this.ssFilterFormComponent.setFormControls(cysPass);
                this.ssFilterFormComponent.onSubmit();

                this.isAuthLoading = false;
                this.generalOutcomeForm.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    educationLevelTypeChanged = (id: number) => {
        this.generalOutcomes = [];
    };

    searchClicked = (cys: SchoolSoftFilter) => {
        let searchStr = `/generalOutcomes/byEducationLevelTypeId?educationLevelTypeId=${cys.educationLevelTypeId ?? ''}`;
        this.generalOutcomeSvc.get(searchStr).subscribe({
            next: (generalOutcomes) => {
                this.generalOutcomes = generalOutcomes.sort(
                    (a, b) => a.rank - b.rank
                );
                if (this.generalOutcomes.length <= 0 && !this.firstLoad) {
                    this.toastr.info(
                        'No record found for the selected education level type!'
                    );
                }
                this.firstLoad = false;
                this.isAuthLoading = false;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    editItem(id: number) {
        this.generalOutcomeSvc.getById(id, '/generalOutcomes').subscribe(
            (res) => {
                let generalOutcomeId = res.id;
                this.generalOutcome = new GeneralOutcome(res);
                this.generalOutcome.id = generalOutcomeId;
                this.generalOutcomeForm.setFormControls(this.generalOutcome);
                this.generalOutcomeForm.editMode = true;
                this.generalOutcomeForm.generalOutcome = this.generalOutcome;
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
                this.generalOutcomeSvc.delete('/generalOutcomes', id).subscribe(
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
        this.generalOutcomeForm.editMode = false;
        this.generalOutcomeForm.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addGeneralOutcome = (generalOutcome: GeneralOutcome) => {
        Swal.fire({
            title: `${this.generalOutcomeForm.editMode ? 'Update' : 'Add'} general outcome?`,
            text: `Confirm if you want to ${
                this.generalOutcomeForm.editMode ? 'update' : 'add'
            } general outcome.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.generalOutcomeForm.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new GeneralOutcome(generalOutcome);
                if (this.generalOutcomeForm.editMode)
                    app.id = generalOutcome.id;
                let reqToProcess = this.generalOutcomeForm.editMode
                    ? this.generalOutcomeSvc.update('/generalOutcomes', app)
                    : this.generalOutcomeSvc.create('/generalOutcomes', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.generalOutcomeForm.editMode = false;
                        this.generalOutcomeForm.refreshItems();
                        this.toastr.success(
                            'General outcome saved successfully'
                        );
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
