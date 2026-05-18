import {Component, OnDestroy, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {ApprovalService} from '@/approvals/services/approval.service';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-notifications',
    templateUrl: './notifications.component.html',
    styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit, OnDestroy {
    pendingApprovals: any[] = [];
    notificationCount: number = 0;
    private refreshSub?: Subscription;

    constructor(
        private approvalSvc: ApprovalService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.loadNotifications();
        this.refreshSub = this.approvalSvc.refresh$.subscribe(() => this.loadNotifications());
    }

    ngOnDestroy(): void {
        this.refreshSub?.unsubscribe();
    }

    loadNotifications() {
        this.approvalSvc.getMyPending().subscribe({
            next: (rows) => {
                this.pendingApprovals = rows || [];
                this.notificationCount = this.pendingApprovals.length;
            },
            error: () => {
                this.pendingApprovals = [];
                this.notificationCount = 0;
            }
        });
    }

    formKeyLabel(key: string): string {
        switch (key) {
            case 'Expense': return 'Expense';
            case 'JournalEntry': return 'Journal Entry';
            case 'CreditDebitNote': return 'Credit / Debit Note';
            case 'BudgetAmendment': return 'Budget Amendment';
            case 'Budget': return 'Budget';
            default: return key;
        }
    }

    iconFor(key: string): string {
        switch (key) {
            case 'Expense': return 'fas fa-money-bill-wave text-danger';
            case 'JournalEntry': return 'fas fa-pen text-primary';
            case 'CreditDebitNote': return 'fas fa-file-invoice text-warning';
            case 'BudgetAmendment': return 'fas fa-edit text-warning';
            case 'Budget': return 'fas fa-piggy-bank text-success';
            default: return 'fas fa-check-double text-secondary';
        }
    }

    private routeFor(key: string): string | null {
        switch (key) {
            case 'Expense': return '/finance/expenses';
            case 'JournalEntry': return '/finance/journal-entries';
            case 'CreditDebitNote': return '/finance/payments';
            case 'BudgetAmendment': return '/finance/budget-amendments';
            case 'Budget': return '/finance/budgets';
            default: return null;
        }
    }

    openTask(item: any) {
        let route = this.routeFor(item.entityType);
        if (!route) return;
        this.router.navigate([route], {queryParams: {actId: item.entityId}});
    }

    getTimeAgo(dateStr: string): string {
        if (!dateStr) return '';
        let now = new Date().getTime();
        let target = new Date(dateStr).getTime();
        let diff = now - target;
        let minutes = Math.floor(diff / 60000);
        let hours = Math.floor(diff / 3600000);
        let days = Math.floor(diff / 86400000);
        if (days > 0) return days + 'd ago';
        if (hours > 0) return hours + 'h ago';
        if (minutes > 0) return minutes + 'm ago';
        return 'just now';
    }
}
