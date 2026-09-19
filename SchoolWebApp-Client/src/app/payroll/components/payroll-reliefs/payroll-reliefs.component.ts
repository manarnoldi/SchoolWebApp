import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {PayrollRelief, DeductionType} from '@/payroll/models/payroll-models';
import {PayrollReliefService, DeductionTypeService} from '@/payroll/services/payroll-services';

@Component({
    selector: 'app-payroll-reliefs',
    templateUrl: './payroll-reliefs.component.html'
})
export class PayrollReliefsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/payroll/reliefs'], title: 'Reliefs'}
    ];
    dashboardTitle = 'Payroll: Reliefs';

    items: PayrollRelief[] = [];
    deductionTypes: DeductionType[] = [];
    item: PayrollRelief = this.blank();
    editMode = false;
    showForm = false;

    page = 1;
    pageSize = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(
        private toastr: ToastrService,
        private svc: PayrollReliefService,
        private deductionTypeSvc: DeductionTypeService
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.svc.get('/payrollReliefs'),
            this.deductionTypeSvc.get('/deductionTypes')
        ]).subscribe({
            next: ([reliefs, types]) => {
                this.items = reliefs || [];
                // A relief is worked out from a deduction the employee actually pays,
                // so the statutory ones payroll computes itself are not offered.
                this.deductionTypes = (types || [])
                    .filter((t: DeductionType) => t.isActive && !t.isSystemComputed)
                    .sort((a: DeductionType, b: DeductionType) => (a.name || '').localeCompare(b.name || ''));
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading reliefs.')
        });
    }

    load(): void {
        this.svc.get('/payrollReliefs').subscribe({
            next: (r) => this.items = r || [],
            error: (err) => this.toastr.error(err.error?.message || 'Error loading reliefs.')
        });
    }

    private blank(): PayrollRelief {
        return new PayrollRelief({
            basis: 0, value: 0, monthlyCap: null, deductionTypeId: null,
            appliesToAll: true, isActive: true
        });
    }

    addNew(): void { this.item = this.blank(); this.editMode = false; this.showForm = true; }
    edit(x: PayrollRelief): void { this.item = new PayrollRelief(x); this.editMode = true; this.showForm = true; }
    cancel(): void { this.showForm = false; }

    // A percentage relief can only apply where its deduction exists, so the
    // applies-to-all switch is meaningless for one.
    isPercentage(): boolean { return +this.item.basis! === 1; }

    onBasisChange(): void {
        if (this.isPercentage()) this.item.appliesToAll = false;
    }

    deductionName(r: PayrollRelief): string {
        if (r.deductionTypeName) return r.deductionTypeName;
        let t = this.deductionTypes.find((d) => +d.id === +r.deductionTypeId!);
        return t?.name || '';
    }

    save(): void {
        if (!this.item.name) { this.toastr.warning('Name is required.'); return; }
        if (!this.item.code) { this.toastr.warning('Code is required.'); return; }
        if (!this.item.value || +this.item.value <= 0) {
            this.toastr.warning('The relief value must be greater than zero.');
            return;
        }
        if (this.isPercentage() && !this.item.deductionTypeId) {
            this.toastr.warning('A percentage relief must say which deduction it is worked out from.');
            return;
        }
        let req = this.editMode
            ? this.svc.update('/payrollReliefs', this.item)
            : this.svc.create('/payrollReliefs', this.item);
        req.subscribe({
            next: () => { this.toastr.success('Saved.'); this.showForm = false; this.load(); },
            error: (err) => this.toastr.error(err.error?.message || 'Error saving.')
        });
    }

    delete(x: PayrollRelief): void {
        Swal.fire({
            title: 'Delete relief?',
            text: `Remove ${x.name}? Payroll will stop applying it on the next run.`,
            icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'
        }).then((r) => {
            if (r.value) {
                this.svc.delete('/payrollReliefs', parseInt(x.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error deleting.')
                });
            }
        });
    }
}
