import {Component, HostBinding, Input} from '@angular/core';
import {P9Employee, P9Row} from '@/payroll/models/payroll-models';

/**
 * One printed P9A tax deduction card, a page to itself. Used by Payroll >
 * Reports > P9 Tax Cards and by a staff member's Pay tab, printed with
 * payrollReportPrintCss('landscape') (the host is the .rp-page).
 */
@Component({
    selector: 'app-p9-card',
    template: `
        <div class="rp-form">KENYA REVENUE AUTHORITY - DOMESTIC TAXES DEPARTMENT</div>
        <div class="rp-header">
            <div class="rp-title">P9A - TAX DEDUCTION CARD YEAR {{ year }}</div>
        </div>
        <div class="p9-details">
            <div><div class="k">Employer's name</div><div class="v">{{ schoolName }}</div></div>
            <div><div class="k">Employer's PIN</div><div class="v">{{ employerKraPin || '-' }}</div></div>
            <div><div class="k">Employee's main name</div><div class="v">{{ mainName() }}</div></div>
            <div><div class="k">Employee's other names</div><div class="v">{{ otherNames() || '-' }}</div></div>
            <div><div class="k">Employee's PIN</div><div class="v">{{ e.kraPin || '-' }}</div></div>
        </div>
        <table class="rp p9">
            <thead>
                <tr>
                    <th>Month</th>
                    <th *ngFor="let c of cols" class="num">{{ c.title }}</th>
                </tr>
                <tr>
                    <th></th>
                    <th *ngFor="let c of cols" class="code">{{ c.code }}</th>
                </tr>
            </thead>
            <tbody>
                <tr *ngFor="let m of months()">
                    <td class="month">{{ m.name }}</td>
                    <td *ngFor="let c of cols" class="num">{{ m.row ? (m.row[c.key] | number:'1.2-2') : '' }}</td>
                </tr>
                <tr class="totals">
                    <td>TOTALS</td>
                    <td *ngFor="let c of cols" class="num">{{ total(c.key) | number:'1.2-2' }}</td>
                </tr>
            </tbody>
        </table>
        <div class="p9-foot">
            <div>
                <div>Total chargeable pay (col. H): <strong>KES {{ total('chargeable') | number:'1.2-2' }}</strong></div>
                <div>Total tax (col. L): <strong>KES {{ total('paye') | number:'1.2-2' }}</strong></div>
                <div style="margin-top:2mm;">Housing levy, SHIF and PRMF are allowable deductions (Tax Laws (Amendment) Act 2024).</div>
            </div>
            <div>
                <div class="line"></div>
                <div>Employer's signature and stamp</div>
            </div>
            <div>
                <div>Printed {{ printedOn | date:'d MMM yyyy' }}</div>
            </div>
        </div>`
})
export class P9CardComponent {
    @HostBinding('class') hostClass = 'rp-page';
    @HostBinding('style.display') display = 'block';
    @Input() e!: P9Employee;
    @Input() year: number | null = null;
    @Input() schoolName = '';
    @Input() employerKraPin = '';
    @Input() printedOn = new Date();

    // KRA's P9A columns, in order. "code" is the column letter on the form.
    static readonly COLUMNS: {key: keyof P9Row; code: string; title: string}[] = [
        {key: 'basic', code: 'A', title: 'Basic salary'},
        {key: 'benefits', code: 'B', title: 'Benefits and allowances'},
        {key: 'quarters', code: 'C', title: 'Value of quarters'},
        {key: 'gross', code: 'D', title: 'Total gross pay'},
        {key: 'e1', code: 'E1', title: 'Retirement: % of A'},
        {key: 'e2', code: 'E2', title: 'Retirement: actual'},
        {key: 'e3', code: 'E3', title: 'Retirement: fixed'},
        {key: 'ownerOccupiedInterest', code: 'F', title: 'Owner-occupied interest'},
        {key: 'retirement', code: 'G', title: 'Retirement and owner-occupied interest (lowest of E, plus F)'},
        {key: 'ahl', code: '', title: 'Housing levy'},
        {key: 'shif', code: '', title: 'SHIF'},
        {key: 'otherDeductible', code: '', title: 'PRMF and other allowable'},
        {key: 'chargeable', code: 'H', title: 'Chargeable pay'},
        {key: 'taxCharged', code: 'J', title: 'Tax charged'},
        {key: 'personalRelief', code: 'K', title: 'Personal relief'},
        {key: 'insuranceRelief', code: '', title: 'Insurance relief'},
        {key: 'paye', code: 'L', title: 'PAYE tax (J - K)'}
    ];
    static readonly MONTHS = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August',
        'September', 'October', 'November', 'December'];
    readonly cols = P9CardComponent.COLUMNS;

    // All twelve months, as on the form; a month with no approved payslip is left blank.
    months(): {name: string; row: P9Row | null}[] {
        return P9CardComponent.MONTHS.map((name, i) => ({name, row: this.e.rows.find((r) => r.month === i + 1) || null}));
    }

    total(key: keyof P9Row): number {
        return this.e.rows.reduce((s, r) => s + (+r[key] || 0), 0);
    }

    // The form asks for the main name (surname) and other names, without the title.
    private plainName(): string[] {
        return (this.e.name || '').replace(/^(MR|MRS|MS|MD|MISS|DR|MADAM)\.?\s+/i, '').trim().split(/\s+/);
    }
    mainName(): string { let n = this.plainName(); return n.length > 1 ? n[n.length - 1] : n[0]; }
    otherNames(): string { let n = this.plainName(); return n.length > 1 ? n.slice(0, -1).join(' ') : ''; }
}
