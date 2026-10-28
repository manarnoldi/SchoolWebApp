import {escapeHtml} from '@/shared/utils/print-frame';

/** The school, as /schoolDetails gives it: the letterhead of every fee document. */
export interface FeeSchool {
    name?: string;
    address?: string;
    telephone?: string;
    email?: string;
    motto?: string;
    logoAsBase64?: string;
}

export interface FeeStudent {
    name: string;
    upi: string;
    className?: string | null;
    year?: string | null;
}

/** For printInFrame: A4 portrait, one document a page. */
export const FEE_PRINT_CSS = `
    @page { size: A4 portrait; margin: 12mm; }
    body { font-family: Arial, Helvetica, sans-serif; color: #212529; margin: 0; font-size: 12px; }
    .doc { page-break-after: always; break-after: page; }
    .doc:last-child { page-break-after: auto; break-after: auto; }
    .head { display: flex; align-items: center; gap: 12px; border-bottom: 2px solid #084298; padding-bottom: 8px; margin-bottom: 10px; }
    .head img { height: 64px; width: auto; }
    .head .school { font-size: 17px; font-weight: bold; color: #084298; text-transform: uppercase; }
    .head .contact { color: #52514e; font-size: 11px; }
    .head .motto { font-style: italic; color: #52514e; font-size: 11px; }
    .title { text-align: center; font-size: 15px; font-weight: bold; letter-spacing: .08em; margin: 6px 0 10px; text-transform: uppercase; }
    .meta { display: grid; grid-template-columns: 1fr 1fr; gap: 2px 24px; margin-bottom: 10px; }
    .meta div span { color: #52514e; display: inline-block; min-width: 110px; }
    table { width: 100%; border-collapse: collapse; margin-top: 4px; }
    th, td { border: 1px solid #ced4da; padding: 4px 6px; text-align: left; vertical-align: top; }
    th { background: #e7f1ff; }
    td.n, th.n { text-align: right; white-space: nowrap; }
    tr.total td { font-weight: bold; background: #f8f9fa; }
    .balance { margin-top: 10px; font-size: 14px; font-weight: bold; text-align: right; }
    .due { color: #842029; } .clear { color: #0f5132; }
    .foot { margin-top: 28px; display: flex; justify-content: space-between; color: #52514e; font-size: 11px; }
    .sign { border-top: 1px solid #6c757d; padding-top: 3px; min-width: 200px; text-align: center; }
    .printed { margin-top: 14px; color: #6c757d; font-size: 10px; text-align: right; }
`;

const esc = (s: any) => escapeHtml(s === null || s === undefined ? '' : String(s));
export const money = (n: number) => (+n || 0).toLocaleString('en-KE', {minimumFractionDigits: 2, maximumFractionDigits: 2});
const date = (d: string | null | undefined) => d ? new Date(d).toLocaleDateString('en-GB', {day: '2-digit', month: 'short', year: 'numeric'}) : '';

function letterhead(school: FeeSchool): string {
    const contact = [school.address, school.telephone ? 'Tel ' + school.telephone : '', school.email].filter(Boolean).map(esc).join(' &middot; ');
    return `<div class="head">
        ${school.logoAsBase64 ? `<img src="${school.logoAsBase64}" alt="">` : ''}
        <div><div class="school">${esc(school.name)}</div>
            ${contact ? `<div class="contact">${contact}</div>` : ''}
            ${school.motto ? `<div class="motto">${esc(school.motto)}</div>` : ''}</div>
    </div>`;
}

function studentMeta(st: FeeStudent, extra: [string, string][]): string {
    const rows: [string, string][] = [['Student', st.name], ['Admission No.', st.upi], ['Class', st.className || ''], ...extra];
    return `<div class="meta">${rows.map(([k, v]) => `<div><span>${esc(k)}:</span> <b>${esc(v)}</b></div>`).join('')}</div>`;
}

const printedOn = () => `<div class="printed">Printed ${esc(new Date().toLocaleString('en-GB'))}</div>`;

/** The fee statement: every invoice, receipt and note, with the running balance. */
export function statementDoc(school: FeeSchool, st: FeeStudent, statement: any): string {
    const rows = (statement.transactions || []).map((t: any) => `<tr><td>${date(t.date)}</td><td>${esc(t.type)}${t.method ? ' (' + esc(t.method) + ')' : ''}</td>
        <td>${esc(t.reference)}</td><td>${esc(t.description)}</td><td class="n">${t.debit ? money(t.debit) : ''}</td>
        <td class="n">${t.credit ? money(t.credit) : ''}</td><td class="n">${money(t.balance)}</td></tr>`).join('');
    const bal = +statement.balance || 0;
    return `<div class="doc">${letterhead(school)}<div class="title">Fee statement</div>
        ${studentMeta(st, [['Year', st.year || ''], ['Date', date(new Date().toISOString())]])}
        <table><thead><tr><th>Date</th><th>Type</th><th>Reference</th><th>Description</th><th class="n">Debit</th><th class="n">Credit</th><th class="n">Balance</th></tr></thead>
        <tbody>${rows || '<tr><td colspan="7">No invoices or payments.</td></tr>'}
            <tr class="total"><td colspan="4">Totals</td><td class="n">${money(statement.totalInvoiced)}</td><td class="n">${money(statement.totalPaid)}</td><td class="n">${money(bal)}</td></tr>
        </tbody></table>
        <div class="balance ${bal > 0 ? 'due' : 'clear'}">${bal > 0 ? 'Balance due' : bal < 0 ? 'In credit' : 'Balance'}: ${money(Math.abs(bal))}</div>
        ${printedOn()}</div>`;
}

