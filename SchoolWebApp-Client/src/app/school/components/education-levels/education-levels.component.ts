import {Component, OnInit, ViewChild} from '@angular/core';
import {EducationLevelFormComponent} from './education-level-form/education-level-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Subscription, forkJoin} from 'rxjs';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {EducationLevel} from '@/school/models/educationLevel';
import {EducationLevelType} from '@/school/models/education-level-types';
import {Curriculum} from '@/academics/models/curriculum';
import {ToastrService} from 'ngx-toastr';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {EducationLevelTypesService} from '@/school/services/education-level-types.service';
import Swal from 'sweetalert2';
import {CurriculumYearStaff} from '@/shared/models/curriculum-year-staff';
import {CurriculumYearFilterFormComponent} from '@/shared/components/curriculum-year-filter-form/curriculum-year-filter-form.component';

@Component({
    selector: 'app-education-levels',
    templateUrl: './education-levels.component.html',
    styleUrl: './education-levels.component.scss'
})
export class EducationLevelsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(EducationLevelFormComponent)
    educationLevelForm: EducationLevelFormComponent;
    @ViewChild(CurriculumYearFilterFormComponent)
    cyfFormComponent: CurriculumYearFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    tableModel: string = 'educationLevel';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/school/educationLevels'], title: 'School: Education Levels'}
    ];
    dashboardTitle = 'School: Education Levels';
    tableTitle: string = ' Education levels list';
    tableHeaders: string[] = [
        'Name',
        'Rank',
        'Abbreviation',
        'NumOfYears',
        'Education level Type',
        'Curriculum',
        'Description',
        'Action'
    ];

    educationLevel: EducationLevel;
    educationLevels: EducationLevel[] = [];
    educationLevelTypes: EducationLevelType[] = [];
    curricula: Curriculum[] = [];

    constructor(
        private toastr: ToastrService,
        private tableSettingsSvc: TableSettingsService,
        private educationLevelSvc: EducationLevelService,
        private curriculumSvc: CurriculumService,
        private educationLevelTypeSvc: EducationLevelTypesService
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
        let curriculaReq = this.curriculumSvc.get('/curricula');
        let educationLevelTypeReq = this.educationLevelTypeSvc.get(
            '/educationLevelTypes'
        );

        forkJoin([curriculaReq, educationLevelTypeReq]).subscribe(
            ([curricular, educationLevelTypes]) => {
                this.educationLevelTypes = educationLevelTypes;
                this.curricula = curricular.sort(
                    (a, b) => a.rank - b.rank
                );
                const topCurriculum = this.curricula[0];

                let cysPass = new CurriculumYearStaff();
                cysPass.curriculumId = parseInt(topCurriculum.id);

                this.cyfFormComponent.setFormControls(cysPass);
                this.cyfFormComponent.onSubmit();

                this.isAuthLoading = false;
                this.educationLevelForm.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    curriculumChanged = (id: number) => {
        this.educationLevels = [];
    };

    searchClicked = (cys: CurriculumYearStaff) => {
        let searchStr = `/educationLevels/byCurriculumId?curriculumId=${cys.curriculumId ?? ''}`;
        this.educationLevelSvc.get(searchStr).subscribe({
            next: (educationLevels) => {
                this.educationLevels = educationLevels.sort(
                    (a, b) => a.rank - b.rank
                );
                this.collectionSize = educationLevels.length;
                if (this.collectionSize <= 0) {
                    this.toastr.info(
                        'No record found for the selected curriculum!'
                    );
                }
                this.isAuthLoading = false;
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    editItem(id: number) {
        this.educationLevelSvc.getById(id, '/educationLevels').subscribe(
            (res) => {
                let educationLevelId = res.id;
                this.educationLevel = new EducationLevel(res);
                this.educationLevel.id = educationLevelId;
                this.educationLevelForm.setFormControls(this.educationLevel);
                this.educationLevelForm.editMode = true;
                this.educationLevelForm.educationLevel = this.educationLevel;
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
                this.educationLevelSvc.delete('/educationLevels', id).subscribe(
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
        this.educationLevelForm.editMode = false;
        this.educationLevelForm.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addEducationLevel = (educationLevel: EducationLevel) => {
        Swal.fire({
            title: `${this.educationLevelForm.editMode ? 'Update' : 'Add'} education Level?`,
            text: `Confirm if you want to ${
                this.educationLevelForm.editMode ? 'update' : 'add'
            } education Level.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.educationLevelForm.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new EducationLevel(educationLevel);
                if (this.educationLevelForm.editMode)
                    app.id = educationLevel.id;
                let reqToProcess = this.educationLevelForm.editMode
                    ? this.educationLevelSvc.update('/educationLevels', app)
                    : this.educationLevelSvc.create('/educationLevels', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.educationLevelForm.editMode = false;
                        this.educationLevelForm.refreshItems();
                        this.toastr.success(
                            'Education level saved successfully'
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
