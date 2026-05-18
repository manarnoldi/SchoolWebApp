import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {Account, AccountType} from '@/finance/models/account';
import {AccountService} from '@/finance/services/finance-services';

@Component({
    selector: 'app-finance-accounts',
    templateUrl: './accounts.component.html',
    styleUrls: ['./accounts.component.scss']
})
export class FinanceAccountsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/accounts'], title: 'Chart of Accounts'}
    ];
    dashboardTitle = 'Finance: Chart of Accounts';

    accounts: Account[] = [];
    account: Account = new Account({accountType: AccountType.Asset, isActive: true});
    editMode: boolean = false;
    showForm: boolean = false;
    filterType: AccountType | null = null;
    filterActive: any = null;

    page: number = 1;
    pageSize: number = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    accountTypes = [
        {value: AccountType.Asset, label: 'Asset'},
        {value: AccountType.Liability, label: 'Liability'},
        {value: AccountType.Equity, label: 'Equity'},
        {value: AccountType.Income, label: 'Income'},
        {value: AccountType.Expense, label: 'Expense'}
    ];

    constructor(
        private toastr: ToastrService,
        private accountSvc: AccountService
    ) {}

    ngOnInit(): void { this.load(); }

    load = () => {
        this.accountSvc.get('/accounts').subscribe({
            next: (accounts) => {
                this.accounts = accounts.sort((a, b) => (a.code || '').localeCompare(b.code || ''));
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    filtered(): Account[] {
        return this.accounts.filter((a) => {
            if (this.filterType != null && a.accountType !== this.filterType) return false;
            if (this.filterActive != null && a.isActive !== this.filterActive) return false;
            return true;
        });
    }

    clearFilters = () => {
        this.filterType = null;
        this.filterActive = null;
        this.page = 1;
    };

    getTypeLabel(t: any): string {
        return this.accountTypes.find((x) => x.value === t)?.label || '';
    }

    getTypeBadgeClass(t: any): string {
        switch (t) {
            case AccountType.Asset: return 'bg-success';
            case AccountType.Liability: return 'bg-warning text-dark';
            case AccountType.Equity: return 'bg-info';
            case AccountType.Income: return 'bg-primary';
            case AccountType.Expense: return 'bg-danger';
            default: return 'bg-secondary';
        }
    }

    addNew() {
        this.account = new Account({accountType: AccountType.Asset, isActive: true});
        this.editMode = false;
        this.showForm = true;
    }

    edit(a: Account) {
        this.account = new Account(a);
        this.editMode = true;
        this.showForm = true;
    }

    cancel() {
        this.showForm = false;
        this.account = new Account({accountType: AccountType.Asset, isActive: true});
    }

    save() {
        if (!this.account.code || !this.account.name) {
            this.toastr.warning('Code and Name are required.');
            return;
        }
        let req = this.editMode
            ? this.accountSvc.update('/accounts', this.account)
            : this.accountSvc.create('/accounts', this.account);
        req.subscribe({
            next: () => {
                this.toastr.success('Saved.');
                this.showForm = false;
                this.load();
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    }

    delete(a: Account) {
        Swal.fire({
            title: 'Delete account?',
            text: `Delete ${a.code} - ${a.name}?`,
            icon: 'warning', showCancelButton: true,
            confirmButtonText: 'Delete', confirmButtonColor: '#d33'
        }).then((r) => {
            if (r.value) {
                this.accountSvc.delete('/accounts', parseInt(a.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    }
}
