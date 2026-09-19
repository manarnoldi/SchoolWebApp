import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {DeductionType} from '@/payroll/models/payroll-models';
import {DeductionTypeService} from '@/payroll/services/payroll-services';
import {AccountService} from '@/finance/services/finance-services';
import {Account} from '@/finance/models/account';

@Component({
    selector: 'app-payroll-deduction-types',
    templateUrl: './deduction-types.component.html'
})
export class PayrollDeductionTypesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/payroll/deduction-types'], title: 'Deduction Types'}
    ];
    dashboardTitle = 'Payroll: Deduction Types';

    items: DeductionType[] = [];
    item: DeductionType = new DeductionType({isActive: true, isStatutory: false, isTaxDeductible: false, taxDeductibleCap: null, isRetirementContribution: false, liabilityAccountId: null, calculationMethod: 0, defaultValue: null, appliesToAll: false, isSystemComputed: false});
    editMode: boolean = false;
    showForm: boolean = false;

    page: number = 1;
    pageSize: number = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    // Liability accounts a deduction can be credited to when payroll posts to the GL.
    liabilityAccounts: Account[] = [];

    constructor(
        private toastr: ToastrService,
        private svc: DeductionTypeService,
        private accountSvc: AccountService
    ) {}

    ngOnInit(): void {
        this.load();
        this.accountSvc.get('/accounts').subscribe({
            // AccountType 2 = Liability. Money deducted from pay is owed onward.
            next: (r) => this.liabilityAccounts = (r || [])
                .filter((a: Account) => +a.accountType! === 2 && a.isActive !== false)
                .sort((a: Account, b: Account) => (a.code || '').localeCompare(b.code || '')),
            // The account list is optional here; the form still works without it.
            error: () => this.liabilityAccounts = []
        });
    }

    accountLabel(id: number | null | undefined): string {
        if (id == null) return '';
        let a = this.liabilityAccounts.find((x) => +x.id === +id);
        return a ? `${a.code} ${a.name}` : '';
    }

    load() {
        this.svc.get('/deductionTypes').subscribe({
            next: (r) => this.items = r || [],
            error: (err) => this.toastr.error(err.error)
        });
    }

    addNew() { this.item = new DeductionType({isActive: true, isStatutory: false, isTaxDeductible: false, taxDeductibleCap: null, isRetirementContribution: false, liabilityAccountId: null, calculationMethod: 0, defaultValue: null, appliesToAll: false, isSystemComputed: false}); this.editMode = false; this.showForm = true; }
    edit(x: DeductionType) { this.item = new DeductionType(x); this.editMode = true; this.showForm = true; }
    cancel() { this.showForm = false; }

    // A retirement contribution is relieved through the NSSF pool, so it must not
    // also be ticked tax deductible - that would take it off taxable income twice.
    onRetirementChange() {
        if (this.item.isRetirementContribution) {
            this.item.isTaxDeductible = false;
            this.item.taxDeductibleCap = null;
        }
    }

    save() {
        if (!this.item.name) { this.toastr.warning('Name is required.'); return; }
        if (!this.item.code) { this.toastr.warning('Code is required.'); return; }
        // An applies-to-all type needs a value, or it would be applied as zero and
        // quietly do nothing.
        if (this.item.appliesToAll && !this.item.isSystemComputed &&
            (this.item.defaultValue == null || +this.item.defaultValue <= 0)) {
            this.toastr.warning('A deduction that applies to all staff needs an amount or percentage greater than zero.');
            return;
        }
        if (!this.item.isTaxDeductible) {
            // Don't keep a cap against a type that is no longer deductible - it
            // would read as live on the list and confuse the next person.
            this.item.taxDeductibleCap = null;
        } else if (this.item.taxDeductibleCap != null && +this.item.taxDeductibleCap < 0) {
            this.toastr.warning('The monthly cap cannot be negative.');
            return;
        }
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
