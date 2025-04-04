import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {LearningLevelsFormComponent} from './learning-levels-form/learning-levels-form.component';
import {Subscription, forkJoin} from 'rxjs';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {EducationLevel} from '@/school/models/educationLevel';
import {ToastrService} from 'ngx-toastr';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import Swal from 'sweetalert2';
import { LearningLevel } from '@/class/models/learning-level';
import { LearningLevelsService } from '@/class/services/learning-levels.service';

@Component({
    selector: 'app-learning-levels',
    templateUrl: './learning-levels.component.html',
    styleUrl: './learning-levels.component.scss'
})
export class LearningLevelsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(LearningLevelsFormComponent)
    learningLevelForm: LearningLevelsFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    tableModel: string = 'learningLevel';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Class'},
        {link: ['/class/learningLevels'], title: 'Class: Learning Levels'}
    ];
    dashboardTitle = 'Class: Learning Levels';
    tableTitle: string = ' Learning levels list';
    tableHeaders: string[] = [
        'Name',
        'Education level',
        'Rank',
        'Description',
        'Action'
    ];

    learningLevel: LearningLevel;
    learningLevels: LearningLevel[] = [];
    educationLevels: EducationLevel[] = [];

    constructor(
        private toastr: ToastrService,
        private tableSettingsSvc: TableSettingsService,
        private learningLevelSvc: LearningLevelsService,
        private educationLevelSvc: EducationLevelService
    ) {}

    ngOnInit(): void {
        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );
        this.refreshItems();
    }

    refreshItems() {
        let learningLevelsReq = this.learningLevelSvc.get('/learningLevels');
        let educationLevelReq = this.educationLevelSvc.get('/educationLevels');

        forkJoin([learningLevelsReq, educationLevelReq]).subscribe(
            ([learningLevels, educationLevels]) => {
                this.collectionSize = learningLevels.length;
                this.learningLevels = learningLevels.sort(
                    (a, b) => parseInt(a.id) - parseInt(b.id)
                );
                this.educationLevels = educationLevels;
                this.isAuthLoading = false;
                this.learningLevelForm.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    editItem(id: number) {
        this.learningLevelSvc.getById(id, '/learningLevels').subscribe(
            (res) => {
                let learningLevelId = res.id;
                this.learningLevel = new LearningLevel(res);
                this.learningLevel.id = learningLevelId;
                this.learningLevelForm.setFormControls(this.learningLevel);
                this.learningLevelForm.editMode = true;
                this.learningLevelForm.learningLevel = this.learningLevel;
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
                this.learningLevelSvc.delete('/learningLevels', id).subscribe(
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
        this.learningLevelForm.editMode = false;
        this.learningLevelForm.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addLearningLevel = (learningLevel: LearningLevel) => {
        Swal.fire({
            title: `${this.learningLevelForm.editMode ? 'Update' : 'Add'} learning Level?`,
            text: `Confirm if you want to ${
                this.learningLevelForm.editMode ? 'update' : 'add'
            } learning Level.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.learningLevelForm.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new LearningLevel(learningLevel);
                if (this.learningLevelForm.editMode) app.id = learningLevel.id;
                let reqToProcess = this.learningLevelForm.editMode
                    ? this.learningLevelSvc.update('/learningLevels', app)
                    : this.learningLevelSvc.create('/learningLevels', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.learningLevelForm.editMode = false;
                        this.learningLevelForm.refreshItems();
                        this.toastr.success(
                            'Learning level saved successfully'
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
