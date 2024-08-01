import {ClassLeadership} from '@/class/models/class-leadership';
import {ClassLeadershipRole} from '@/class/models/class-leadership-role';
import {SchoolClass} from '@/class/models/school-class';
import {ClassLeadershipRolesService} from '@/class/services/class-leadership-roles.service';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {PersonType} from '@/core/enums/personTypes';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {Person} from '@/school/models/person';
import { TableButtonComponent } from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import { ClassLeadershipAddFormComponent } from './class-leadership-add-form/class-leadership-add-form.component';
import { ClassLeadershipsService } from '@/class/services/class-leaderships.service';

@Component({
    selector: 'app-class-details',
    templateUrl: './class-details.component.html',
    styleUrl: './class-details.component.scss'
})
export class ClassDetailsComponent implements OnInit {
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(ClassLeadershipAddFormComponent)
    classLeadershipForm: ClassLeadershipAddFormComponent;
    
    schoolClassId: number = 0;
    schoolClass: SchoolClass;
    classLeadership: ClassLeadership;
    classLeaderships: ClassLeadership[] = [];
    classLeadershipRoles: ClassLeadershipRole[] = [];
    persons: Person[] = [];

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/class/classDetails'], title: 'Class: Class details'}
    ];

    dashboardTitle = 'Class details';
    backLinkUrl: string = '/class/classes';
    personType = PersonType;
    personTypes;

    constructor(
        private route: ActivatedRoute,
        private schoolClassesSvc: SchoolClassesService,
        private classLeadershipRolesSvc: ClassLeadershipRolesService,
        private classLeadershipsSvc: ClassLeadershipsService,
        private toastr: ToastrService
    ) {
        this.personTypes = Object.keys(this.personType).filter((k) =>
            isNaN(Number(k))
        );
    }

    editItem(id: number) {
        this.classLeadershipsSvc.getById(id, '/schoolClassLeaders').subscribe(
            (res) => {
                let classLeadershipId = res.id;
                this.classLeadership = new ClassLeadership(res);
                this.classLeadership.id = classLeadershipId;
                this.classLeadershipForm.setFormControls(this.classLeadership);
                this.classLeadershipForm.editMode = true;
                this.classLeadershipForm.classLeadership = this.classLeadership;
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
                this.classLeadershipsSvc.delete('/schoolClassLeaders', id).subscribe(
                    (res) => {
                        this.loadSelectedSchoolClass();
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


    addClassLeader = (classLeadership: ClassLeadership) => {
        Swal.fire({
            title: `${this.classLeadershipForm.editMode ? 'Update' : 'Add'} class leadership?`,
            text: `Confirm if you want to ${
                this.classLeadershipForm.editMode ? 'update' : 'add'
            } class leadership.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.classLeadershipForm.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new ClassLeadership(classLeadership);
                if (this.classLeadershipForm.editMode) app.id = classLeadership.id;
                let reqToProcess = this.classLeadershipForm.editMode
                    ? this.classLeadershipsSvc.update('/schoolClassLeaders', app)
                    : this.classLeadershipsSvc.create('/schoolClassLeaders', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.classLeadershipForm.editMode = false;
                        this.classLeadershipForm.refreshItems();
                        this.toastr.success(
                            'Class leadership details saved successfully'
                        );
                        this.loadSelectedSchoolClass();
                        this.classLeadershipForm.closeButton.nativeElement.click();
                    },
                    (err) => {
                        this.toastr.error(err.error?.message);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };

    ngOnInit(): void {
        this.loadSelectedSchoolClass();
    }

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    loadSelectedSchoolClass = () => {
        this.route.queryParams.subscribe((params) => {
            this.schoolClassId = params['id'];

            let schoolClassByIdReq = this.schoolClassesSvc.getById(
                this.schoolClassId,
                '/schoolClasses'
            );
            let schoolClassLeadersReq = this.schoolClassesSvc.get(
                '/schoolClassLeaders/bySchoolClassId/' +
                    this.schoolClassId.toString()
            );
            let classLeadershipRolesReq = this.classLeadershipRolesSvc.get(
                '/classLeadershipRoles'
            );

            forkJoin([
                schoolClassByIdReq,
                schoolClassLeadersReq,
                classLeadershipRolesReq
            ]).subscribe(
                ([schoolClass, classLeaderships, classLeadershipRoles]) => {
                    this.schoolClass = schoolClass;
                    this.classLeaderships = classLeaderships;
                    this.classLeadershipRoles = classLeadershipRoles;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };
}
