import {Component, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {
    CHANNEL_LABELS,
    MESSAGE_TYPE_LABELS,
    MessageBatch,
    MessageChannel,
    MessageStatus,
    OutboundMessage,
    RECIPIENT_TYPE_LABELS,
    STATUS_BADGES,
    STATUS_LABELS
} from '@/communications/models/communication-models';
import {MessageService} from '@/communications/services/communication-services';
import {apiError} from '../communication-settings/communication-settings.component';

/**
 * The message queue: each send (batch) with its progress, and every message
 * with its status, gateway reply and error. Refreshes itself while anything is
 * still waiting to go.
 */
@Component({
    selector: 'app-message-queue',
    templateUrl: './message-queue.component.html'
})
export class MessageQueueComponent implements OnInit, OnDestroy {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/communications/queue'], title: 'Message Queue'}
    ];
    dashboardTitle = 'Communications: Message Queue';

    statusLabels = STATUS_LABELS;
    statusBadges = STATUS_BADGES;
    channelLabels = CHANNEL_LABELS;
    typeLabels = MESSAGE_TYPE_LABELS;
    recipientTypeLabels = RECIPIENT_TYPE_LABELS;
    MessageStatus = MessageStatus;
    MessageChannel = MessageChannel;
    statuses = [MessageStatus.Queued, MessageStatus.Sending, MessageStatus.Sent, MessageStatus.Delivered,
        MessageStatus.Undelivered, MessageStatus.Failed, MessageStatus.Cancelled];
    typeCodes = Object.keys(MESSAGE_TYPE_LABELS);

    tab: 'batches' | 'messages' = 'batches';

    batches: MessageBatch[] = [];
    batchTotal = 0;
    batchPage = 1;
    batchPageSize = 10;

    messages: OutboundMessage[] = [];
    messageTotal = 0;
    statusCounts: {[k: number]: number} = {};
    messagePage = 1;
    messagePageSize = 20;

    filters: {status: MessageStatus | null; channel: MessageChannel | null; messageTypeCode: string | null;
        batchId: number | null; from: string | null; to: string | null; search: string} = {
        status: null, channel: null, messageTypeCode: null, batchId: null, from: null, to: null, search: ''
    };

    selected: OutboundMessage | null = null;
    private timer: any;

    constructor(private toastr: ToastrService, private route: ActivatedRoute, private svc: MessageService) {}

    ngOnInit(): void {
        let batchId = Number(this.route.snapshot.queryParamMap.get('batchId'));
        if (batchId) {
            this.filters.batchId = batchId;
            this.tab = 'messages';
        }
        this.refresh();
        this.timer = setInterval(() => this.autoRefresh(), 15000);
    }

    ngOnDestroy(): void {
        clearInterval(this.timer);
    }

    private get hasPending(): boolean {
        return this.tab === 'batches'
            ? this.batches.some((b) => b.queued > 0)
            : (this.statusCounts[MessageStatus.Queued] || 0) + (this.statusCounts[MessageStatus.Sending] || 0) > 0;
    }

    private autoRefresh() {
        if (this.hasPending && !this.selected) this.refresh();
    }

    refresh() {
        if (this.tab === 'batches') this.loadBatches();
        else this.loadMessages();
    }

    setTab(t: 'batches' | 'messages') {
        this.tab = t;
        this.refresh();
    }

    loadBatches() {
        this.svc.batches({pageNumber: this.batchPage, pageSize: this.batchPageSize}).subscribe({
            next: (r) => { this.batches = r.data; this.batchTotal = r.totalCount; },
            error: (err) => this.toastr.error(apiError(err))
        });
    }

    loadMessages() {
        this.svc.queue({...this.filters, pageNumber: this.messagePage, pageSize: this.messagePageSize}).subscribe({
            next: (r) => {
                this.messages = r.data;
                this.messageTotal = r.totalCount;
                this.statusCounts = {};
                r.statusCounts.forEach((c) => this.statusCounts[c.status] = c.count);
            },
            error: (err) => this.toastr.error(apiError(err))
        });
    }

    applyFilters() {
        this.messagePage = 1;
        this.loadMessages();
    }

    clearFilters() {
        this.filters = {status: null, channel: null, messageTypeCode: null, batchId: null, from: null, to: null, search: ''};
        this.applyFilters();
    }

    filterStatus(s: MessageStatus | null) {
        this.filters.status = s;
        this.applyFilters();
    }

    openBatch(b: MessageBatch) {
        this.filters = {status: null, channel: null, messageTypeCode: null, batchId: b.id, from: null, to: null, search: ''};
        this.tab = 'messages';
        this.applyFilters();
    }

    get totalAcrossStatuses(): number {
        return Object.values(this.statusCounts).reduce((a, b) => a + b, 0);
    }

    progress(b: MessageBatch): number {
        return b.totalMessages ? Math.round(100 * (b.totalMessages - b.queued) / b.totalMessages) : 100;
    }

    retry(m: OutboundMessage) {
        this.svc.retry(m.id).subscribe({
            next: () => { this.toastr.success('Message queued again.'); this.selected = null; this.loadMessages(); },
            error: (err) => this.toastr.error(apiError(err))
        });
    }

    cancel(m: OutboundMessage) {
        this.svc.cancel(m.id).subscribe({
            next: () => { this.toastr.success('Message cancelled.'); this.selected = null; this.loadMessages(); },
            error: (err) => this.toastr.error(apiError(err))
        });
    }

    retryBatch(b: MessageBatch) {
        this.svc.retryBatch(b.id).subscribe({
            next: (r) => { this.toastr.success(`${r.requeued} message(s) queued again.`); this.loadBatches(); },
            error: (err) => this.toastr.error(apiError(err))
        });
    }

    cancelBatch(b: MessageBatch) {
        Swal.fire({
            title: 'Cancel the rest of this send?',
            text: `${b.queued} message(s) still waiting will not be sent. Messages already sent are not affected.`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Cancel them',
            cancelButtonText: 'Keep sending'
        }).then((r) => {
            if (!r.isConfirmed) return;
            this.svc.cancelBatch(b.id).subscribe({
                next: (x) => { this.toastr.success(`${x.cancelled} message(s) cancelled.`); this.loadBatches(); },
                error: (err) => this.toastr.error(apiError(err))
            });
        });
    }
}
