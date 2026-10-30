import {Component, HostListener, Input, OnDestroy} from '@angular/core';
import {formatDate, formatNumber} from '@angular/common';
import {Observable, map} from 'rxjs';
import {ToastrService} from 'ngx-toastr';
import {FinanceReportsService} from '@/finance/services/finance-services';
import {downloadCsv} from '@/core/utils/csv-export';
import {escapeHtml, printInFrame} from '@/shared/utils/print-frame';

// What a report figure opens. A server row's own `drill` is one of these too
// ({kind: 'document', type, id} or {kind: 'ledger', accountId}).
export type DrillTarget =
    | {kind: 'ledger'; accountId: number; from?: string; to?: string}
    | {kind: 'budgetLine'; academicYearId: number; departmentId: number; accountId: number; shared: boolean; from: string; to: string}
    | {kind: 'statement'; studentId: number; academicYearId?: any; sessionId?: any; from?: string; to?: string; label?: string}
    | {kind: 'expenses'; title: string; subtitle?: string; rows: any[]}
    | {kind: 'document'; type: string; id: number};

interface DrillRow {cells: any[]; drill?: any}
interface DrillSection {title?: string; columns: string[]; rows: DrillRow[]; footer?: any[]}
interface DrillField {label: string; value: any; drill?: any}
interface DrillView {title: string; subtitle?: string; icon?: string; status?: string; note?: string; fields: DrillField[]; sections: DrillSection[]}

const DOC_ICONS: Record<string, string> = {
    invoice: 'fa-file-invoice', payment: 'fa-receipt', expense: 'fa-money-bill-wave',
    journal: 'fa-pen', sponsorPayment: 'fa-hand-holding-usd'
};

// A pop-up over a finance report that lists the transactions behind a figure.
// Every row that stands for a document opens it, one level deeper; the back
// arrow (or the trail) returns, and closing leaves the report as it was.
@Component({
    selector: 'app-finance-drilldown',
    templateUrl: './finance-drilldown.component.html',
    styleUrls: ['./finance-drilldown.component.scss']
})
export class FinanceDrilldownComponent implements OnDestroy {
    // Opened from a list page: the page's name, shown first in the trail with a back arrow to it.
    @Input() backLabel = '';
    stack: DrillView[] = [];
    loading = false;
    // Long lists show this many rows until "Show all" is pressed.
    limits = new Map<DrillSection, number>();
    // The report's period, for ledgers opened from inside a document.
    private from = '';
    private to = '';

    constructor(private svc: FinanceReportsService, private toastr: ToastrService) {}

    get open(): boolean { return this.stack.length > 0 || this.loading; }
    get view(): DrillView | undefined { return this.stack[this.stack.length - 1]; }

    show(target: DrillTarget, period?: {from: string; to: string}) {
        if (period) { this.from = period.from; this.to = period.to; }
        this.stack = [];
        this.lockPage(true);
        this.push(target);
    }

    drill(d: any) {
        if (!d || this.loading) return;
        this.push(d.kind === 'ledger' ? {kind: 'ledger', accountId: d.accountId} : d);
    }

    back() { if (this.stack.length > 1) this.stack.pop(); else this.close(); }
    backTo(i: number) { this.stack = this.stack.slice(0, i + 1); }
    close() { this.stack = []; this.loading = false; this.lockPage(false); }

    // While open, the page behind does not scroll (Bootstrap's modal-open), so only the pop-up has a scroll bar.
    private lockPage(on: boolean) { document.body.classList.toggle('modal-open', on); }
    ngOnDestroy() { this.lockPage(false); }

    @HostListener('document:keydown.escape')
    onEscape() { if (this.open) this.close(); }

    limit(s: DrillSection): number { return this.limits.get(s) ?? 200; }
    showAll(s: DrillSection) { this.limits.set(s, s.rows.length); }

    private push(target: DrillTarget) {
        this.loading = true;
        this.load(target).subscribe({
            next: (v) => { this.stack.push(this.dropEmptyColumns(v)); this.loading = false; },
            error: (err) => { this.loading = false; if (!this.stack.length) this.lockPage(false);this.toastr.error(err.error?.message || 'Could not load the details.'); }
        });
    }

    // A column with nothing in any row (an unused description, say) is left out.
    private dropEmptyColumns(v: DrillView): DrillView {
        let empty = (x: any) => x === null || x === undefined || x === '';
        for (let s of v.sections) {
            if (!s.rows.length) continue;
            let keep = s.columns.map((_, i) => s.rows.some((r) => !empty(r.cells[i])));
            s.columns = s.columns.filter((_, i) => keep[i]);
            s.rows.forEach((r) => (r.cells = r.cells.filter((_, i) => keep[i])));
            if (s.footer) s.footer = s.footer.filter((_, i) => keep[i]);
        }
        return v;
    }

