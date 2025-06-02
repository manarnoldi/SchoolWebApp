import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {SubjectGroupsAddFormComponent} from './subject-groups-add-form/subject-groups-add-form.component';
import {forkJoin} from 'rxjs';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {SubjectGroup} from '@/academics/models/subject-group';
import {Curriculum} from '@/academics/models/curriculum';
import {ToastrService} from 'ngx-toastr';
import {SubjectGroupsService} from '@/academics/services/subject-groups.service';
import {CurriculumService} from '@/academics/services/curriculum.service';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-subject-groups',
    templateUrl: './subject-groups.component.html',
    styleUrl: './subject-groups.component.scss'
})
export class SubjectGroupsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(SubjectGroupsAddFormComponent)
    subjectGroupForm: SubjectGroupsAddFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;

    tableModel: string = 'subjectGroup';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/academics/subjectGroups'], title: 'Academics: Subject groups'}
    ];
    dashboardTitle = 'Academics: Subject groups';
    tableTitle: string = ' Subject groups list';
    tableHeaders: string[] = [
        'Name',
        'Abbreviation',
        'Curriculum',
        'Description',
        'Action'
    ];

    subjectGroup: SubjectGroup;
    subjectGroups: SubjectGroup[] = [];
    curricula: Curriculum[] = [];

    constructor(
        private toastr: ToastrService,
        private subjectGroupsSvc: SubjectGroupsService,
        private curriculumSvc: CurriculumService
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
        let subjectGroupsReq = this.subjectGroupsSvc.get('/subjectGroups');
        let curriculaReq = this.curriculumSvc.get('/curricula');

        forkJoin([subjectGroupsReq, curriculaReq]).subscribe(
            ([subjectGroups, curricular]) => {
                this.subjectGroups = subjectGroups.sort(
                    (a, b) => parseInt(a.id) - parseInt(b.id)
                );
                this.curricula = curricular;
                this.isAuthLoading = false;
                this.subjectGroupForm.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    editItem(id: number) {
        this.subjectGroupsSvc.getById(id, '/subjectGroups').subscribe(
            (res) => {
                let subjectGroupId = res.id;
                this.subjectGroup = new SubjectGroup(res);
                this.subjectGroup.id = subjectGroupId;
                this.subjectGroupForm.setFormControls(this.subjectGroup);
                this.subjectGroupForm.editMode = true;
                this.subjectGroupForm.subjectGroup = this.subjectGroup;
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
                this.subjectGroupsSvc.delete('/subjectGroups', id).subscribe(
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
        this.subjectGroupForm.editMode = false;
        this.subjectGroupForm.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addSubjectGroup = (subjectGroup: SubjectGroup) => {
        Swal.fire({
            title: `${this.subjectGroupForm.editMode ? 'Update' : 'Add'} subject group?`,
            text: `Confirm if you want to ${
                this.subjectGroupForm.editMode ? 'update' : 'add'
            } subject group.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.subjectGroupForm.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new SubjectGroup(subjectGroup);
                if (this.subjectGroupForm.editMode) app.id = subjectGroup.id;
                let reqToProcess = this.subjectGroupForm.editMode
                    ? this.subjectGroupsSvc.update('/subjectGroups', app)
                    : this.subjectGroupsSvc.create('/subjectGroups', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.subjectGroupForm.editMode = false;
                        this.subjectGroupForm.refreshItems();
                        this.toastr.success('Subject group saved successfully');
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
