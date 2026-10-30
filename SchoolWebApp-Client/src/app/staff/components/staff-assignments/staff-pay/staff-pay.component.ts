import {Component, ElementRef, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import {ProfilePayslip, ProfilesService} from '@/core/services/profiles.service';
import {Payslip, P9Employee} from '@/payroll/models/payroll-models';
import {PAYSLIP_PRINT_CSS} from '@/payroll/components/payslips/payslip-print.styles';
import {payrollReportPrintCss} from '@/payroll/components/payroll-reports/payroll-report-print.styles';
import {printInFrame, removePrintFrame} from '@/shared/utils/print-frame';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';

/**
 * Staff page, Pay tab: the staff member's payslips, newest first, each
 * printable. Staff see their own once the payroll is approved; payroll officers
 * see every one (the API decides). Above them, the P9 tax card of each year that
 * has closed (ended, every payroll approved).
 */
@Component({
    selector: 'app-staff-pay',
    styles: [`.slip-source { position: absolute; left: -10000px; top: 0; }`],
    template: `
        <!-- P9 tax cards, one per closed year: above the payslips, which run long -->
        <div class="mb-3 border rounded p-2 bg-light" *ngIf="p9Years.length">
            <h6 class="mb-1"><i class="fas fa-id-card text-primary me-1"></i> P9 tax cards</h6>
            <div class="d-flex flex-wrap gap-2 align-items-center">
                <button *ngFor="let y of p9Years" class="btn btn-sm btn-outline-primary" [disabled]="printingP9 === y" (click)="printP9(y)"
                    [title]="'Print the P9 for ' + y + ' - for filing your tax return'">
                    <i class="fas" [ngClass]="printingP9 === y ? 'fa-spinner fa-spin' : 'fa-print'"></i> P9 {{ y }}
                </button>
                <small class="text-muted">A year's P9 is available once the year has ended and all its payroll is approved.</small>
            </div>
        </div>
        <div *ngIf="loading" class="text-muted p-2"><i class="fas fa-spinner fa-spin"></i> Loading the payslips...</div>
        <div *ngIf="!loading && error" class="text-muted p-2">{{ error }}</div>
        <div *ngIf="!loading && !error && slips.length === 0" class="text-muted p-2">No payslips yet.</div>
        <table class="table table-sm table-striped mb-0 align-middle" *ngIf="!loading && slips.length">
            <thead class="table-light"><tr><th>Period</th><th class="text-end">Gross pay</th><th class="text-end">Deductions</th>
                <th class="text-end">Net pay</th><th></th><th style="width: 1%;"></th></tr></thead>
            <tbody>
                <tr *ngFor="let p of slips">
                    <td><b>{{ p.period }}</b></td>
                    <td class="text-end">{{ p.grossPay | number:'1.2-2' }}</td>
                    <td class="text-end">{{ p.totalDeductions | number:'1.2-2' }}</td>
                    <td class="text-end fw-bold">{{ p.netPay | number:'1.2-2' }}</td>
                    <td><span class="badge" *ngIf="p.status !== 'Approved' && p.status !== 'Posted'" [ngClass]="'bg-warning text-dark'"
                        title="Not yet approved - only payroll officers see it">{{ p.status }}</span></td>
                    <td class="text-nowrap"><button class="btn btn-sm btn-outline-primary py-0" [disabled]="printing === p.id" (click)="print(p)">
                        <i class="fas" [ngClass]="printing === p.id ? 'fa-spinner fa-spin' : 'fa-print'"></i> Payslip</button></td>
                </tr>
            </tbody>
        </table>
        <!-- Print source: never shown -->
        <div class="slip-source" #source>
            <div class="payslip-pair" *ngIf="slip">
                <app-payslip-slip [p]="slip" [schoolName]="schoolName" [schoolLogo]="schoolLogo" [employerKraPin]="employerKraPin"
                    [periodLabel]="slipPeriod"></app-payslip-slip>
            </div>
        </div>
        <div class="slip-source" #p9Source>
            <app-p9-card *ngIf="p9" [e]="p9" [year]="p9Year" [schoolName]="schoolName" [employerKraPin]="employerKraPin"></app-p9-card>
        </div>`
})
export class StaffPayComponent implements OnInit, OnDestroy {
    @Input() staffId!: number;
    @ViewChild('source') source?: ElementRef<HTMLElement>;
    @ViewChild('p9Source') p9Source?: ElementRef<HTMLElement>;
    p9Years: number[] = [];
    p9: P9Employee | null = null;
    p9Year: number | null = null;
    printingP9: number | null = null;
    slips: ProfilePayslip[] = [];
    loading = false;
    error = '';
    printing: number | null = null;
    slip: Payslip | null = null;
    slipPeriod = '';
    schoolName = '';
    schoolLogo: string | null = null;
    employerKraPin = '';

    constructor(private svc: ProfilesService, private toastr: ToastrService, private schoolSvc: SchoolDetailsService,
                private globalSettingSvc: GlobalSettingService) {}

    ngOnInit(): void {
        this.loading = true;
        this.svc.staffPayslips(+this.staffId).subscribe({
            next: (s) => {
                this.loading = false;
                this.slips = s;
            },
            error: (err) => {
                this.loading = false;
                this.error = err.status === 403 ? 'Only this staff member and payroll officers can see these payslips.' : 'The payslips could not be loaded.';
            }
        });
        this.svc.staffP9Years(+this.staffId).subscribe({next: (y) => this.p9Years = y || [], error: () => {}});
        forkJoin([this.schoolSvc.get('/schoolDetails'), this.globalSettingSvc.getByKey('Payroll', 'EmployerKraPin')]).subscribe({
            next: ([school, kraPin]: any[]) => {
                const s = (school || [])[0];
                this.schoolName = s?.name || '';
                this.schoolLogo = s?.logoAsBase64 || null;
                this.employerKraPin = kraPin?.settingValue || '';
            },
            error: () => {}
        });
    }

    print(p: ProfilePayslip) {
        this.printing = p.id;
        this.svc.staffPayslip(+this.staffId, p.id).subscribe({
            next: (slip) => {
                this.slip = slip;
                this.slipPeriod = p.period;
                // Let Angular render the slip before copying it.
                setTimeout(() => {
                    this.printing = null;
                    const body = this.source?.nativeElement.innerHTML || '';
                    if (body.trim()) printInFrame(`Payslip - ${p.period}`, PAYSLIP_PRINT_CSS, body);
                }, 150);
            },
            error: () => {
                this.printing = null;
                this.toastr.error('The payslip could not be loaded.');
            }
        });
    }

    printP9(year: number) {
        this.printingP9 = year;
        this.svc.staffP9(+this.staffId, year).subscribe({
            next: (card) => {
                this.p9 = card;
                this.p9Year = year;
                // Let Angular render the card before copying it.
                setTimeout(() => {
                    this.printingP9 = null;
                    const body = this.p9Source?.nativeElement.innerHTML || '';
                    if (body.trim()) printInFrame(`P9 ${year}`, payrollReportPrintCss('landscape'), body, 'landscape');
                }, 150);
            },
            error: () => {
                this.printingP9 = null;
                this.toastr.error('The P9 could not be loaded.');
            }
        });
    }

    ngOnDestroy(): void {
        removePrintFrame();
    }
}
