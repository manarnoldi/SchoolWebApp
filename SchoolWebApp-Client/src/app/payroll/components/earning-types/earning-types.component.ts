import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {EarningType} from '@/payroll/models/payroll-models';
import {EarningTypeService} from '@/payroll/services/payroll-services';

@Component({
    selector: 'app-payroll-earning-types',
    templateUrl: './earning-types.component.html'
})
export class PayrollEarningTypesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/payroll/earning-types'], title: 'Earning Types'}
    ];
    dashboardTitle = 'Payroll: Earning Types';

    items: EarningType[] = [];
    item: EarningType = new EarningType({isActive: true, isTaxable: false});
    editMode: boolean = false;
    showForm: boolean = false;

    page: number = 1;
    pageSize: number = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(private toastr: ToastrService, private svc: EarningTypeService) {}

    ngOnInit(): void {
        this.load();
    }

    load() {
        this.svc.get('/earningTypes').subscribe({
            next: (r) => this.items = r || [],
            error: (err) => this.toastr.error(err.error)
        });
    }

    addNew() { this.item = new EarningType({isActive: true, isTaxable: false}); this.editMode = false; this.showForm = true; }
    edit(x: EarningType) { this.item = new EarningType(x); this.editMode = true; this.showForm = true; }
    cancel() { this.showForm = false; }

    save() {
        if (!this.item.name) { this.toastr.warning('Name is required.'); return; }
        if (!this.item.code) { this.toastr.warning('Code is required.'); return; }
        let req = this.editMode ? this.svc.update('/earningTypes', this.item) : this.svc.create('/earningTypes', this.item);
        req.subscribe({
            next: () => { this.toastr.success('Saved.'); this.showForm = false; this.load(); },
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    }

    delete(x: EarningType) {
        Swal.fire({title: 'Delete?', icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (r.value) {
                this.svc.delete('/earningTypes', parseInt(x.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    }
}
