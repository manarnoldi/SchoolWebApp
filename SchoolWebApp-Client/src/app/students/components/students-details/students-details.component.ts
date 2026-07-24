import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {downloadCsv, exportStamp, CsvColumn} from '@/core/utils/csv-export';
import { SchoolSoftFilterFormComponent } from '@/shared/components/school-soft-filter-form/school-soft-filter-form.component';
import {SchoolSoftFilter} from '@/shared/models/school-soft-filter';
import {StudentDetailsService} from '@/students/services/student-details.service';
import {AuthService} from '@/core/services/auth.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-students-details',
    templateUrl: './students-details.component.html',
    styleUrl: './students-details.component.scss'
})
export class StudentsDetailsComponent implements OnInit {
    @ViewChild(SchoolSoftFilterFormComponent)
    ssFFormComponent: SchoolSoftFilterFormComponent;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/students/details'], title: 'Student: Students list'}
    ];

    dashboardTitle = 'Student: Students list';

    querySource = '';
    status: Status = Status.Active;
    students;
    itemDeleted: boolean = false;
    sourceLink: string = 'details';
    showTable = false;

    constructor(
        private studentsSvc: StudentDetailsService,
        private toarst: ToastrService,
        private router: Router,
        private route: ActivatedRoute,
        private authSvc: AuthService
    ) {}

    // Excel/CSV export is an administrator-only action.
    get canExport(): boolean {
        return this.authSvc.isAdmin;
    }

    ngOnInit(): void {
        this.refreshItems();
    }

    statusChanged = (status: Status) => {
        this.students = [];
    };

    searchClicked = (cfy: SchoolSoftFilter) => {
        this.showTable = false;
        this.studentsSvc.getBySearchDetails(cfy.status).subscribe({
            next: (students) => {
                this.students = students;
                this.status = cfy.status;
                this.showTable = true;
            },
            error: (err) => {
                this.toarst.error(err.error);
            }
        });
    };

    // Export the currently-loaded list (all rows for the selected status, not
    // just the visible page) to a CSV that opens in Excel. Columns mirror the
    // on-screen table.
    exportExcel = () => {
        if (!this.students || this.students.length === 0) {
            this.toarst.info('No students to export.');
            return;
        }
        let columns: CsvColumn<any>[] = [
            {header: 'Ref#', value: (s) => s.id},
            {header: 'Adm#', value: (s) => s.upi},
            {header: 'Full name', value: (s) => s.fullName},
            {header: 'Phone', value: (s) => s.phoneNumber},
            {header: 'Admission date', value: (s) => this.fmtDate(s.admissionDate)},
            {header: 'Learning mode', value: (s) => s.learningMode?.name},
            {header: 'Nationality', value: (s) => s.nationality?.name},
            {header: 'Gender', value: (s) => s.gender?.name},
            {header: 'Status', value: (s) => Status[s.status]}
        ];
        let statusLabel = (Status[this.status] || 'all').toLowerCase();
        downloadCsv(`students-${statusLabel}-${exportStamp()}`, columns, this.students);
        this.toarst.success(`Exported ${this.students.length} student(s).`);
    };

    private fmtDate(d: any): string {
        if (!d) return '';
        let dt = new Date(d);
        if (isNaN(dt.getTime())) return '';
        let p = (n: number) => String(n).padStart(2, '0');
        return `${p(dt.getDate())}/${p(dt.getMonth() + 1)}/${dt.getFullYear()}`;
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
                this.studentsSvc.delete('/students', id).subscribe(
                    (res) => {
                        this.itemDeleted = true;
                        this.refreshItems();
                    },
                    (err) => {
                        this.toarst.error(err);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    refreshItems = () => {
        this.route.queryParams.subscribe((params) => {
            let searchUrl: string = '/students';
            this.sourceLink = this.router.url.split('/').pop();
            this.querySource = params['source'];
            if (this.sourceLink.includes('?')) {
                searchUrl = searchUrl + '?' + this.sourceLink.split('?')[1];
                this.sourceLink = this.sourceLink.split('?')[0];
            }

            let cysPass = new SchoolSoftFilter();
            cysPass.academicYearId = null;
            cysPass.curriculumId = null;

            cysPass.status = params['status']
                ? parseInt(params['status'])
                : Status.Active;
            this.status = cysPass.status;
            this.studentsSvc.getBySearchDetails(cysPass.status).subscribe({
                next: (students) => {
                    this.students = students.sort((a, b) =>
                        a.upi.localeCompare(b.upi)
                    );
                    this.ssFFormComponent.setFormControls(cysPass);
                    this.showTable = true;
                    if (this.itemDeleted) {
                        this.toarst.success('Record deleted successfully!');
                        this.itemDeleted = false;
                        let currentUrl = this.router.url;
                        this.showTable = true;
                        this.router
                            .navigateByUrl('/', {skipLocationChange: true})
                            .then(() => this.router.navigate([currentUrl]));
                    }
                },
                error: (err) => {
                    this.toarst.error(err.error);
                }
            });
        });
    };
}
