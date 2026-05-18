import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {DeductionType} from '@/payroll/models/payroll-models';
import {DeductionTypeService} from '@/payroll/services/payroll-services';

@Component({
    selector: 'app-payroll-deduction-types',
    templateUrl: './deduction-types.component.html'
})
export class PayrollDeductionTypesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/payroll/deduction-types'], title: 'Deduction Types'}
    ];
    dashboardTitle = 'Payroll: Deduction Types';

    items: DeductionType[] = [];
    item: DeductionType = new DeductionType({isActive: true, isStatutory: false});
    editMode: boolean = false;
    showForm: boolean = false;

    page: number = 1;
    pageSize: number = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(private toastr: ToastrService, private svc: DeductionTypeService) {}

    ngOnInit(): void {
        this.load();
    }

    load() {
        this.svc.get('/deductionTypes').subscribe({
            next: (r) => this.items = r || [],
            error: (err) => this.toastr.error(err.error)
        });
    }

    addNew() { this.item = new DeductionType({isActive: true, isStatutory: false}); this.editMode = false; this.showForm = true; }
    edit(x: DeductionType) { this.item = new DeductionType(x); this.editMode = true; this.showForm = true; }
    cancel() { this.showForm = false; }

    save() {
        if (!this.item.name) { this.toastr.warning('Name is required.'); return; }
        if (!this.item.code) { this.toastr.warning('Code is required.'); return; }
        let req = this.editMode ? this.svc.update('/deductionTypes', this.item) : this.svc.create('/deductionTypes', this.item);
        req.subscribe({
            next: () => { this.toastr.success('Saved.'); this.showForm = false; this.load(); },
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    }

    delete(x: DeductionType) {
        Swal.fire({title: 'Delete?', icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (r.value) {
                this.svc.delete('/deductionTypes', parseInt(x.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    }
}
