import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {BudgetMaster, BudgetMasterStatus} from '@/finance/models/budget-master';
import {BudgetMasterService} from '@/finance/services/finance-services';
import {AcademicYearsService} from '@/school/services/academic-years.service';

@Component({
    selector: 'app-finance-budget-masters',
    templateUrl: './budget-masters.component.html'
})
export class FinanceBudgetMastersComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/budget-register'], title: 'Budget Register'}
    ];
    dashboardTitle = 'Finance: Budget Register';

    items: BudgetMaster[] = [];
    academicYears: any[] = [];
    item: BudgetMaster = new BudgetMaster({status: BudgetMasterStatus.Draft});
    editMode: boolean = false;
    showForm: boolean = false;

    filterYearId: any = null;
    filterStatus: any = null;

    statuses = [
        {value: BudgetMasterStatus.Draft, label: 'Draft'},
        {value: BudgetMasterStatus.Open, label: 'Open'},
        {value: BudgetMasterStatus.Closed, label: 'Closed'}
    ];

    page: number = 1;
    pageSize: number = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(
        private toastr: ToastrService,
        private svc: BudgetMasterService,
        private yearSvc: AcademicYearsService
    ) {}

    ngOnInit(): void {
        forkJoin([
            this.svc.get('/budgetMasters'),
            this.yearSvc.get('/academicYears')
        ]).subscribe({
            next: ([items, years]) => {
                this.items = items || [];
                this.academicYears = (years || []).sort((a: any, b: any) => (b.rank || 0) - (a.rank || 0));
                let active = this.academicYears.find((y: any) => y.status === true);
                this.filterYearId = active ? active.id : (this.academicYears[0]?.id || null);
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    load = () => {
        this.svc.get('/budgetMasters').subscribe({
            next: (items) => { this.items = items || []; },
            error: (err) => this.toastr.error(err.error)
        });
    };

    filtered = (): BudgetMaster[] => {
        return (this.items || []).filter((b) => {
            if (this.filterYearId != null && b.academicYearId != this.filterYearId) return false;
            if (this.filterStatus != null && b.status !== this.filterStatus) return false;
            return true;
        });
    };

    clearFilters = () => {
        let active = this.academicYears.find((y: any) => y.status === true);
        this.filterYearId = active ? active.id : (this.academicYears[0]?.id || null);
        this.filterStatus = null;
        this.page = 1;
    };

    addNew = () => {
        let today = new Date();
        let yearStart = new Date(today.getFullYear(), 0, 1);
        let yearEnd = new Date(today.getFullYear(), 11, 31);
        this.item = new BudgetMaster({
            status: BudgetMasterStatus.Draft,
            startDate: yearStart.toISOString().substring(0, 10),
            endDate: yearEnd.toISOString().substring(0, 10),
            academicYearId: this.filterYearId || undefined,
            code: 'Auto'
        });
        this.editMode = false;
        this.showForm = true;
    };

    edit = (x: BudgetMaster) => {
        this.item = new BudgetMaster(x);
        this.item.startDate = x.startDate ? x.startDate.substring(0, 10) : '';
        this.item.endDate = x.endDate ? x.endDate.substring(0, 10) : '';
        this.editMode = true;
        this.showForm = true;
    };

    cancel = () => { this.showForm = false; };

    save = () => {
        if (!this.item.name || !this.item.academicYearId || !this.item.startDate || !this.item.endDate) {
            this.toastr.info('Name, academic year, start date and end date are required.');
            return;
        }
        let payload = new BudgetMaster({...this.item});
        if (!this.editMode && payload.code === 'Auto') payload.code = '';
        let req = this.editMode ? this.svc.update('/budgetMasters', payload) : this.svc.create('/budgetMasters', payload);
        req.subscribe({
            next: () => { this.toastr.success('Saved.'); this.showForm = false; this.load(); },
            error: (err) => this.toastr.error(err.error?.message || 'Error saving.')
        });
    };

    delete = (x: BudgetMaster) => {
        Swal.fire({
            title: 'Delete budget plan?', icon: 'warning', showCancelButton: true,
            confirmButtonText: 'Delete', confirmButtonColor: '#d33'
        }).then((r) => {
            if (r.value) {
                this.svc.delete('/budgetMasters', parseInt(x.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message)
                });
            }
        });
    };

    getStatusLabel = (s: any): string => {
        return this.statuses.find((x) => x.value === s)?.label || '';
    };

    getStatusBadgeClass = (s: any): string => {
        if (s === BudgetMasterStatus.Draft) return 'bg-secondary';
        if (s === BudgetMasterStatus.Open) return 'bg-success';
        if (s === BudgetMasterStatus.Closed) return 'bg-dark';
        return 'bg-secondary';
    };
}
