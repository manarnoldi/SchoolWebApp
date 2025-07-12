import {BreadCrumb} from '@/core/models/bread-crumb';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {ActivatedRoute, Router} from '@angular/router';
import {StaffCategoriesService} from '@/settings/services/staff-categories.service';
import {EmploymentTypeService} from '@/settings/services/employment-type.service';
import {forkJoin} from 'rxjs';
import {StaffCategory} from '@/settings/models/staff-category';
import {EmploymentType} from '@/settings/models/employment-type';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {Status} from '@/core/enums/status';
import { SchoolSoftFilterFormComponent } from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';

@Component({
    selector: 'app-staff-details',
    templateUrl: './staff-details.component.html',
    styleUrl: './staff-details.component.scss'
})
export class StaffDetailsComponent implements OnInit {
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFilterFormComponent: SchoolSoftFilterFormComponent;

    dashboardTitle = 'Staff details list';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/staff/details'], title: 'School:staff details'}
    ];

    staffs;
    showTable = false;
    itemDeleted: boolean = false;
    sourceLink: string = 'details';
    staffCats: StaffCategory[] = [];
    empTypes: EmploymentType[] = [];

    statuses;
    status = Status;
    querySource = '';

    constructor(
        private staffsSvc: StaffDetailsService,
        private toastr: ToastrService,
        private router: Router,
        private staffCatSvc: StaffCategoriesService,
        private empTypeSvc: EmploymentTypeService,
        private route: ActivatedRoute
    ) {
        this.statuses = Object.keys(this.status).filter((k) =>
            isNaN(Number(k))
        );
    }

    ngOnInit(): void {
        this.refreshItems();
    }

    editItem(id: number) {}

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
                this.staffsSvc.delete('/staffDetails', id).subscribe(
                    (res) => {
                        this.itemDeleted = true;
                        this.refreshItems();
                    },
                    (err) => {
                        this.toastr.error(err);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    searchClicked = (cfy: SchoolSoftFilter) => {
        this.showTable = false;
        this.staffsSvc
            .getBySearchDetails(
                cfy.status,
                cfy.employmentTypeId,
                cfy.staffCategoryId
            )
            .subscribe({
                next: (staffs) => {
                    this.staffs = staffs;
                    this.showTable = true;
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
    };

    employmentTypeChanged = (id: number) => {
        this.staffs = [];
    };

    staffCategoryChanged = (id: number) => {
        this.staffs = [];
    };

    statusChanged = (status: any) => {
        this.staffs = [];
    };

    refreshItems = () => {
        let staffCatsReq = this.staffCatSvc.get('/staffCategories');
        let emloyTypesReq = this.empTypeSvc.get('/employmentTypes');
        this.route.queryParams.subscribe((params) => {
            forkJoin([staffCatsReq, emloyTypesReq]).subscribe({
                next: ([staffCats, empTypes]) => {
                    let cysPass = new SchoolSoftFilter();
                    cysPass.academicYearId = null;
                    cysPass.curriculumId = null;
                    cysPass.status = Status.Active;

                    let linkMain = this.router.url.split('?')[0];
                    this.sourceLink =
                        linkMain.split('/')[linkMain.split('/').length - 1];

                    cysPass.status = params['status']
                        ? parseInt(params['status'])
                        : Status.Active;
                    cysPass.employmentTypeId = params['employmentTypeId']
                        ? parseInt(params['employmentTypeId'])
                        : null;
                    cysPass.staffCategoryId = params['staffCategoryId']
                        ? parseInt(params['staffCategoryId'])
                        : null;
                    this.querySource = params['source'];
                    this.ssFilterFormComponent.setFormControls(cysPass);
                    this.staffsSvc
                        .getBySearchDetails(
                            cysPass.status,
                            cysPass.employmentTypeId,
                            cysPass.staffCategoryId
                        )
                        .subscribe({
                            next: (staffs) => {
                                this.staffCats = staffCats;
                                this.empTypes = empTypes;
                                this.staffs = staffs;
                                this.showTable = true;
                                if (this.itemDeleted) {
                                    this.toastr.success(
                                        'Record deleted successfully!'
                                    );
                                    this.itemDeleted = false;
                                    let currentUrl = this.router.url;
                                    this.router
                                        .navigateByUrl('/', {
                                            skipLocationChange: true
                                        })
                                        .then(() =>
                                            this.router.navigate([currentUrl])
                                        );
                                }
                            },
                            error: (err) => {
                                this.toastr.error(err.error);
                            }
                        });
                },
                error: (err) => {
                    this.toastr.error(err.error);
                }
            });
        });
    };
}
