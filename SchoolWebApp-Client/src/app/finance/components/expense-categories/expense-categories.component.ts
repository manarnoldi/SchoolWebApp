import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {ExpenseCategory} from '@/finance/models/finance-models';
import {Account, AccountType} from '@/finance/models/account';
import {AccountService, ExpenseCategoryService} from '@/finance/services/finance-services';

@Component({
    selector: 'app-expense-categories',
    templateUrl: './expense-categories.component.html'
})
export class ExpenseCategoriesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/expense-categories'], title: 'Expense Categories'}
    ];
    dashboardTitle = 'Finance: Expense Categories';

    items: ExpenseCategory[] = [];
    expenseAccounts: Account[] = [];
    item: ExpenseCategory = new ExpenseCategory({isActive: true, rank: 0});
    editMode: boolean = false;
    showForm: boolean = false;
    filterActive: any = null;

    page: number = 1;
    pageSize: number = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(private toastr: ToastrService, private svc: ExpenseCategoryService, private accountSvc: AccountService) {}

    ngOnInit(): void {
        this.load();
        this.accountSvc.get('/accounts').subscribe({
            next: (accounts) => { this.expenseAccounts = accounts.filter((a) => a.accountType === AccountType.Expense); },
            error: () => {}
        });
    }

    load() {
        this.svc.get('/expenseCategories').subscribe({
            next: (r) => this.items = r.sort((a, b) => (a.rank || 0) - (b.rank || 0)),
            error: (err) => this.toastr.error(err.error)
        });
    }

    filtered = (): ExpenseCategory[] => {
        return this.items.filter((c) => {
            if (this.filterActive != null && c.isActive !== this.filterActive) return false;
            return true;
        });
    };

    clearFilters = () => { this.filterActive = null; this.page = 1; };

    addNew() { this.item = new ExpenseCategory({isActive: true, rank: 0}); this.editMode = false; this.showForm = true; }
    edit(x: ExpenseCategory) { this.item = new ExpenseCategory(x); this.editMode = true; this.showForm = true; }
    cancel() { this.showForm = false; }
    save() {
        if (!this.item.name) { this.toastr.warning('Name is required.'); return; }
        let req = this.editMode ? this.svc.update('/expenseCategories', this.item) : this.svc.create('/expenseCategories', this.item);
        req.subscribe({
            next: () => { this.toastr.success('Saved.'); this.showForm = false; this.load(); },
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    }
    delete(x: ExpenseCategory) {
        Swal.fire({title: 'Delete?', icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (r.value) {
                this.svc.delete('/expenseCategories', parseInt(x.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    }
}
