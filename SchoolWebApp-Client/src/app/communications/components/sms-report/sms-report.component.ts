import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {DatePipe, DecimalPipe} from '@angular/common';
import {MESSAGE_TYPE_LABELS, STATUS_LABELS, SmsReport} from '@/communications/models/communication-models';
import {MessageService} from '@/communications/services/communication-services';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {escapeHtml, printInFrame} from '@/shared/utils/print-frame';
import {downloadCsv} from '@/core/utils/csv-export';
import {apiError} from '../communication-settings/communication-settings.component';

/**
 * SMS sent in a month, for billing: messages accepted by the gateway, counted
 * in SMS parts (what the gateway charges). Test-mode messages are left out.
 */
@Component({
    selector: 'app-sms-report',
    templateUrl: './sms-report.component.html',
    providers: [DatePipe, DecimalPipe]
})
export class SmsReportComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/communications/sms-report'], title: 'SMS Report'}
    ];
    dashboardTitle = 'Communications: Monthly SMS Report';

    typeLabels = MESSAGE_TYPE_LABELS;
    statusLabels = STATUS_LABELS;
    months = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
    years: number[] = [];

    year: number;
    month: number;
    report: SmsReport | null = null;
    showDetails = false;
    detailPage = 1;
    detailPageSize = 20;
    schoolName = '';

    constructor(private toastr: ToastrService, private svc: MessageService, private schoolSvc: SchoolDetailsService,
                private datePipe: DatePipe, private decimalPipe: DecimalPipe) {
        let now = new Date();
        this.year = now.getFullYear();
        this.month = now.getMonth() + 1;
        for (let y = this.year; y >= this.year - 4; y--) this.years.push(y);
    }

    ngOnInit(): void {
        this.schoolSvc.get('/schoolDetails').subscribe({next: (r: any) => this.schoolName = (r?.[0] || r)?.name || ''});
        this.load();
    }

    get monthLabel() { return `${this.months[this.month - 1]} ${this.year}`; }

    load() {
        this.svc.smsReport(this.year, this.month, true).subscribe({
            next: (r) => { this.report = r; this.detailPage = 1; },
            error: (err) => this.toastr.error(apiError(err))
        });
    }

    exportCsv() {
        if (!this.report) return;
        downloadCsv(`SMS report ${this.monthLabel}`, [
            {header: 'Sent', value: (m) => this.datePipe.transform(m.sentAt, 'yyyy-MM-dd HH:mm')},
            {header: 'Type', value: (m) => this.typeLabels[m.messageTypeCode] || m.messageTypeCode},
            {header: 'Send', value: (m) => m.batchTitle},
            {header: 'Recipient', value: (m) => m.recipientName},
            {header: 'Phone', value: (m) => m.destination},
            {header: 'Status', value: (m) => this.statusLabels[m.status]},
            {header: 'SMS parts', value: (m) => m.smsParts},
            {header: 'Sent by', value: (m) => m.createdBy}
        ], this.report.details);
    }

    print() {
        let r = this.report;
        if (!r) return;
        let n = (v: number, d = '1.0-0') => this.decimalPipe.transform(v, d) || '0';
        let rows = (list: {label: string; messages: number; parts: number}[], label: (s: string) => string) =>
            list.map((x) => `<tr><td>${escapeHtml(label(x.label))}</td><td class="r">${n(x.messages)}</td><td class="r">${n(x.parts)}</td></tr>`).join('');
        let body = `
            <h1>${escapeHtml(this.schoolName)}</h1>
            <h2>SMS report - ${escapeHtml(this.monthLabel)}</h2>
            <table class="sum">
                <tr><th>SMS sent</th><td class="r">${n(r.messages)}</td></tr>
                <tr><th>SMS parts (billable units)</th><td class="r"><b>${n(r.parts)}</b></td></tr>
                ${r.unitPrice ? `<tr><th>Price per part</th><td class="r">KES ${n(r.unitPrice, '1.2-2')}</td></tr>
                <tr><th>Amount</th><td class="r"><b>KES ${n(r.amount, '1.2-2')}</b></td></tr>` : ''}
                <tr><th>Delivered / not delivered / awaiting report</th><td class="r">${n(r.delivered)} / ${n(r.undelivered)} / ${n(r.awaitingReport)}</td></tr>
            </table>
            <h3>By message type</h3>
            <table><tr><th>Type</th><th class="r">SMS</th><th class="r">Parts</th></tr>${rows(r.byMessageType, (s) => this.typeLabels[s] || s)}</table>
            <h3>By day</h3>
            <table><tr><th>Date</th><th class="r">SMS</th><th class="r">Parts</th></tr>${rows(r.byDay, (s) => s)}</table>
            <p class="note">Counts SMS accepted by the gateway in ${escapeHtml(this.monthLabel)} (East Africa Time). Test messages are excluded.
            One part is up to 160 characters; longer messages are billed per 153-character part.</p>`;
        let css = `@page { size: A4 portrait; margin: 15mm; }
            body { font-family: Arial, Helvetica, sans-serif; font-size: 11px; color: #222; }
            h1 { font-size: 16px; margin: 0; } h2 { font-size: 13px; margin: 2px 0 10px; font-weight: normal; }
            h3 { font-size: 12px; margin: 14px 0 4px; }
            table { border-collapse: collapse; width: 100%; } th, td { border: 1px solid #999; padding: 3px 6px; text-align: left; }
            table.sum { width: 60%; } .r { text-align: right; } .note { color: #666; margin-top: 14px; }`;
        printInFrame(`SMS report ${this.monthLabel}`, css, body);
    }
}
