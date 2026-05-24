import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {Log} from '../../models/log';
import {LogsService} from '../../services/logs.service';

declare const bootstrap: any;

@Component({
    selector: 'app-logs',
    templateUrl: './logs.component.html',
    styleUrls: ['./logs.component.scss']
})
export class LogsComponent implements OnInit {
    // Templates can't reference globals like Math directly.
    readonly Math = Math;
    dashboardTitle = 'Security: Error logs';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/security/logs'], title: 'Security: Error logs'}
    ];

    filterForm: FormGroup;
    levels: string[] = [];
    logs: Log[] = [];
    total = 0;
    page = 1;
    pageSize = 20;
    loading = false;

    // For each log id, how many other rows in the same page share the same
    // (logger, normalized-message) — i.e. the burst this entry belongs to.
    // Used to surface that an error didn't happen once but N times together,
    // so triage can focus on the underlying event rather than each row.
    rowDuplicateCount = new Map<number, number>();

    selected: Log | null = null;
    resolutionNote = '';
    @ViewChild('logDetailsModal') logDetailsModal: ElementRef<HTMLElement>;

    constructor(
        private logsSvc: LogsService,
        private toastr: ToastrService,
        private fb: FormBuilder
    ) {}

    ngOnInit(): void {
        // Default the date window to today only — keeps the initial view
        // focused on what's happening right now. Users can widen the range
        // via the From/To inputs or clear it entirely with the Clear button.
        let today = this.iso(new Date());

        this.filterForm = this.fb.group({
            level: ['Error'],
            search: [''],
            startDate: [today],
            endDate: [today],
            // 'open' (default) | 'resolved' | 'all'
            status: ['open']
        });

        this.logsSvc.levels().subscribe({
            next: (res) => (this.levels = res ?? []),
            error: () => {} // non-fatal; the input still works
        });

        this.load();
    }

    load = () => {
        this.loading = true;
        let v = this.filterForm.value;
        // 'all' => omit the param; 'open'/'resolved' => boolean.
        let resolvedParam =
            v.status === 'resolved' ? true : v.status === 'open' ? false : null;
        this.logsSvc
            .list({
                level: v.level,
                search: v.search,
                startDate: v.startDate,
                endDate: v.endDate,
                resolved: resolvedParam,
                page: this.page,
                pageSize: this.pageSize
            })
            .subscribe({
                next: (res) => {
                    this.logs = res.items ?? [];
                    this.total = res.total ?? 0;
                    this.computeBurstCounts();
                    this.loading = false;
                },
                error: (err) => {
                    this.toastr.error(err?.error?.message || err?.error || 'Failed to load application logs');
                    this.loading = false;
                }
            });
    };

    /**
     * Walks the current page of logs and counts how many share the same
     * (logger, normalized message). Errors usually arrive in clusters — a
     * single bad request fires the same .LogError(...) call multiple times
     * as it bubbles through layers, or a hosted job retries and each retry
     * fails identically. Showing the cluster size in the row helps the
     * operator see at a glance that 12 rows are actually 1 incident.
     *
     * Message is normalized to the first 200 characters so trivially-varying
     * suffixes (timestamps, ids) don't split otherwise-identical entries.
     */
    private computeBurstCounts() {
        let key = (l: Log) =>
            `${l.logger || ''}|${(l.message || '').substring(0, 200)}`;
        let counts = new Map<string, number>();
        for (let log of this.logs) {
            let k = key(log);
            counts.set(k, (counts.get(k) || 0) + 1);
        }
        this.rowDuplicateCount = new Map<number, number>();
        for (let log of this.logs) {
            this.rowDuplicateCount.set(log.id, counts.get(key(log)) || 1);
        }
    }

    burstCountFor = (log: Log): number => this.rowDuplicateCount.get(log.id) || 1;

    applyFilters = () => {
        this.page = 1;
        this.load();
    };

    clearFilters = () => {
        this.filterForm.reset({
            level: '',
            search: '',
            startDate: '',
            endDate: '',
            status: 'open'
        });
        this.page = 1;
        this.load();
    };

    toggleResolved = (log: Log) => {
        let target = !log.resolved;
        let note = target ? (this.resolutionNote || '').trim() : '';
        this.logsSvc.setResolution(log.id, target, note).subscribe({
            next: (updated) => {
                // Reflect the change in the currently-opened modal and in
                // the list row so the user sees the new state right away.
                this.selected = updated;
                let idx = this.logs.findIndex((l) => l.id === updated.id);
                if (idx >= 0) this.logs[idx] = updated;
                this.toastr.success(target ? 'Log marked as resolved' : 'Log reopened');
                this.resolutionNote = '';
            },
            error: (err) => {
                this.toastr.error(err?.error?.message || err?.error || 'Failed to update log resolution status');
            }
        });
    };

    openDetails = (log: Log) => {
        this.selected = log;
        this.resolutionNote = '';
        if (this.logDetailsModal?.nativeElement) {
            bootstrap.Modal.getOrCreateInstance(this.logDetailsModal.nativeElement).show();
        }
    };

    // Wired to the shared <app-table-paging> component so the logs page
    // matches the navigation style used elsewhere in the app.
    pageChanged = (p: number) => {
        if (p === this.page) return;
        this.page = p;
        this.load();
    };

    pageSizeChanged = (s: number) => {
        if (s === this.pageSize) return;
        this.pageSize = s;
        this.page = 1; // resetting avoids landing past the new last page
        this.load();
    };

    get pageStart(): number {
        return this.total === 0 ? 0 : (this.page - 1) * this.pageSize + 1;
    }
    get pageEnd(): number {
        return Math.min(this.page * this.pageSize, this.total);
    }

    // Stable class for the level chip so the Error rows pop visually and you
    // can scan a noisy table quickly.
    levelClass = (level: string): string => {
        switch ((level ?? '').toLowerCase()) {
            case 'fatal': return 'bg-danger text-white';
            case 'error': return 'bg-danger-subtle text-danger-emphasis';
            case 'warn': return 'bg-warning-subtle text-warning-emphasis';
            case 'info': return 'bg-info-subtle text-info-emphasis';
            default: return 'bg-secondary-subtle text-secondary-emphasis';
        }
    };

    // Drives the modal header colour and the Message/Exception card accents
    // so the severity is felt at a glance, not just read.
    headerBgClass = (level: string): string => {
        switch ((level ?? '').toLowerCase()) {
            case 'fatal':
            case 'error': return 'bg-danger';
            case 'warn': return 'bg-warning';
            case 'info': return 'bg-info';
            default: return 'bg-secondary';
        }
    };

    messageBorderClass = (level: string): string => {
        switch ((level ?? '').toLowerCase()) {
            case 'fatal':
            case 'error': return 'border-danger';
            case 'warn': return 'border-warning';
            case 'info': return 'border-info';
            default: return 'border-secondary';
        }
    };

    trackById = (_: number, item: Log) => item.id;

    private iso(d: Date): string {
        let m = String(d.getMonth() + 1).padStart(2, '0');
        let day = String(d.getDate()).padStart(2, '0');
        return `${d.getFullYear()}-${m}-${day}`;
    }
}