    private load(t: DrillTarget): Observable<DrillView> {
        switch (t.kind) {
            case 'ledger': return this.ledger(t.accountId, t.from || this.from, t.to || this.to);
            case 'budgetLine': return t.shared ? this.budgetLine(t) : this.ledger(t.accountId, t.from, t.to);
            case 'statement': return this.statement(t);
            case 'document': return this.svc.document(t.type, t.id).pipe(map((d) => ({
                title: `${d.kind} ${d.reference}`,
                subtitle: this.fmt(d.date),
                icon: DOC_ICONS[t.type],
                status: d.status,
                note: d.note,
                fields: d.fields || [],
                sections: (d.sections || []).filter((s: DrillSection) => s.rows.length)
            })));
            case 'expenses': return new Observable<DrillView>((o) => {
                o.next({title: t.title, subtitle: t.subtitle, icon: 'fa-money-bill-wave', fields: [], sections: [{
                    columns: ['Date', 'Reference', 'Category', 'Vendor', 'Description', 'Amount'],
                    rows: t.rows.map((r) => ({cells: [r.date, r.referenceNumber, r.category, r.vendor, r.description, r.amount],
                        drill: {kind: 'document', type: 'expense', id: r.expenseId}})),
                    footer: ['Total', null, null, null, null, t.rows.reduce((s, r) => s + (r.amount || 0), 0)]
                }]});
                o.complete();
            });
        }
    }

    private ledger(accountId: number, from: string, to: string): Observable<DrillView> {
        return this.svc.accountLedger(accountId, from, to).pipe(map((r) => ({
            title: `${r.account.code} - ${r.account.name}`,
            subtitle: `${r.account.type} ledger, ${this.fmt(r.from)} to ${this.fmt(r.to)}`,
            icon: 'fa-book',
            note: 'Running balance is the balance of this account after each line, not the balance of the person named on it.',
            fields: [
                {label: 'Opening balance', value: r.opening}, {label: 'Debits', value: r.totalDebit},
                {label: 'Credits', value: r.totalCredit}, {label: 'Closing balance', value: r.closing}
            ],
            sections: [{
                columns: ['Date', 'Reference', 'Name', 'Description', 'Debit', 'Credit', 'Running balance'],
                rows: r.lines.map((l: any) => ({cells: [l.date, l.reference, l.party, l.description, l.debit || null, l.credit || null, l.balance], drill: l.drill})),
                footer: ['Total', null, null, null, r.totalDebit, r.totalCredit, r.closing]
            }]
        })));
    }

    private budgetLine(t: {academicYearId: number; departmentId: number; accountId: number}): Observable<DrillView> {
        return this.svc.budgetLineExpenses(t.academicYearId, t.departmentId, t.accountId).pipe(map((r) => ({
            title: `${r.department}: ${r.account}`,
            subtitle: 'Approved expenses charged to this budget line',
            icon: 'fa-piggy-bank',
            fields: [],
            sections: [{
                columns: ['Date', 'Reference', 'Category', 'Vendor', 'Description', 'Amount'],
                rows: r.lines.map((l: any) => ({cells: [l.date, l.reference, l.category, l.vendor, l.description, l.amount], drill: l.drill})),
                footer: ['Total', null, null, null, null, r.total]
            }]
        })));
    }

    private statement(t: {studentId: number; academicYearId?: any; sessionId?: any; from?: string; to?: string; label?: string}): Observable<DrillView> {
        return this.svc.studentStatement(t.studentId, t.academicYearId, t.sessionId, t.from, t.to).pipe(map((r) => ({
            title: `${r.student.upi} - ${r.student.fullName}`,
            subtitle: 'Fee statement' + (t.label ? `, ${t.label}` : ''),
            icon: 'fa-file-invoice-dollar',
            note: 'Balance is what the student owes after each line.',
            fields: [{label: 'Invoiced', value: r.totalInvoiced}, {label: 'Paid', value: r.totalPaid}, {label: 'Balance', value: r.balance}],
            sections: [{
                columns: ['Date', 'Type', 'Reference', 'Description', 'Method', 'Debit', 'Credit', 'Balance'],
                rows: r.transactions.map((x: any) => ({cells: [x.date, x.type, x.reference, x.description, x.method, x.debit || null, x.credit || null, x.balance],
                    drill: x.sourceId ? {kind: 'document', type: x.sourceType, id: x.sourceId} : null})),
                footer: ['Total', null, null, null, null, r.totalInvoiced, r.totalPaid, r.balance]
            }]
        })));
    }

