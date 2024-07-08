import {BreadCrumb} from '@/core/models/bread-crumb';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {DataTableDirective} from 'angular-datatables';
import {StaffDetailsTableComponent} from './staff-details-table/staff-details-table.component';
import {Router} from '@angular/router';

@Component({
    selector: 'app-staff-details',
    templateUrl: './staff-details.component.html',
    styleUrl: './staff-details.component.scss'
})
export class StaffDetailsComponent implements OnInit {
    dashboardTitle = 'Staff details list';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/staff/details'], title: 'School:staff details'}
    ];

    staffs;
    itemDeleted: boolean = false;

    constructor(
        private staffsSvc: StaffDetailsService,
        private toastr: ToastrService,
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

    refreshItems = () => {
        this.staffsSvc.get('/staffDetails').subscribe(
            (res) => {
                this.staffs = res;
                if (this.itemDeleted) {
                    this.toastr.success('Record deleted successfully!');
                    this.itemDeleted = false;
                    let currentUrl = this.router.url;
                    this.router
                        .navigateByUrl('/', {skipLocationChange: true})
                        .then(() => this.router.navigate([currentUrl]));
                }
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    };
}
