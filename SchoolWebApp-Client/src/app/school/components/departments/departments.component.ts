import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {DepartmentsAddFormComponent} from './departments-add-form/departments-add-form.component';
import {forkJoin, Subscription} from 'rxjs';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {Department} from '@/school/models/department';
import {StaffDetails} from '@/staff/models/staff-details';
import {ToastrService} from 'ngx-toastr';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {DepartmentsService} from '@/school/services/departments.service';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-departments',
    templateUrl: './departments.component.html',
    styleUrl: './departments.component.scss'
})
export class DepartmentsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(DepartmentsAddFormComponent)
    departmentForm: DepartmentsAddFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    tableModel: string = 'department';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/school/departments'], title: 'School: Departments'}
    ];
    dashboardTitle = 'School: Departments';
    tableTitle: string = ' Departments list';
    tableHeaders: string[] = [
        'Code',
        'Name',
        'Description',
        'Head of department',
        'Action'
    ];

    department: Department;
    departments: Department[] = [];
    staffDetails: StaffDetails[] = [];

    constructor(
        private toastr: ToastrService,
        private tableSettingsSvc: TableSettingsService,
        private departmentsSvc: DepartmentsService,
        private staffDetailsSvc: StaffDetailsService
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
        let departmentsReq = this.departmentsSvc.get('/departments');
        let staffDetailsReq = this.staffDetailsSvc.get('/staffDetails');

        forkJoin([departmentsReq, staffDetailsReq]).subscribe(
            ([departments, staffDetails]) => {
                this.collectionSize = departments.length;
                this.departments = departments.sort(
                    (a, b) => parseInt(a.id) - parseInt(b.id)
                );
                this.staffDetails = staffDetails;
                this.isAuthLoading = false;
                this.departmentForm.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    editItem(id: number) {
        this.departmentsSvc.getById(id, '/departments').subscribe(
            (res) => {
                let gradeId = res.id;
                this.department = new Department(res);
                this.department.id = gradeId;
                this.departmentForm.setFormControls(this.department);
                this.departmentForm.editMode = true;
                this.departmentForm.department = this.department;
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
                this.departmentsSvc.delete('/departments', id).subscribe(
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
        this.departmentForm.editMode = false;
        this.departmentForm.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addDepartment = (department: Department) => {
        Swal.fire({
            title: `${this.departmentForm.editMode ? 'Update' : 'Add'} department?`,
            text: `Confirm if you want to ${
                this.departmentForm.editMode ? 'update' : 'add'
            } department.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.departmentForm.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new Department(department);
                if (this.departmentForm.editMode) app.id = department.id;
                let reqToProcess = this.departmentForm.editMode
                    ? this.departmentsSvc.update('/departments', app)
                    : this.departmentsSvc.create('/departments', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.departmentForm.editMode = false;
                        this.departmentForm.refreshItems();
                        this.toastr.success('Department saved successfully');
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
