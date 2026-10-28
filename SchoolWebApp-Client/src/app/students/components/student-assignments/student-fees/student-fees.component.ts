import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {forkJoin} from 'rxjs';
import {ProfilesService} from '@/core/services/profiles.service';
import {printInFrame, removePrintFrame} from '@/shared/utils/print-frame';
import {FEE_PRINT_CSS, FeeSchool, FeeStudent, feeStructureDoc, invoiceDoc, receiptDoc, statementDoc} from '@/shared/utils/fee-documents';

/**
 * Student page, Fees tab: what has been invoiced and paid, the balance and the
 * statement lines, the invoices and receipts - and prints: the fee statement,
 * each invoice and receipt, and the fee structure for the student's grade.
 */
@Component({
    selector: 'app-student-fees',
    template: `
        <div *ngIf="loading" class="text-muted p-2"><i class="fas fa-spinner fa-spin"></i> Loading the fee statement...</div>
        <ng-container *ngIf="s">
            <div class="d-flex justify-content-end mb-2">
                <div class="dropdown">
                    <button class="btn btn-sm btn-outline-primary dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">
                        <i class="fas fa-print"></i> Print</button>
                    <ul class="dropdown-menu dropdown-menu-end">
                        <li><a class="dropdown-item" href="javascript:void(0)" (click)="printStatement()">
                            <i class="fas fa-file-invoice-dollar text-primary me-2"></i>Fee statement</a></li>
                        <li *ngFor="let f of fees?.feeStructures"><a class="dropdown-item" href="javascript:void(0)" (click)="printStructure(f)">
                            <i class="fas fa-list-ol text-primary me-2"></i>Fee structure - {{ f.session || 'whole year' }} {{ f.year }}</a></li>
                        <li *ngIf="fees?.invoices?.length"><a class="dropdown-item" href="javascript:void(0)" (click)="printInvoices(fees.invoices)">
                            <i class="fas fa-file-invoice text-primary me-2"></i>All invoices ({{ fees.invoices.length }})</a></li>
                    </ul>
                </div>
            </div>

            <div class="row g-2 mb-2">
                <div class="col-md-4"><div class="border rounded p-2 text-center">
                    <div class="text-muted small">Invoiced</div><div class="fs-5 fw-bold">{{ s.totalInvoiced | number:'1.2-2' }}</div></div></div>
                <div class="col-md-4"><div class="border rounded p-2 text-center">
                    <div class="text-muted small">Paid</div><div class="fs-5 fw-bold text-success">{{ s.totalPaid | number:'1.2-2' }}</div></div></div>
                <div class="col-md-4"><div class="border rounded p-2 text-center" [ngClass]="s.balance > 0 ? 'border-danger' : 'border-success'">
                    <div class="text-muted small">{{ s.balance > 0 ? 'Balance due' : (s.balance < 0 ? 'In credit' : 'Balance') }}</div>
                    <div class="fs-5 fw-bold" [ngClass]="s.balance > 0 ? 'text-danger' : 'text-success'">{{ abs(s.balance) | number:'1.2-2' }}</div></div></div>
            </div>

            <div class="row g-2 mb-2" *ngIf="fees">
                <!-- Invoices -->
                <div class="col-lg-6">
                    <div class="card mb-0 h-100">
                        <div class="card-header py-1"><b><i class="fas fa-file-invoice text-primary me-1"></i>Invoices</b></div>
                        <table class="table table-sm mb-0 small align-middle">
                            <tbody>
                                <tr *ngIf="fees.invoices.length === 0"><td class="text-muted">No invoices.</td></tr>
                                <tr *ngFor="let i of fees.invoices">
                                    <td><b>{{ i.number }}</b><div class="text-muted">{{ i.session }} {{ i.year }} - {{ i.date | date:'d MMM y' }}</div></td>
                                    <td class="text-end">{{ i.total | number:'1.2-2' }}<div [ngClass]="i.balance > 0 ? 'text-danger' : 'text-success'">
                                        {{ i.balance > 0 ? (i.balance | number:'1.2-2') + ' due' : 'Paid' }}</div></td>
                                    <td class="text-end" style="width: 1%;"><button class="btn btn-sm btn-outline-primary py-0" title="Print this invoice"
                                        (click)="printInvoices([i])"><i class="fas fa-print"></i></button></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
                <!-- Receipts -->
                <div class="col-lg-6">
                    <div class="card mb-0 h-100">
                        <div class="card-header py-1"><b><i class="fas fa-receipt text-success me-1"></i>Receipts</b></div>
                        <table class="table table-sm mb-0 small align-middle">
                            <tbody>
                                <tr *ngIf="fees.receipts.length === 0"><td class="text-muted">No payments.</td></tr>
                                <tr *ngFor="let r of fees.receipts">
                                    <td><b>{{ r.number }}</b><div class="text-muted">{{ r.date | date:'d MMM y' }} - {{ r.method }}<span *ngIf="r.reference"> {{ r.reference }}</span></div></td>
                                    <td class="text-end text-success fw-bold">{{ r.amount | number:'1.2-2' }}</td>
                                    <td class="text-end" style="width: 1%;"><button class="btn btn-sm btn-outline-success py-0" title="Print this receipt"
                                        (click)="printReceipt(r)"><i class="fas fa-print"></i></button></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <div class="card mb-0">
                <div class="card-header py-1"><b><i class="fas fa-file-invoice-dollar text-primary me-1"></i>Statement</b></div>
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
            </div>
        </ng-container>`
})
export class StudentFeesComponent implements OnInit, OnDestroy {
    @Input() studentId!: number;
    s: any = null;
    fees: any = null;
    school: FeeSchool = {};
    loading = false;
    abs = Math.abs;

    constructor(private svc: ProfilesService, private http: HttpClient) {}

    ngOnInit(): void {
        this.loading = true;
        forkJoin([
            this.svc.studentStatement(this.studentId),
            this.http.get<any>(`/profiles/students/${this.studentId}/fees`),
            this.http.get<any[]>('/schoolDetails')
        ]).subscribe({
            next: ([statement, fees, school]) => {
                this.loading = false;
                this.s = statement;
                this.fees = fees;
                this.school = (school || [])[0] || {};
            },
            error: () => (this.loading = false)
        });
    }

    ngOnDestroy(): void {
        removePrintFrame();
    }

    private get student(): FeeStudent {
        const st = this.fees?.student || {};
        return {name: st.name || this.s?.student?.fullName || '', upi: st.upi || this.s?.student?.upi || '', className: st.className, year: st.year};
    }

    printStatement() {
        printInFrame(`Fee statement - ${this.student.name}`, FEE_PRINT_CSS, statementDoc(this.school, this.student, this.s));
    }

    printInvoices(list: any[]) {
        printInFrame(list.length === 1 ? `Invoice ${list[0].number}` : `Invoices - ${this.student.name}`, FEE_PRINT_CSS,
            list.map((i) => invoiceDoc(this.school, this.student, i)).join(''));
    }

    printReceipt(r: any) {
        printInFrame(`Receipt ${r.number}`, FEE_PRINT_CSS, receiptDoc(this.school, this.student, r, this.s?.balance ?? null));
    }

    printStructure(f: any) {
        printInFrame(`Fee structure - ${f.name}`, FEE_PRINT_CSS, feeStructureDoc(this.school, this.student, f));
    }
}
