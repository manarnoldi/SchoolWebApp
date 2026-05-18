import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {formatDate} from '@angular/common';
import {Sponsor, SponsorPayment} from '@/finance/models/sponsorship';
import {Account, AccountType} from '@/finance/models/account';
import {PaymentMethod} from '@/finance/models/finance-models';
import {AccountService, SponsorService, SponsorPaymentService} from '@/finance/services/finance-services';

@Component({
    selector: 'app-finance-sponsor-payments',
    templateUrl: './sponsor-payments.component.html'
})
export class SponsorPaymentsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/sponsor-payments'], title: 'Sponsor Payments'}
    ];
    dashboardTitle = 'Finance: Sponsor Payments';

    payments: SponsorPayment[] = [];
    sponsors: Sponsor[] = [];
    bankAccounts: Account[] = [];

    item: SponsorPayment = {
        sponsorId: 0,
        paymentDate: formatDate(new Date(), 'yyyy-MM-dd', 'en'),
        amount: 0,
        paymentMethod: PaymentMethod.BankTransfer
    };
    showForm = false;

    filterSponsorId: any = null;
    filterDateFrom: string = '';
    filterDateTo: string = '';

    page = 1;
    pageSize = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    paymentMethods = [
        {value: PaymentMethod.Cash, label: 'Cash'},
        {value: PaymentMethod.Mpesa, label: 'M-Pesa'},
        {value: PaymentMethod.BankTransfer, label: 'Bank Transfer'},
        {value: PaymentMethod.Cheque, label: 'Cheque'},
        {value: PaymentMethod.CardPayment, label: 'Card'},
        {value: PaymentMethod.Other, label: 'Other'}
    ];

    constructor(
        private toastr: ToastrService,
        private svc: SponsorPaymentService,
        private sponsorSvc: SponsorService,
        private accSvc: AccountService
    ) {}

    ngOnInit(): void {
        let now = new Date();
        this.filterDateFrom = formatDate(new Date(now.getFullYear(), now.getMonth(), 1), 'yyyy-MM-dd', 'en');
        this.filterDateTo = formatDate(now, 'yyyy-MM-dd', 'en');
        forkJoin([this.svc.getAll(), this.sponsorSvc.getAll(), this.accSvc.get('/accounts')]).subscribe({
            next: ([payments, sponsors, accounts]: any) => {
                this.payments = payments || [];
                this.sponsors = (sponsors || []).filter((s: Sponsor) => s.isActive);
                this.bankAccounts = (accounts || []).filter((a: Account) => a.accountType === AccountType.Asset)
                    .sort((a: any, b: any) => (a.code || '').localeCompare(b.code || ''));
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading.')
        });
    }

    load = () => {
        this.svc.getAll().subscribe({
            next: (p) => this.payments = p || [],
            error: () => {}
        });
    };

    blank = (): SponsorPayment => ({
        sponsorId: 0,
        paymentDate: formatDate(new Date(), 'yyyy-MM-dd', 'en'),
        amount: 0,
        paymentMethod: PaymentMethod.BankTransfer
    });

    filtered = (): SponsorPayment[] =>
        this.payments.filter(p => {
            if (this.filterSponsorId && p.sponsorId != this.filterSponsorId) return false;
            if (this.filterDateFrom && new Date(p.paymentDate) < new Date(this.filterDateFrom)) return false;
            if (this.filterDateTo && new Date(p.paymentDate) > new Date(this.filterDateTo + 'T23:59:59')) return false;
            return true;
        });

    getFilteredTotal = (): number => this.filtered().reduce((s, p) => s + (+p.amount || 0), 0);

    clearFilters = () => {
        let now = new Date();
        this.filterDateFrom = formatDate(new Date(now.getFullYear(), now.getMonth(), 1), 'yyyy-MM-dd', 'en');
        this.filterDateTo = formatDate(now, 'yyyy-MM-dd', 'en');
        this.filterSponsorId = null;
    };

    addNew = () => { this.item = this.blank(); this.showForm = true; };
    cancel = () => { this.showForm = false; };

    save = () => {
        if (!this.item.sponsorId) { this.toastr.info('Select a sponsor.'); return; }
        if (!this.item.paymentDate) { this.toastr.info('Payment date is required.'); return; }
        if (!this.item.amount || +this.item.amount <= 0) { this.toastr.info('Amount must be positive.'); return; }

        this.svc.create(this.item).subscribe({
            next: () => {
                this.toastr.success('Sponsor payment recorded and posted.');
                this.showForm = false;
                this.load();
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error saving.')
        });
    };

    delete = (p: SponsorPayment) => {
        Swal.fire({
            title: 'Delete sponsor payment?',
            text: 'The auto-posted journal entry will also be removed.',
            icon: 'warning', showCancelButton: true,
            width: 420, position: 'top', padding: '1em',
            confirmButtonColor: '#d33', confirmButtonText: 'Delete'
        }).then(r => {
            if (!r.value) return;
            this.svc.delete(+p.id!).subscribe({
                next: () => { this.toastr.success('Deleted.'); this.load(); },
                error: (err) => this.toastr.error(err.error?.message || 'Error deleting.')
            });
        });
    };

    getMethodLabel = (m: any): string => this.paymentMethods.find(x => x.value === m)?.label || '';
}
