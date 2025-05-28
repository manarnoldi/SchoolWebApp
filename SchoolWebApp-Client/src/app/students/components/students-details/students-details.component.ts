import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {CurriculumYearFilterFormComponent} from '@/shared/components/curriculum-year-filter-form/curriculum-year-filter-form.component';
import {CurriculumYearPerson} from '@/shared/models/curriculum-year-person';
import {StudentDetailsService} from '@/students/services/student-details.service';
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
    @ViewChild(CurriculumYearFilterFormComponent)
    cyfFormComponent: CurriculumYearFilterFormComponent;

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
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    statusChanged = (status: Status) => {
        this.students = [];
    };

    searchClicked = (cfy: CurriculumYearPerson) => {
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

            let cysPass = new CurriculumYearPerson();
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
                    this.cyfFormComponent.setFormControls(cysPass);
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
