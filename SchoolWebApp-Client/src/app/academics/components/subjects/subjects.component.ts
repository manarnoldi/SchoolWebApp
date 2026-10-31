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
import {SubjectsService} from '@/academics/services/subjects.service';
import {SubjectGroupsService} from '@/academics/services/subject-groups.service';
import {DepartmentsService} from '@/school/services/departments.service';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {ActivatedRoute} from '@angular/router';
import Swal from 'sweetalert2';
import {HttpClient} from '@angular/common/http';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';

// One class a subject is allocated to in a year.
interface SubjectClassRow {
    schoolClassId: number;
    className: string;
    learningLevel: string;
    teachers: {name: string; staffNo?: string}[];
    learners: number;
    taking: number;
}

@Component({
    selector: 'app-subjects',
    templateUrl: './subjects.component.html',
    styleUrl: './subjects.component.scss'
})
export class SubjectsComponent implements OnInit {
    querySource: string = '';
    @ViewChild('closebutton') closeButton;
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @ViewChild(SubjectsAddFormComponent)
    subjectsAddForm: SubjectsAddFormComponent;
    tblShowViewButton: true;
    isAuthLoading: boolean;

    page = 1;
    pageSize = 15;

    tableModel: string = 'subject';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/academics/subjects'], title: 'Subjects'}
    ];
    dashboardTitle = 'Academics: Subjects';

    subject: Subject;
    subjects: Subject[] = [];

    subjectGroups: SubjectGroup[] = [];
    departments: Department[] = [];
    staffs: StaffDetails[] = [];

    // The Classes pop-up: the subject, the year (allocations are yearly) and its classes.
    classesFor: Subject | null = null;
    years: AcademicYear[] = [];
    classesYearId: number | null = null;
    classRows: SubjectClassRow[] = [];
    classesLoading = false;

    constructor(
        private toastr: ToastrService,
        private subjectsSvc: SubjectsService,
        private subjectGroupsSvc: SubjectGroupsService,
        private departmentsSvc: DepartmentsService,
        private staffsSvc: StaffDetailsService,
        private route: ActivatedRoute,
        private http: HttpClient,
        private academicYearsSvc: AcademicYearsService
    ) {}

    openClasses = (s: Subject) => {
        this.classesFor = s;
        this.classRows = [];
        if (this.years.length) { this.loadClasses(); return; }
        this.academicYearsSvc.get('/academicYears').subscribe({
            next: (years) => {
                this.years = [...(years || [])].sort((a, b) => +new Date(b.startDate!) - +new Date(a.startDate!));
                // The year running today, else the latest.
                let now = new Date();
                let current = this.years.find((y) => new Date(y.startDate!) <= now && now <= new Date(y.endDate!)) || this.years[0];
                this.classesYearId = current ? +current.id : null;
                this.loadClasses();
            },
            error: () => this.toastr.error('The academic years could not be loaded.')
        });
    };

    loadClasses = () => {
        if (!this.classesFor || !this.classesYearId) return;
        this.classesLoading = true;
        this.classRows = [];
        this.http.get<SubjectClassRow[]>(`/staffSubjects/bySubject/${this.classesFor.id}`, {params: {academicYearId: this.classesYearId}})
            .subscribe({
                next: (rows) => { this.classRows = rows || []; this.classesLoading = false; },
                error: () => { this.classesLoading = false; this.toastr.error('The classes could not be loaded.'); }
            });
    };

    closeClasses = () => { this.classesFor = null; };

    teacherNames = (r: SubjectClassRow): string => r.teachers.map((t) => t.name).join(', ');
    totalLearners = (): number => this.classRows.reduce((s, r) => s + r.learners, 0);
    totalTaking = (): number => this.classRows.reduce((s, r) => s + r.taking, 0);
    yearName = (): string => this.years.find((y) => +y.id === +this.classesYearId!)?.name || '';

    ngOnInit(): void {
        this.querySource = this.route.snapshot.queryParamMap.get('source') || '';
        this.refreshItems();
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
                this.subjects = subjects.sort((a, b) => a.rank - b.rank);
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
    
    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
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
