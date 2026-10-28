import {Component, Input, OnInit} from '@angular/core';
import {ProfilesService} from '@/core/services/profiles.service';

/** Student page, Fees tab: what has been invoiced and paid, the balance, and the statement lines. */
@Component({
    selector: 'app-student-fees',
    template: `
        <div *ngIf="loading" class="text-muted p-2"><i class="fas fa-spinner fa-spin"></i> Loading the fee statement...</div>
        <ng-container *ngIf="s">
            <div class="row g-2 mb-2">
                <div class="col-md-4"><div class="border rounded p-2 text-center">
                    <div class="text-muted small">Invoiced</div><div class="fs-5 fw-bold">{{ s.totalInvoiced | number:'1.2-2' }}</div></div></div>
                <div class="col-md-4"><div class="border rounded p-2 text-center">
                    <div class="text-muted small">Paid</div><div class="fs-5 fw-bold text-success">{{ s.totalPaid | number:'1.2-2' }}</div></div></div>
                <div class="col-md-4"><div class="border rounded p-2 text-center" [ngClass]="s.balance > 0 ? 'border-danger' : 'border-success'">
                    <div class="text-muted small">{{ s.balance > 0 ? 'Balance due' : (s.balance < 0 ? 'In credit' : 'Balance') }}</div>
                    <div class="fs-5 fw-bold" [ngClass]="s.balance > 0 ? 'text-danger' : 'text-success'">{{ abs(s.balance) | number:'1.2-2' }}</div></div></div>
            </div>
            <div class="table-responsive">
                <table class="table table-sm table-striped mb-0 align-middle small">
                    <thead class="table-light"><tr><th>Date</th><th>Type</th><th>Reference</th><th>Description</th>
                        <th class="text-end">Debit</th><th class="text-end">Credit</th><th class="text-end">Balance</th></tr></thead>
                    <tbody>
                        <tr *ngIf="s.transactions.length === 0"><td colspan="7" class="text-center text-muted py-2">No invoices or payments yet.</td></tr>
                        <tr *ngFor="let t of s.transactions">
                            <td class="text-nowrap">{{ t.date | date:'d MMM y' }}</td>
                            <td>{{ t.type }}<span class="text-muted" *ngIf="t.method"> ({{ t.method }})</span></td>
                            <td>{{ t.reference }}</td>
                            <td>{{ t.description }}</td>
                            <td class="text-end">{{ t.debit ? (t.debit | number:'1.2-2') : '' }}</td>
                            <td class="text-end">{{ t.credit ? (t.credit | number:'1.2-2') : '' }}</td>
                            <td class="text-end fw-bold">{{ t.balance | number:'1.2-2' }}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </ng-container>`
})
export class StudentFeesComponent implements OnInit {
    @Input() studentId!: number;
    s: any = null;
    loading = false;
    abs = Math.abs;

    constructor(private svc: ProfilesService) {}

    ngOnInit(): void {
        this.loading = true;
        this.svc.studentStatement(this.studentId).subscribe({
            next: (s) => {
                this.loading = false;
                this.s = s;
            },
            error: () => (this.loading = false)
        });
    }
}