/** One invoice: its fee items, discounts and payments, and what is still owed. */
export function invoiceDoc(school: FeeSchool, st: FeeStudent, inv: any): string {
    const rows = (inv.items || []).map((it: any) => `<tr><td>${esc(it.category)}${it.description ? ' - ' + esc(it.description) : ''}</td>
        <td class="n">${money(it.amount)}</td><td class="n">${it.discount ? money(it.discount) : ''}</td><td class="n">${money(it.paid)}</td>
        <td class="n">${money(it.amount - (it.discount || 0) - (it.paid || 0))}</td></tr>`).join('');
    return `<div class="doc">${letterhead(school)}<div class="title">Invoice ${esc(inv.number)}</div>
        ${studentMeta(st, [['Term', [inv.session, inv.year].filter(Boolean).join(' ')], ['Invoice date', date(inv.date)], ['Due date', date(inv.dueDate)], ['Status', String(inv.status || '').replace(/([a-z])([A-Z])/g, (m, a, b) => a + ' ' + b.toLowerCase())]])}
        ${inv.description ? `<div style="margin-bottom:6px;">${esc(inv.description)}</div>` : ''}
        <table><thead><tr><th>Item</th><th class="n">Amount</th><th class="n">Discount</th><th class="n">Paid</th><th class="n">Balance</th></tr></thead>
        <tbody>${rows}<tr class="total"><td>Total</td><td class="n">${money(inv.total)}</td><td class="n">${inv.discount ? money(inv.discount) : ''}</td>
            <td class="n">${money(inv.paid)}</td><td class="n">${money(inv.balance)}</td></tr></tbody></table>
        <div class="balance ${inv.balance > 0 ? 'due' : 'clear'}">Amount due: ${money(Math.max(0, inv.balance))}</div>
        <div class="foot"><div class="sign">Accounts office</div><div class="sign">Date</div></div>
        ${printedOn()}</div>`;
}

/** A payment receipt. */
export function receiptDoc(school: FeeSchool, st: FeeStudent, r: any, balanceAfter?: number | null): string {
    return `<div class="doc">${letterhead(school)}<div class="title">Official receipt</div>
        ${studentMeta(st, [['Receipt No.', r.number], ['Date', date(r.date)], ['Paid by', r.method], ['Reference', r.reference || '-']])}
        <table><thead><tr><th>Description</th><th>Invoice</th><th class="n">Amount</th></tr></thead>
        <tbody><tr><td>${esc(r.description || 'School fees')}</td><td>${esc(r.invoiceNumber || '-')}</td><td class="n">${money(r.amount)}</td></tr>
            <tr class="total"><td colspan="2">Amount received</td><td class="n">${money(r.amount)}</td></tr></tbody></table>
        ${balanceAfter !== null && balanceAfter !== undefined
            ? `<div class="balance ${balanceAfter > 0 ? 'due' : 'clear'}">Current fee balance: ${money(Math.abs(balanceAfter))}${balanceAfter < 0 ? ' (in credit)' : ''}</div>` : ''}
        <div class="foot"><div class="sign">Received by</div><div class="sign">Signature &amp; stamp</div></div>
        ${printedOn()}</div>`;
}

/** The fee structure for the student's grade: each fee and the total. */
export function feeStructureDoc(school: FeeSchool, st: FeeStudent, fs: any): string {
    const rows = (fs.items || []).map((it: any) => `<tr><td>${esc(it.category)}</td><td>${it.mandatory ? 'Required' : 'Optional'}</td>
        <td class="n">${money(it.amount)}</td></tr>`).join('');
    return `<div class="doc">${letterhead(school)}<div class="title">Fee structure</div>
        ${studentMeta(st, [['Fee structure', fs.name], ['Term', [fs.session || 'Whole year', fs.year].filter(Boolean).join(' ')]])}
        ${fs.description ? `<div style="margin-bottom:6px;">${esc(fs.description)}</div>` : ''}
        <table><thead><tr><th>Fee</th><th></th><th class="n">Amount</th></tr></thead>
        <tbody>${rows}<tr class="total"><td colspan="2">Total</td><td class="n">${money(fs.total)}</td></tr></tbody></table>
        ${printedOn()}</div>`;
}
