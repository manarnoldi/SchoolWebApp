import {TableButtonComponent} from '@/shared/directives/table-button/table-button.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {SubjectsAddFormComponent} from './subjects-add-form/subjects-add-form.component';
import {forkJoin, Subscription} from 'rxjs';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {Subject} from '@/academics/models/subject';
import {SubjectGroup} from '@/academics/models/subject-group';
import {Department} from '@/school/models/department';
import {StaffDetails} from '@/staff/models/staff-details';
import {ToastrService} from 'ngx-toastr';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {SubjectsService} from '@/academics/services/subjects.service';
import {SubjectGroupsService} from '@/academics/services/subject-groups.service';
import {DepartmentsService} from '@/school/services/departments.service';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-subjects',
    templateUrl: './subjects.component.html',
    styleUrl: './subjects.component.scss'
})
export class SubjectsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(SubjectsAddFormComponent)
    subjectsAddForm: SubjectsAddFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    tableModel: string = 'subject';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/academics/subjects'], title: 'Academics: Subjects'}
    ];
    dashboardTitle = 'Academics: Subjects';
    tableTitle: string = ' Subjects list';
    tableHeaders: string[] = [
        'Name',
        'Code',
        'Abbr',
        'Lessons #',
        'Optional',
        'Subject group',
        'Department',
        'H.O.S',
        'Description',
        'Action'
    ];

    subject: Subject;
    subjects: Subject[] = [];

    subjectGroups: SubjectGroup[] = [];
    departments: Department[] = [];
    staffs: StaffDetails[] = [];

    constructor(
        private toastr: ToastrService,
        private tableSettingsSvc: TableSettingsService,
        private subjectsSvc: SubjectsService,
        private subjectGroupsSvc: SubjectGroupsService,
        private departmentsSvc: DepartmentsService,
        private staffsSvc: StaffDetailsService
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
        let subjectsReq = this.subjectsSvc.get('/subjects');
        let subjectGroupsReq = this.subjectGroupsSvc.get('/subjectGroups');
        let departmentsReq = this.departmentsSvc.get('/departments');
        let staffsReq = this.staffsSvc.get('/staffDetails');

        forkJoin([
            subjectsReq,
            subjectGroupsReq,
            departmentsReq,
            staffsReq
        ]).subscribe(
            ([subjects, subjectGroups, departments, staffs]) => {
                this.collectionSize = subjectGroups.length;
                this.subjects = subjects.sort(
                    (a, b) => parseInt(a.id) - parseInt(b.id)
                );
                this.subjectGroups = subjectGroups;
                this.departments = departments;
                this.staffs = staffs;
                this.isAuthLoading = false;
                this.subjectsAddForm.editMode = false;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    editItem(id: number) {
        this.subjectsSvc.getById(id, '/subjects').subscribe(
            (res) => {
                let subjectId = res.id;
                this.subject = new SubjectGroup(res);
                this.subject.id = subjectId;
                this.subjectsAddForm.setFormControls(this.subject);
                this.subjectsAddForm.editMode = true;
                this.subjectsAddForm.subject = this.subject;
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
                this.subjectGroupsSvc.delete('/subjects', id).subscribe(
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
        this.subjectsAddForm.editMode = false;
        this.subjectsAddForm.refreshItems();
    };

    errorEvent = (errorName: string) => {
        this.toastr.error(errorName);
    };

    addSubject = (subject: Subject) => {
        Swal.fire({
            title: `${this.subjectsAddForm.editMode ? 'Update' : 'Add'} subject?`,
            text: `Confirm if you want to ${
                this.subjectsAddForm.editMode ? 'update' : 'add'
            } subject.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.subjectsAddForm.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let app = new Subject(subject);
                if (this.subjectsAddForm.editMode) app.id = subject.id;
                let reqToProcess = this.subjectsAddForm.editMode
                    ? this.subjectsSvc.update('/subjects', app)
                    : this.subjectsSvc.create('/subjects', app);

                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.subjectsAddForm.editMode = false;
                        this.subjectsAddForm.refreshItems();
                        this.toastr.success('Subject saved successfully');
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
