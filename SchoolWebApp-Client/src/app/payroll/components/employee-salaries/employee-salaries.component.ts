import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {EmployeeSalary, EmployeeSalaryItem, EarningType, DeductionType} from '@/payroll/models/payroll-models';
import {EmployeeSalaryService, EarningTypeService, DeductionTypeService} from '@/payroll/services/payroll-services';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {Status} from '@/core/enums/status';

@Component({
    selector: 'app-payroll-employee-salaries',
    templateUrl: './employee-salaries.component.html'
})
export class PayrollEmployeeSalariesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/payroll/employee-salaries'], title: 'Employee Salaries'}
    ];
    dashboardTitle = 'Payroll: Employee Salaries';

    items: EmployeeSalary[] = [];
    staffList: any[] = [];
    earningTypes: EarningType[] = [];
    deductionTypes: DeductionType[] = [];
    item: EmployeeSalary = new EmployeeSalary({isActive: true, items: []});
    editMode: boolean = false;
    showForm: boolean = false;

    filterStaffId: any = null;
    filterActive: any = null;

    page: number = 1;
    pageSize: number = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(
        private toastr: ToastrService,
        private svc: EmployeeSalaryService,
        private staffSvc: StaffDetailsService,
        private earningTypeSvc: EarningTypeService,
        private deductionTypeSvc: DeductionTypeService
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.svc.get('/employeeSalaries'),
            this.staffSvc.getBySearchDetails(Status.Active, null as any, null as any),
            this.earningTypeSvc.get('/earningTypes'),
            this.deductionTypeSvc.get('/deductionTypes')
        ]).subscribe({
            next: ([salaries, staff, earnings, deductions]) => {
                this.items = salaries || [];
                this.staffList = (staff || []).sort((a: any, b: any) => (a.fullName || '').localeCompare(b.fullName || ''));
                this.earningTypes = (earnings || []).filter((e: EarningType) => e.isActive);
                this.deductionTypes = (deductions || []).filter((d: DeductionType) => d.isActive);
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    load() {
        this.svc.get('/employeeSalaries').subscribe({
            next: (r) => this.items = r || [],
            error: (err) => this.toastr.error(err.error)
        });
    }

    filtered(): EmployeeSalary[] {
        return (this.items || []).filter((s) => {
            if (this.filterStaffId != null && s.staffDetailsId != this.filterStaffId) return false;
            if (this.filterActive != null && s.isActive !== this.filterActive) return false;
            return true;
        });
    }

    clearFilters() { this.filterStaffId = null; this.filterActive = null; this.page = 1; }

    addNew() {
        let today = new Date().toISOString().substring(0, 10);
        this.item = new EmployeeSalary({
            isActive: true,
            basicSalary: 0,
            houseAllowance: 0,
            transportAllowance: 0,
            otherAllowances: 0,
            effectiveDate: today,
            items: []
        });
        this.editMode = false;
        this.showForm = true;
    }

    edit(x: EmployeeSalary) {
        this.svc.getById(parseInt(x.id), '/employeeSalaries').subscribe({
            next: (raw: any) => {
                let full: any = Array.isArray(raw) ? raw[0] : raw;
                this.item = new EmployeeSalary(full);
                this.item.items = (full?.items || []).map((i: any) => Object.assign(new EmployeeSalaryItem(), i));
                this.item.effectiveDate = full.effectiveDate ? full.effectiveDate.substring(0, 10) : '';
                this.editMode = true;
                this.showForm = true;
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    cancel() { this.showForm = false; }

    // --- Items sub-table ---
    getEarningItems(): EmployeeSalaryItem[] {
        return (this.item.items || []).filter((i) => i.earningTypeId != null);
    }

    getDeductionItems(): EmployeeSalaryItem[] {
        return (this.item.items || []).filter((i) => i.deductionTypeId != null);
    }

    addEarningItem() {
        this.item.items.push(Object.assign(new EmployeeSalaryItem(), {earningTypeId: null, deductionTypeId: null, amount: 0}));
    }

    addDeductionItem() {
        this.item.items.push(Object.assign(new EmployeeSalaryItem(), {deductionTypeId: null, earningTypeId: null, amount: 0}));
    }

    removeItem(line: EmployeeSalaryItem) {
        let i = this.item.items.indexOf(line);
        if (i >= 0) this.item.items.splice(i, 1);
    }

    onEarningTypeChange(line: EmployeeSalaryItem) {
        line.deductionTypeId = null;
    }

    onDeductionTypeChange(line: EmployeeSalaryItem) {
        line.earningTypeId = null;
    }

    getTotalEarnings(): number {
        return this.getEarningItems().reduce((s, i) => s + (+i.amount || 0), 0);
    }

    getTotalDeductions(): number {
        return this.getDeductionItems().reduce((s, i) => s + (+i.amount || 0), 0);
    }

    getGrossTotal(): number {
        return (+this.item.basicSalary || 0) + (+this.item.houseAllowance || 0) +
            (+this.item.transportAllowance || 0) + (+this.item.otherAllowances || 0) +
            this.getTotalEarnings();
    }

    save() {
        if (!this.item.staffDetailsId) { this.toastr.warning('Staff member is required.'); return; }
        if (!this.item.effectiveDate) { this.toastr.warning('Effective date is required.'); return; }
        let payload = new EmployeeSalary({...this.item, items: this.item.items});
        let req = this.editMode
            ? this.svc.updateById(parseInt(this.item.id), payload)
            : this.svc.create('/employeeSalaries', payload);
        req.subscribe({
            next: (saved: any) => {
                this.toastr.success('Saved.');
                if (!this.editMode && saved?.id) {
                    this.svc.getById(+saved.id, '/employeeSalaries').subscribe({
                        next: (raw: any) => {
                            let full: any = Array.isArray(raw) ? raw[0] : raw;
                            this.item = new EmployeeSalary(full);
                            this.item.items = (full?.items || []).map((i: any) => Object.assign(new EmployeeSalaryItem(), i));
                            this.item.effectiveDate = full.effectiveDate ? full.effectiveDate.substring(0, 10) : '';
                            this.editMode = true;
                            this.load();
                        }
                    });
                } else {
                    this.showForm = false;
                    this.load();
                }
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    }

    delete(x: EmployeeSalary) {
        Swal.fire({title: 'Delete?', icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (r.value) {
                this.svc.delete('/employeeSalaries', parseInt(x.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    }

    getStaffName(staffId: any): string {
        let s = this.staffList.find((x: any) => x.id == staffId);
        return s ? (s.fullName || `${s.firstName || ''} ${s.lastName || ''}`.trim()) : '';
    }
}
