import {Component, OnInit, ViewChild} from '@angular/core';
import {EducationLevelFormComponent} from './education-level-form/education-level-form.component';
import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {forkJoin} from 'rxjs';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {EducationLevel} from '@/school/models/educationLevel';
import {EducationLevelType} from '@/school/models/education-level-types';
import {Curriculum} from '@/academics/models/curriculum';
import {ToastrService} from 'ngx-toastr';
import {EducationLevelService} from '@/school/services/education-level.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {EducationLevelTypesService} from '@/school/services/education-level-types.service';
import Swal from 'sweetalert2';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import { SchoolSoftFilterFormComponent } from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';

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
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

    tableModel: string = 'educationLevel';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/school/educationLevels'], title: 'School: Education Levels'}
    ];
    dashboardTitle = 'School: Education Levels';
    tableTitle: string = ' Education levels list';
    tableHeaders: string[] = [
        'Ref#',
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
        private educationLevelSvc: EducationLevelService,
        private curriculumSvc: CurriculumService,
        private educationLevelTypeSvc: EducationLevelTypesService
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems() {
        let curriculaReq = this.curriculumSvc.get('/curricula');
        let educationLevelTypeReq = this.educationLevelTypeSvc.get(
            '/educationLevelTypes'
        );

        forkJoin([curriculaReq, educationLevelTypeReq]).subscribe(
            ([curricular, educationLevelTypes]) => {
                this.educationLevelTypes = educationLevelTypes;
                this.curricula = curricular.sort((a, b) => a.rank - b.rank);
                const topCurriculum = this.curricula[0];

                let cysPass = new SchoolSoftFilter();
                cysPass.curriculumId = parseInt(topCurriculum.id);

                this.ssFilterFormComponent.setFormControls(cysPass);
                this.ssFilterFormComponent.onSubmit();

                this.isAuthLoading = false;
                this.educationLevelForm.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    curriculumChanged = (id: number) => {
        this.educationLevels = [];
    };

    searchClicked = (cys: SchoolSoftFilter) => {
        let searchStr = `/educationLevels/byCurriculumId?curriculumId=${cys.curriculumId ?? ''}`;
        this.educationLevelSvc.get(searchStr).subscribe({
            next: (educationLevels) => {
                this.educationLevels = educationLevels.sort(
                    (a, b) => a.rank - b.rank
                );

                if (this.educationLevels.length <= 0) {
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
