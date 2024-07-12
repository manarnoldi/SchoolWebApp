import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {SchoolClassAddFormComponent} from './school-class-add-form/school-class-add-form.component';
import {forkJoin, Subscription} from 'rxjs';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolStream} from '@/class/models/school-stream';
import {AcademicYear} from '@/school/models/academic-year';
import {ToastrService} from 'ngx-toastr';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {SchoolStreamsService} from '@/class/services/school-streams.service';
import Swal from 'sweetalert2';
import { LearningLevel } from '@/class/models/learning-level';
import { LearningLevelsService } from '@/class/services/learning-levels.service';

@Component({
    selector: 'app-school-class',
    templateUrl: './school-class.component.html',
    styleUrl: './school-class.component.scss'
})
export class SchoolClassComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(SchoolClassAddFormComponent)
    schoolClassForm: SchoolClassAddFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    tableModel: string = 'schoolClass';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/school/classes'], title: 'School: Classes'}
    ];
    dashboardTitle = 'School: Classes';
    tableTitle: string = ' Classes list';
    tableHeaders: string[] = [
        'Academic year',
        'Class',
        'Stream',        
        'Class name',
        'Class Leaders',
        'Description',
        'Action',
        'Manage Leadership'
    ];

    schoolClass: SchoolClass;
    schoolClasses: SchoolClass[] = [];
    learningLevels: LearningLevel[] = [];
    schoolStreams: SchoolStream[] = [];
    academicYears: AcademicYear[] = [];

    constructor(
        private toastr: ToastrService,
        private tableSettingsSvc: TableSettingsService,
        private schoolClassesSvc: SchoolClassesService,
        private learningLevelsSvc: LearningLevelsService,
        private schoolStreamsSvc: SchoolStreamsService,
        private academicYearsSvc: AcademicYearsService,
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
        let schoolClassesReq = this.schoolClassesSvc.get('/schoolClasses');
        let learningLevelsReq = this.learningLevelsSvc.get('/learningLevels');
        let schoolStreamsReq = this.schoolStreamsSvc.get('/schoolStreams');
        let academicYearsReq = this.academicYearsSvc.get('/academicYears');

        forkJoin([schoolClassesReq, learningLevelsReq, schoolStreamsReq,academicYearsReq]).subscribe(
            ([schoolClasses, learningLevels, schoolStreams,academicYears]) => {
                this.collectionSize = schoolClasses.length;
                this.schoolClasses = schoolClasses.sort(
                    (a, b) => parseInt(b.academicYearId) - parseInt(a.academicYearId)
                );
                this.learningLevels = learningLevels;
                this.schoolStreams = schoolStreams;
                this.academicYears = academicYears.sort((a, b) =>
                    b.name.localeCompare(a.name)
                );
                this.isAuthLoading = false;
                this.schoolClassForm.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    editItem(id: number) {
        this.schoolClassesSvc.getById(id, '/schoolClasses').subscribe(
            (res) => {
                let schoolClassId = res.id;
                this.schoolClass = new SchoolClass(res);
                this.schoolClass.id = schoolClassId;
                this.schoolClassForm.setFormControls(this.schoolClass);
                this.schoolClassForm.editMode = true;
                this.schoolClassForm.schoolClass = this.schoolClass;
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
                this.schoolClassesSvc.delete('/schoolClasses', id).subscribe(
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
        this.schoolClassForm.editMode = false;
        this.schoolClassForm.resetFormControls();
    };

    errorSchoolClass = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addSchoolClass = (schoolClass: SchoolClass) => {
        Swal.fire({
            title: `${this.schoolClassForm.editMode ? 'Update' : 'Add'} school class?`,
            text: `Confirm if you want to ${
                this.schoolClassForm.editMode ? 'update' : 'add'
            } school class.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.schoolClassForm.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new SchoolClass(schoolClass);
                if (this.schoolClassForm.editMode) app.id = schoolClass.id;
                let reqToProcess = this.schoolClassForm.editMode
                    ? this.schoolClassesSvc.update('/schoolClasses', app)
                    : this.schoolClassesSvc.create('/schoolClasses', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.schoolClassForm.editMode = false;
                        this.schoolClassForm.resetFormControls();
                        this.toastr.success('School class saved successfully');
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
