import {BreadCrumb} from '@/core/models/bread-crumb';
import {ParentsService} from '@/students/services/parents.service';
import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-parents-list',
    templateUrl: './parents-list.component.html',
    styleUrl: './parents-list.component.scss'
})
export class ParentsListComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/students/parents'], title: 'Parent: Parents list'}
    ];

    dashboardTitle = 'Parent: Parents list';

    parents;
    itemDeleted: boolean = false;
    sourceLink: string = 'details';

    constructor(
        private parentsSvc: ParentsService,
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
                this.parentsSvc.delete('/parents', id).subscribe(
                    (res) => {
                        this.itemDeleted = true;
                        this.refreshItems();
                    },
                    (err) => {
                        this.toarst.error(err.message);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    refreshItems = () => {
        this.sourceLink = this.router.url.split('/').pop();
        this.parentsSvc.get('/parents').subscribe(
            (res) => {
                this.parents = res;
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
