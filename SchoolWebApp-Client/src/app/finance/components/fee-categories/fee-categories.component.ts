import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {FeeCategory} from '@/finance/models/finance-models';
import {Account, AccountType} from '@/finance/models/account';
import {AccountService, FeeCategoryService} from '@/finance/services/finance-services';

@Component({
    selector: 'app-fee-categories',
    templateUrl: './fee-categories.component.html'
})
export class FeeCategoriesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/fee-categories'], title: 'Fee Categories'}
    ];
    dashboardTitle = 'Finance: Fee Categories';

    items: FeeCategory[] = [];
    incomeAccounts: Account[] = [];
    item: FeeCategory = new FeeCategory({isActive: true, rank: 0});
    editMode: boolean = false;
    showForm: boolean = false;
    filterActive: any = null;

    page: number = 1;
    pageSize: number = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(private toastr: ToastrService, private svc: FeeCategoryService, private accountSvc: AccountService) {}

    ngOnInit(): void {
        this.load();
        this.accountSvc.get('/accounts').subscribe({
            next: (accounts) => {
                this.incomeAccounts = accounts.filter((a) => a.accountType === AccountType.Income);
            },
            error: () => {}
        });
    }

    load() {
        this.svc.get('/feeCategories').subscribe({
            next: (r) => this.items = r.sort((a, b) => (a.rank || 0) - (b.rank || 0)),
            error: (err) => this.toastr.error(err.error)
        });
    }

    filtered = (): FeeCategory[] => {
        return this.items.filter((c) => {
            if (this.filterActive != null && c.isActive !== this.filterActive) return false;
            return true;
        });
    };

    clearFilters = () => { this.filterActive = null; this.page = 1; };

    addNew() { this.item = new FeeCategory({isActive: true, rank: 0}); this.editMode = false; this.showForm = true; }
    edit(x: FeeCategory) { this.item = new FeeCategory(x); this.editMode = true; this.showForm = true; }
    cancel() { this.showForm = false; }
    save() {
        if (!this.item.name) { this.toastr.warning('Name is required.'); return; }
        let req = this.editMode ? this.svc.update('/feeCategories', this.item) : this.svc.create('/feeCategories', this.item);
        req.subscribe({
            next: () => { this.toastr.success('Saved.'); this.showForm = false; this.load(); },
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    }
    delete(x: FeeCategory) {
        Swal.fire({title: 'Delete?', icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (r.value) {
                this.svc.delete('/feeCategories', parseInt(x.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    }
}
