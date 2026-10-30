import {Component, OnInit, ViewChild} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {formatNumber} from '@angular/common';
import {AccountingPeriodService} from '@/finance/services/finance-services';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {FinanceDrilldownComponent} from '@/finance/components/reports/drilldown/finance-drilldown.component';

// The accounting periods of a year - terms or the whole year, per the Finance setting - with what each earned and
// spent, and closing them: the surplus goes to Retained Earnings and the period is locked. An administrator can
// reopen the latest closed period to correct it.
@Component({
    selector: 'app-finance-accounting-periods',
    templateUrl: './accounting-periods.component.html'
})
export class AccountingPeriodsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/accounting-periods'], title: 'Accounting Periods'}
    ];
    dashboardTitle = 'Finance: Accounting Periods';
    @ViewChild('drill') drillDown!: FinanceDrilldownComponent;

    academicYears: any[] = [];
    yearId: any = null;
    periods: any[] = [];
    termly = true;
    loading = false;
    busy = false;

    constructor(private svc: AccountingPeriodService, private yearSvc: AcademicYearsService, private toastr: ToastrService) {}

    ngOnInit(): void {
        this.yearSvc.get('/academicYears').subscribe({
            next: (years: any[]) => {
                this.academicYears = (years || []).sort((a: any, b: any) => (b.rank || 0) - (a.rank || 0));
                this.yearId = this.academicYears.find((y: any) => y.status === true)?.id ?? this.academicYears[0]?.id ?? null;
                this.load();
            },
            error: () => this.load()
        });
    }

    load = () => {
        this.loading = true;
        this.svc.list(this.yearId).subscribe({
            next: (r) => { this.periods = r.periods || []; this.termly = r.termly; this.loading = false; },
            error: (err) => { this.loading = false; this.toastr.error(err.error?.message || 'Error loading the periods.'); }
        });
    };

    private money = (v: number) => formatNumber(v || 0, 'en', '1.2-2');

    close = (p: any) => {
        Swal.fire({
            title: `Close ${p.name}?`,
            html: `<div style="text-align:left; font-size:0.9rem;">
                <p>Income <strong>${this.money(p.income)}</strong> less expenses <strong>${this.money(p.expenses)}</strong>:
                a ${p.net >= 0 ? 'surplus' : 'deficit'} of <strong>${this.money(Math.abs(p.net))}</strong> goes to Retained Earnings.</p>
                <p>Then nothing dated ${p.startDate.substring(0, 10)} to ${p.endDate.substring(0, 10)} can be recorded, changed or deleted.
                Students' unpaid fees carry into the next period as their balance brought forward, and can still be paid.</p>
            </div>`,
            icon: 'question', width: 520, position: 'top', padding: '1em',
            showCancelButton: true, confirmButtonText: 'Close the period'
        }).then((r) => {
            if (!r.value) return;
            this.busy = true;
            this.svc.close(p.key).subscribe({
                next: (res: any) => { this.busy = false; this.toastr.success(res.message || 'Closed.'); this.load(); },
                error: (err) => { this.busy = false; this.toastr.error(err.error?.message || 'Could not close the period.'); }
            });
        });
    };

    reopen = (p: any) => {
        Swal.fire({
            title: `Reopen ${p.name}?`,
            text: 'Its closing entry is taken out and it can be changed again. Close it again when the correction is done.',
            icon: 'warning', width: 460, position: 'top', padding: '1em',
            showCancelButton: true, confirmButtonColor: '#d33', confirmButtonText: 'Reopen'
        }).then((r) => {
            if (!r.value) return;
            this.busy = true;
            this.svc.reopen(p.key).subscribe({
                next: () => { this.busy = false; this.toastr.success(`${p.name} reopened.`); this.load(); },
                error: (err) => { this.busy = false; this.toastr.error(err.error?.message || 'Could not reopen the period.'); }
            });
        });
    };

    openJournal = (p: any) => {
        if (p.closingJournalId) this.drillDown.show({kind: 'document', type: 'journal', id: p.closingJournalId});
    };

    statusOf = (p: any): {label: string; css: string} =>
        p.closed ? {label: 'Closed', css: 'bg-secondary'}
            : p.current ? {label: 'Current', css: 'bg-primary'}
            : p.blockers?.length ? {label: 'Open', css: 'bg-warning text-dark'}
            : {label: 'Ready to close', css: 'bg-success'};
}
