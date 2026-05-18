import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {Sponsor, SponsorType} from '@/finance/models/sponsorship';
import {Account, AccountType} from '@/finance/models/account';
import {AccountService, SponsorService} from '@/finance/services/finance-services';

@Component({
    selector: 'app-finance-sponsors',
    templateUrl: './sponsors.component.html'
})
export class SponsorsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/finance/sponsors'], title: 'Sponsors'}
    ];
    dashboardTitle = 'Finance: Sponsors';

    sponsors: Sponsor[] = [];
    receivableAccounts: Account[] = [];
    item: Sponsor = {name: '', sponsorType: SponsorType.External, isActive: true};
    editMode = false;
    showForm = false;

    searchTerm: string = '';
    page = 1;
    pageSize = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    sponsorTypes = [
        {value: SponsorType.External, label: 'External (NGO / Foundation / Company)'},
        {value: SponsorType.School, label: 'School (Internal Bursary Fund)'},
        {value: SponsorType.Government, label: 'Government'},
        {value: SponsorType.Parent, label: 'Parent'},
        {value: SponsorType.Other, label: 'Other'}
    ];

    // Statement modal
    statement: any = null;
    statementFromDate: string = '';
    statementToDate: string = '';
    statementSponsor: Sponsor | null = null;

    constructor(private toastr: ToastrService, private svc: SponsorService, private accSvc: AccountService) {}

    ngOnInit(): void {
        forkJoin([this.svc.getAll(), this.accSvc.get('/accounts')]).subscribe({
            next: ([sponsors, accounts]) => {
                this.sponsors = sponsors || [];
                this.receivableAccounts = (accounts || []).filter((a: Account) => a.accountType === AccountType.Asset)
                    .sort((a: any, b: any) => (a.code || '').localeCompare(b.code || ''));
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading sponsors.')
        });
    }

    load = () => {
        this.svc.getAll().subscribe({
            next: (s) => this.sponsors = s || [],
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    };

    blank = (): Sponsor => ({
        name: '',
        sponsorType: SponsorType.External,
        isActive: true
    });

    filtered = (): Sponsor[] => {
        let q = (this.searchTerm || '').trim().toLowerCase();
        if (!q) return this.sponsors;
        return this.sponsors.filter(s =>
            (s.name || '').toLowerCase().includes(q) ||
            (s.contactName || '').toLowerCase().includes(q) ||
            (s.email || '').toLowerCase().includes(q));
    };

    typeLabel = (t: any): string =>
        this.sponsorTypes.find(x => x.value === t)?.label || '';

    addNew = () => { this.item = this.blank(); this.editMode = false; this.showForm = true; };
    edit = (s: Sponsor) => { this.item = {...s}; this.editMode = true; this.showForm = true; };
    cancel = () => { this.showForm = false; };

    save = () => {
        if (!this.item.name) { this.toastr.info('Name is required.'); return; }
        let obs = this.editMode ? this.svc.update(+this.item.id!, this.item) : this.svc.create(this.item);
        obs.subscribe({
            next: () => {
                this.toastr.success(this.editMode ? 'Sponsor updated.' : 'Sponsor created.');
                this.showForm = false;
                this.load();
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error saving sponsor.')
        });
    };

    delete = (s: Sponsor) => {
        Swal.fire({
            title: `Delete sponsor '${s.name}'?`, icon: 'warning',
            width: 400, position: 'top', padding: '1em',
            showCancelButton: true, confirmButtonColor: '#d33', confirmButtonText: 'Delete'
        }).then(r => {
            if (!r.value) return;
            this.svc.delete(+s.id!).subscribe({
                next: () => { this.toastr.success('Deleted.'); this.load(); },
                error: (err) => this.toastr.error(err.error?.message || 'Error deleting.')
            });
        });
    };

    openStatement = (s: Sponsor) => {
        this.statementSponsor = s;
        this.statement = null;
        this.statementFromDate = '';
        this.statementToDate = '';
        this.loadStatement();
    };

    loadStatement = () => {
        if (!this.statementSponsor) return;
        this.svc.statement(+this.statementSponsor.id!, this.statementFromDate, this.statementToDate).subscribe({
            next: (s) => this.statement = s,
            error: (err) => this.toastr.error(err.error?.message || 'Error loading statement.')
        });
    };

    closeStatement = () => { this.statementSponsor = null; this.statement = null; };

    typeBadge = (t: any): string => {
        switch (t) {
            case SponsorType.External: return 'bg-primary';
            case SponsorType.School: return 'bg-success';
            case SponsorType.Government: return 'bg-info';
            case SponsorType.Parent: return 'bg-warning text-dark';
            default: return 'bg-secondary';
        }
    };
}
