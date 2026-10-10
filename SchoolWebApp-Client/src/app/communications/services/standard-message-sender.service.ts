import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {ComposePreview, QueueResult} from '../models/communication-models';
import {escapeHtml} from '@/shared/utils/print-frame';

export type StandardMessageKind = 'examResults' | 'feeInvoices' | 'feeBalances' | 'feePayments';

const KIND_LABELS: {[k in StandardMessageKind]: string} = {
    examResults: 'exam results',
    feeInvoices: 'invoice',
    feeBalances: 'fee balance reminder',
    feePayments: 'payment acknowledgement'
};

/**
 * "Send to parents" from the results, invoice, balance and payment screens:
 * previews the message type's template for the learners given, shows what
 * will go out - counts, SMS parts, the first message as rendered - and queues
 * it once confirmed. The template and its channel come from
 * Communications > Templates.
 */
@Injectable({providedIn: 'root'})
export class StandardMessageSender {
    constructor(private http: HttpClient, private toastr: ToastrService, private router: Router) {}

    send(kind: StandardMessageKind, body: any): void {
        this.http.post<ComposePreview>(`/messages/${kind}`, {...body, preview: true}).subscribe({
            next: (p) => this.confirm(kind, body, p),
            error: (err) => this.toastr.error(this.error(err))
        });
    }

    private confirm(kind: StandardMessageKind, body: any, p: ComposePreview) {
        let parts: string[] = [];
        if (p.smsCount) parts.push(`<b>${p.smsCount}</b> SMS (${p.smsParts} SMS part${p.smsParts === 1 ? '' : 's'})`);
        if (p.emailCount) parts.push(`<b>${p.emailCount}</b> email${p.emailCount === 1 ? '' : 's'}`);
        if (!parts.length) {
            Swal.fire({
                title: 'Nobody to send to',
                text: 'None of the parents has a phone number or email for this message type\'s channel.',
                icon: 'info'
            });
            return;
        }

        let html = `<div class="text-start small">Send the ${KIND_LABELS[kind]} to parents: ${parts.join(' and ')}.`;
        if (p.missingContact) html += `<br><span class="text-danger">${p.missingContact} learner(s) have no parent contact and will be skipped.</span>`;
        let sample = p.sampleSms || p.sampleEmailBody;
        if (sample) html += `<div class="mt-2 text-muted">The first message reads:</div>
            <div class="border rounded p-2 bg-light" style="white-space: pre-wrap;">${escapeHtml(sample)}</div>`;
        if (p.testMode) html += '<div class="mt-2 text-warning"><b>Test mode is on</b> - no real parent will receive it.</div>';
        html += '</div>';

        Swal.fire({
            title: 'Send to parents?',
            html,
            icon: 'question',
            width: 600,
            showCancelButton: true,
            confirmButtonText: 'Send'
        }).then((r) => {
            if (!r.isConfirmed) return;
            this.http.post<QueueResult>(`/messages/${kind}`, {...body, preview: false}).subscribe({
                next: (q) => {
                    let t = this.toastr.success(
                        `${q.queued} message(s) queued${q.testMode ? ' (test mode)' : ''}. Click to open the message queue.`);
                    t.onTap.subscribe(() => this.router.navigate(['/communications/queue'], {queryParams: {batchId: q.batchId}}));
                },
                error: (err) => this.toastr.error(this.error(err))
            });
        });
    }

    private error(err: any): string {
        let e = err?.error;
        if (err?.status === 403) return 'You are not allowed to send messages.';
        return (typeof e === 'string' && e) ? e : (e?.message || 'The message could not be sent.');
    }
}
