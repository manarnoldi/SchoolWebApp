import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AuditLog} from '../../models/audit-log';
import {AuditLogsService} from '../../services/audit-logs.service';

declare const bootstrap: any;

@Component({
    selector: 'app-audit-logs',
    templateUrl: './audit-logs.component.html',
    styleUrls: ['./audit-logs.component.scss']
})
export class AuditLogsComponent implements OnInit {
    // Templates can't reference globals like Math directly.
    readonly Math = Math;
    dashboardTitle = 'Security: Audit trail';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/security/audit-logs'], title: 'Security: Audit trail'}
    ];

    filterForm: FormGroup;
    actions: string[] = [];
    entityTypes: string[] = [];
    logs: AuditLog[] = [];
    total = 0;
    page = 1;
    pageSize = 20;
    loading = false;

    selected: AuditLog | null = null;
    selectedOld: any = null;
    selectedNew: any = null;

    @ViewChild('auditDetailsModal') auditDetailsModal: ElementRef<HTMLElement>;

    constructor(
        private svc: AuditLogsService,
        private toastr: ToastrService,
        private fb: FormBuilder
    ) {}

    ngOnInit(): void {
        // Default the date window to today only — operators reviewing
        // activity want "what happened today" by default. They can widen
        // the range via the From/To inputs or clear it entirely.
        let today = this.iso(new Date());

        this.filterForm = this.fb.group({
            userName: [''],
            action: [''],
            entityType: [''],
            search: [''],
            startDate: [today],
            endDate: [today]
        });

        // Distinct action verbs + entity types power the two filter
        // dropdowns. Both are best-effort — if they fail the input
        // controls still accept free text.
        this.svc.actions().subscribe({
            next: (res) => (this.actions = res ?? []),
            error: () => {}
        });
        this.svc.entityTypes().subscribe({
            next: (res) => (this.entityTypes = res ?? []),
            error: () => {}
        });

        this.load();
    }

    load = () => {
        this.loading = true;
        let v = this.filterForm.value;
        this.svc
            .list({
                userName: v.userName,
                action: v.action,
                entityType: v.entityType,
                search: v.search,
                startDate: v.startDate,
                endDate: v.endDate,
                page: this.page,
                pageSize: this.pageSize
            })
            .subscribe({
                next: (res) => {
                    this.logs = res.items ?? [];
                    this.total = res.total ?? 0;
                    this.loading = false;
                },
                error: (err) => {
                    this.toastr.error(
                        err?.error?.message ||
                            err?.error ||
                            'Failed to load audit trail'
                    );
                    this.loading = false;
                }
            });
    };

    applyFilters = () => {
        this.page = 1;
        this.load();
    };

    clearFilters = () => {
        this.filterForm.reset({
            userName: '',
            action: '',
            entityType: '',
            search: '',
            startDate: '',
            endDate: ''
        });
        this.page = 1;
        this.load();
    };

    openDetails = (row: AuditLog) => {
        this.selected = row;
        // Parse the JSON payloads up front so the template can pretty-
        // print them without smearing JSON.parse calls across bindings.
        // Bad/empty values just become null so the template can hide
        // the block.
        this.selectedOld = this.tryParse(row.oldValues);
        this.selectedNew = this.tryParse(row.newValues);
        if (this.auditDetailsModal?.nativeElement) {
            bootstrap.Modal.getOrCreateInstance(
                this.auditDetailsModal.nativeElement
            ).show();
        }
    };

    // Wired to the shared <app-table-paging> component so the audit
    // page matches the navigation style used elsewhere in the app.
    pageChanged = (p: number) => {
        if (p === this.page) return;
        this.page = p;
        this.load();
    };

    pageSizeChanged = (s: number) => {
        if (s === this.pageSize) return;
        this.pageSize = s;
        this.page = 1;
        this.load();
    };

    get pageStart(): number {
        return this.total === 0 ? 0 : (this.page - 1) * this.pageSize + 1;
    }
    get pageEnd(): number {
        return Math.min(this.page * this.pageSize, this.total);
    }

    actionClass = (action: string): string => {
        switch ((action ?? '').toLowerCase()) {
            case 'create':
                return 'bg-success-subtle text-success-emphasis';
            case 'update':
                return 'bg-info-subtle text-info-emphasis';
            case 'delete':
                return 'bg-danger-subtle text-danger-emphasis';
            case 'login':
                return 'bg-primary-subtle text-primary-emphasis';
            case 'loginfailed':
                return 'bg-danger text-white';
            case 'logout':
                return 'bg-secondary-subtle text-secondary-emphasis';
            case 'view':
            case 'print':
                return 'bg-warning-subtle text-warning-emphasis';
            default:
                return 'bg-secondary-subtle text-secondary-emphasis';
        }
    };

    asJson(value: any): string {
        if (value == null) return '';
        try {
            return JSON.stringify(value, null, 2);
        } catch {
            return String(value);
        }
    }

    trackById = (_: number, item: AuditLog) => item.id;

    private tryParse(raw: string | null): any {
        if (!raw) return null;
        try {
            return JSON.parse(raw);
        } catch {
            return raw;
        }
    }

    private iso(d: Date): string {
        let m = String(d.getMonth() + 1).padStart(2, '0');
        let day = String(d.getDate()).padStart(2, '0');
        return `${d.getFullYear()}-${m}-${day}`;
    }
}