    // ---------- Display ----------

    isNumber(v: any): boolean { return typeof v === 'number'; }

    // Money figures show as tiles across the top; everything else as a details grid.
    tiles(v: DrillView): DrillField[] { return v.fields.filter((f) => typeof f.value === 'number'); }
    details(v: DrillView): DrillField[] { return v.fields.filter((f) => typeof f.value !== 'number' && f.value !== null && f.value !== undefined && f.value !== ''); }

    // The amount a document is for leads in green; the balance it leaves stands out, red when something is owed.
    tileClass(f: DrillField): string {
        if (/^amount/i.test(f.label)) return 'main';
        if (/closing/i.test(f.label)) return 'result';
        if (/balance/i.test(f.label) && !/opening/i.test(f.label)) return 'result' + (f.value > 0 ? ' owing' : '');
        return '';
    }

    statusClass(s: string): string {
        if (/reject|cancel|void/i.test(s)) return 'bg-danger';
        if (/partial|pending|draft|submitted|review/i.test(s)) return 'bg-warning text-dark';
        if (/paid|approved|posted|active/i.test(s)) return 'bg-success';
        return 'bg-light text-dark';
    }

    // Right-align a header over a column of figures.
    numericColumn(s: DrillSection, i: number): boolean {
        return s.rows.some((r) => typeof r.cells[i] === 'number') || (s.footer ? typeof s.footer[i] === 'number' : false);
    }

    fmt(v: any): string {
        if (v === null || v === undefined || v === '') return '';
        if (typeof v === 'number') return formatNumber(v, 'en', '1.2-2');
        if (typeof v === 'string' && /^\d{4}-\d{2}-\d{2}T/.test(v)) return formatDate(v, 'dd MMM yyyy', 'en');
        return String(v);
    }

    // ---------- Export ----------

    exportCsv() {
        let v = this.view!;
        let main = v.sections.reduce((a, b) => (b.rows.length > a.rows.length ? b : a), v.sections[0]);
        if (!main) return;
        downloadCsv(v.title.replace(/[^\w\- ]+/g, ''), main.columns.map((c, i) => ({header: c, value: (r: DrillRow) => {
            let x = r.cells[i];
            return typeof x === 'number' ? x : this.fmt(x);
        }})), main.rows);
    }

    print() {
        let v = this.view!;
        let cell = (x: any) => `<td class="${typeof x === 'number' ? 'r' : ''}">${escapeHtml(this.fmt(x))}</td>`;
        let body = `<h1>${escapeHtml(v.title)}</h1><h2>${escapeHtml([v.subtitle, v.status].filter(Boolean).join(' · '))}</h2>`
            + (v.fields.length ? `<table class="f">${v.fields.map((f) => `<tr><th>${escapeHtml(f.label)}</th>${cell(f.value)}</tr>`).join('')}</table>` : '')
            + v.sections.map((s) => `${s.title ? `<h3>${escapeHtml(s.title)}</h3>` : ''}<table>
                <tr>${s.columns.map((c) => `<th>${escapeHtml(c)}</th>`).join('')}</tr>
                ${s.rows.map((r) => `<tr>${r.cells.map(cell).join('')}</tr>`).join('')}
                ${s.footer ? `<tr class="t">${s.footer.map(cell).join('')}</tr>` : ''}</table>`).join('');
        let wide = v.sections.some((s) => s.columns.length > 6);
        let css = `@page { size: A4 ${wide ? 'landscape' : 'portrait'}; margin: 12mm; }
            body { font-family: Arial, Helvetica, sans-serif; font-size: 10px; color: #222; }
            h1 { font-size: 15px; margin: 0; } h2 { font-size: 11px; margin: 2px 0 10px; font-weight: normal; color: #555; }
            h3 { font-size: 12px; margin: 12px 0 4px; }
            table { border-collapse: collapse; width: 100%; } th, td { border: 1px solid #aaa; padding: 2px 5px; text-align: left; }
            table.f { width: auto; margin-bottom: 8px; } table.f th { background: #f1f5f9; }
            .r { text-align: right; } tr.t td { font-weight: bold; background: #fef9c3; }`;
        printInFrame(v.title, css, body, wide ? 'landscape' : 'portrait');
    }
}
