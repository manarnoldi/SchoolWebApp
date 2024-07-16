import {BreadCrumb} from '@/core/models/bread-crumb';
import {StudentDetailsService} from '@/students/services/student-details.service';
import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-students-details',
    templateUrl: './students-details.component.html',
    styleUrl: './students-details.component.scss'
})
export class StudentsDetailsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/students/details'], title: 'Student: Students list'}
    ];

    dashboardTitle = 'Student: Students list';

    students;
    itemDeleted: boolean = false;
    sourceLink: string = 'details';

    constructor(
        private studentsSvc: StudentDetailsService,
        private toarst: ToastrService,
        private router: Router
    ) {}

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
        this.sourceLink = this.router.url.split('/').pop();
        this.studentsSvc.get('/students').subscribe(
            (res) => {
                this.students = res;
                if (this.itemDeleted) {
                    this.toarst.success('Record deleted successfully!');
                    this.itemDeleted = false;
                    let currentUrl = this.router.url;
                    this.router
                        .navigateByUrl('/', {skipLocationChange: true})
                        .then(() => this.router.navigate([currentUrl]));
                }
            },
            (err) => {
                this.toarst.error(err.error);
            }
        );
    };
}
