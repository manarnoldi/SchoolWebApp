import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {TaxBand} from '@/payroll/models/payroll-models';
import {TaxBandService} from '@/payroll/services/payroll-services';

@Component({
    selector: 'app-payroll-tax-bands',
    templateUrl: './tax-bands.component.html'
})
export class PayrollTaxBandsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/payroll/tax-bands'], title: 'Tax Bands'}
    ];
    dashboardTitle = 'Payroll: Tax Bands';

    items: TaxBand[] = [];
    item: TaxBand = new TaxBand({isActive: true, rate: 0, lowerLimit: 0, upperLimit: 0});
    editMode: boolean = false;
    showForm: boolean = false;

    page: number = 1;
    pageSize: number = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(private toastr: ToastrService, private svc: TaxBandService) {}

    ngOnInit(): void {
        this.load();
    }

    load() {
        this.svc.get('/taxBands').subscribe({
            next: (r) => this.items = (r || []).sort((a, b) => (+a.lowerLimit || 0) - (+b.lowerLimit || 0)),
            error: (err) => this.toastr.error(err.error)
        });
    }

    addNew() {
        let today = new Date().toISOString().substring(0, 10);
        this.item = new TaxBand({isActive: true, rate: 0, lowerLimit: 0, upperLimit: 0, effectiveDate: today});
        this.editMode = false;
        this.showForm = true;
    }

    edit(x: TaxBand) {
        this.item = new TaxBand(x);
        this.item.effectiveDate = x.effectiveDate ? x.effectiveDate.substring(0, 10) : '';
        this.editMode = true;
        this.showForm = true;
    }

    cancel() { this.showForm = false; }

    save() {
        if (!this.item.description) { this.toastr.warning('Description is required.'); return; }
        if (this.item.lowerLimit == null || this.item.upperLimit == null || this.item.rate == null) {
            this.toastr.warning('Lower limit, upper limit and rate are required.'); return;
        }
        let req = this.editMode ? this.svc.update('/taxBands', this.item) : this.svc.create('/taxBands', this.item);
        req.subscribe({
            next: () => { this.toastr.success('Saved.'); this.showForm = false; this.load(); },
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    }

    delete(x: TaxBand) {
        Swal.fire({title: 'Delete?', icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (r.value) {
                this.svc.delete('/taxBands', parseInt(x.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    }
}
