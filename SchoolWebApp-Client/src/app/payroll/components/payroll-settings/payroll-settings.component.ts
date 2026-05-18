import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {PayrollSetting} from '@/payroll/models/payroll-models';
import {PayrollSettingService} from '@/payroll/services/payroll-services';

@Component({
    selector: 'app-payroll-settings',
    templateUrl: './payroll-settings.component.html'
})
export class PayrollSettingsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/payroll/payroll-settings'], title: 'Payroll Settings'}
    ];
    dashboardTitle = 'Payroll: Settings';

    items: PayrollSetting[] = [];
    item: PayrollSetting = new PayrollSetting({isActive: true});
    editMode: boolean = false;
    showForm: boolean = false;

    filterCategory: string | null = null;

    page: number = 1;
    pageSize: number = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(private toastr: ToastrService, private svc: PayrollSettingService) {}

    ngOnInit(): void {
        this.load();
    }

    load() {
        this.svc.get('/payrollSettings').subscribe({
            next: (r) => this.items = (r || []).sort((a, b) => (a.category || '').localeCompare(b.category || '')),
            error: (err) => this.toastr.error(err.error)
        });
    }

    get categories(): string[] {
        let cats = [...new Set((this.items || []).map((i) => i.category).filter((c) => !!c))];
        return cats.sort();
    }

    filtered(): PayrollSetting[] {
        if (!this.filterCategory) return this.items;
        return this.items.filter((i) => i.category === this.filterCategory);
    }

    clearFilters() { this.filterCategory = null; this.page = 1; }

    addNew() {
        let today = new Date().toISOString().substring(0, 10);
        this.item = new PayrollSetting({isActive: true, effectiveDate: today});
        this.editMode = false;
        this.showForm = true;
    }

    edit(x: PayrollSetting) {
        this.item = new PayrollSetting(x);
        this.item.effectiveDate = x.effectiveDate ? x.effectiveDate.substring(0, 10) : '';
        this.editMode = true;
        this.showForm = true;
    }

    cancel() { this.showForm = false; }

    save() {
        if (!this.item.key) { this.toastr.warning('Key is required.'); return; }
        if (!this.item.name) { this.toastr.warning('Name is required.'); return; }
        let req = this.editMode ? this.svc.update('/payrollSettings', this.item) : this.svc.create('/payrollSettings', this.item);
        req.subscribe({
            next: () => { this.toastr.success('Saved.'); this.showForm = false; this.load(); },
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    }

    delete(x: PayrollSetting) {
        Swal.fire({title: 'Delete?', icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (r.value) {
                this.svc.delete('/payrollSettings', parseInt(x.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    }
}
