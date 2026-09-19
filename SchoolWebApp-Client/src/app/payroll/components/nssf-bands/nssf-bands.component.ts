import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {NssfBand} from '@/payroll/models/payroll-models';
import {NssfBandService} from '@/payroll/services/payroll-services';

@Component({
    selector: 'app-payroll-nssf-bands',
    templateUrl: './nssf-bands.component.html'
})
export class PayrollNssfBandsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/payroll/nssf-bands'], title: 'NSSF Bands'}
    ];
    dashboardTitle = 'Payroll: NSSF Bands';

    items: NssfBand[] = [];
    item: NssfBand = this.blank();
    editMode = false;
    showForm = false;

    page = 1;
    pageSize = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(private toastr: ToastrService, private svc: NssfBandService) {}

    ngOnInit(): void { this.load(); }

    private blank(): NssfBand {
        return new NssfBand({
            tier: 1, lowerLimit: 0, upperLimit: 0, rate: 6, isActive: true,
            effectiveDate: new Date().toISOString().substring(0, 10)
        });
    }

    load(): void {
        this.svc.get('/nssfBands').subscribe({
            next: (r) => this.items = r || [],
            error: (err) => this.toastr.error(err.error?.message || 'Error loading NSSF bands.')
        });
    }

    // Bands sharing an effective date form one set; payroll uses the latest set in
    // force for the period being run. Grouping makes that visible.
    effectiveDates(): string[] {
        let dates = new Set((this.items || []).map((b) => (b.effectiveDate || '').substring(0, 10)));
        return Array.from(dates).sort((a, b) => b.localeCompare(a));
    }

    bandsFor(date: string): NssfBand[] {
        return (this.items || [])
            .filter((b) => (b.effectiveDate || '').substring(0, 10) === date)
            .sort((a, b) => (a.tier || 0) - (b.tier || 0));
    }

    // The set payroll would use today, so it is obvious which one is live.
    currentSet(): string | null {
        let today = new Date().toISOString().substring(0, 10);
        let inForce = this.effectiveDates().filter((d) => d <= today);
        return inForce.length ? inForce[0] : null;
    }

    // A set runs until the day before the next one starts. Showing the span makes
    // it obvious which years a set covers, rather than just when it began.
    endDateFor(date: string): string | null {
        let dates = this.effectiveDates();           // newest first
        let i = dates.indexOf(date);
        if (i <= 0) return null;                    // newest set: still running
        let nextStart = new Date(dates[i - 1] + 'T00:00:00');
        nextStart.setDate(nextStart.getDate() - 1);
        return nextStart.toISOString().substring(0, 10);
    }

    maxContributionFor(date: string): number {
        return this.bandsFor(date)
            .filter((b) => b.isActive)
            .reduce((sum, b) => sum + ((+b.upperLimit! - +b.lowerLimit!) * (+b.rate! || 0) / 100), 0);
    }

    addNew(): void { this.item = this.blank(); this.editMode = false; this.showForm = true; }

    // Adds the next tier onto an existing set, pre-filled to continue from the
    // last tier's ceiling - which is how the tiers actually join up.
    addTierTo(date: string): void {
        let existing = this.bandsFor(date);
        let last = existing[existing.length - 1];
        this.item = new NssfBand({
            name: `NSSF Tier ${(last?.tier || 0) + 1}`,
            tier: (last?.tier || 0) + 1,
            lowerLimit: last ? +last.upperLimit! : 0,
            upperLimit: 0,
            rate: last ? +last.rate! : 6,
            effectiveDate: date,
            isActive: true
        });
        this.editMode = false;
        this.showForm = true;
    }

    edit(x: NssfBand): void {
        this.item = new NssfBand({...x, effectiveDate: (x.effectiveDate || '').substring(0, 10)});
        this.editMode = true;
        this.showForm = true;
    }

    cancel(): void { this.showForm = false; }

    save(): void {
        if (!this.item.name) { this.toastr.warning('Name is required.'); return; }
        if (!this.item.effectiveDate) { this.toastr.warning('Effective date is required.'); return; }
        if ((+this.item.tier! || 0) < 1) { this.toastr.warning('Tier must be 1 or higher.'); return; }
        if (+this.item.upperLimit! <= +this.item.lowerLimit!) {
            this.toastr.warning('The upper limit must be above the lower limit.');
            return;
        }
        if (+this.item.rate! <= 0) { this.toastr.warning('Rate must be greater than zero.'); return; }

        let req = this.editMode
            ? this.svc.update('/nssfBands', this.item)
            : this.svc.create('/nssfBands', this.item);
        req.subscribe({
            next: () => { this.toastr.success('Saved.'); this.showForm = false; this.load(); },
            error: (err) => this.toastr.error(err.error?.message || 'Error saving.')
        });
    }

    delete(x: NssfBand): void {
        Swal.fire({
            title: 'Delete band?',
            text: `Remove ${x.name} effective ${(x.effectiveDate || '').substring(0, 10)}?`,
            icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'
        }).then((r) => {
            if (r.value) {
                this.svc.delete('/nssfBands', parseInt(x.id)).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error deleting.')
                });
            }
        });
    }
}
