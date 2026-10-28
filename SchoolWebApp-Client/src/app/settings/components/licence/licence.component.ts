import {Component, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ALL_MODULES, AppModule, LicenceService, LicenceStatus, MODULE_LABELS} from '@/core/services/licence.service';

const STATE_BADGES: {[s: string]: string} = {
    Active: 'bg-success', Expiring: 'bg-warning text-dark', Grace: 'bg-warning text-dark',
    Expired: 'bg-danger', Invalid: 'bg-danger', WrongSite: 'bg-danger', None: 'bg-secondary'
};
const STATE_LABELS: {[s: string]: string} = {
    Active: 'Active', Expiring: 'Expiring soon', Grace: 'Expired - grace period', Expired: 'Expired',
    Invalid: 'Invalid key', WrongSite: 'For another site', None: 'Not installed'
};

/** Settings > Licence: what the school's licence includes, and installing the key Kodetek sends. */
@Component({
    selector: 'app-licence',
    template: `
        <app-dashboard-header [title]="'Settings: Licence'" [breadcrumbs]="breadcrumbs"></app-dashboard-header>
        <hr class="mb-1">
        <section class="content">
            <div class="container-fluid px-1">
                <div class="row g-2">
                    <!-- The licence now -->
                    <div class="col-lg-7">
                        <div class="card card-outline card-primary h-100 mb-0">
                            <div class="card-header py-2 d-flex align-items-center">
                                <h6 class="mb-0 me-auto"><i class="fas fa-key text-warning me-1"></i>ShuleNova licence</h6>
                                <span class="badge" *ngIf="s" [ngClass]="badges[s.state]">{{ labels[s.state] || s.state }}</span>
                            </div>
                            <div class="card-body">
                                <div *ngIf="!s" class="text-muted"><i class="fas fa-spinner fa-spin"></i> Loading...</div>
                                <ng-container *ngIf="s">
                                    <div class="alert py-2" *ngIf="s.message"
                                        [ngClass]="s.state === 'Expiring' || s.state === 'Grace' ? 'alert-warning' : 'alert-danger'">{{ s.message }}</div>
                                    <dl class="row mb-3" *ngIf="s.licenceId">
                                        <dt class="col-sm-4">Licensed to</dt><dd class="col-sm-8">{{ s.school }}</dd>
                                        <dt class="col-sm-4">Licence</dt><dd class="col-sm-8">{{ s.licenceId }}</dd>
                                        <dt class="col-sm-4">Site</dt><dd class="col-sm-8">{{ s.hosts.length ? s.hosts.join(', ') : 'Any' }}</dd>
                                        <dt class="col-sm-4">Issued</dt><dd class="col-sm-8">{{ s.issued | date:'d MMM y' }}</dd>
                                        <dt class="col-sm-4">Valid until</dt>
                                        <dd class="col-sm-8">{{ s.expires | date:'d MMM y' }}
                                            <span class="text-muted" *ngIf="s.daysLeft !== null && s.daysLeft >= 0"> ({{ s.daysLeft }} days left)</span></dd>
                                    </dl>
                                    <h6 class="mb-2">Modules</h6>
                                    <ul class="list-group mb-2">
                                        <li class="list-group-item d-flex align-items-center py-2">
                                            <i class="fas fa-check-circle text-success me-2"></i>
                                            <span class="me-auto"><b>Core</b> <small class="text-muted ms-1">- dashboard, school, students, staff, settings, reports, security</small></span>
                                            <span class="badge bg-light text-success border border-success">Always included</span>
                                        </li>
                                        <li class="list-group-item d-flex align-items-center py-2" *ngFor="let m of modules">
                                            <i class="fas me-2" [ngClass]="s.modules.includes(m) ? 'fa-check-circle text-success' : 'fa-minus-circle text-muted'"></i>
                                            <span class="me-auto" [class.text-muted]="!s.modules.includes(m)">{{ moduleLabels[m] }}</span>
                                            <span class="badge" [ngClass]="s.modules.includes(m) ? 'bg-success' : 'bg-light text-muted border'">
                                                {{ s.modules.includes(m) ? 'On' : (s.licensedModules.includes(m) ? 'Licence not valid' : 'Not licensed') }}</span>
                                        </li>
                                    </ul>
                                    <small class="text-muted">To add a module or renew, contact Kodetek: they send a new key to install here.</small>
                                </ng-container>
                            </div>
                        </div>
                    </div>

                    <!-- Install a key -->
                    <div class="col-lg-5">
                        <div class="card h-100 mb-0">
                            <div class="card-header py-2"><h6 class="mb-0"><i class="fas fa-upload text-primary me-1"></i>Install a licence key</h6></div>
                            <div class="card-body">
                                <ng-container *ngIf="isAdmin; else notAdmin">
                                    <textarea class="form-control font-monospace small mb-2" rows="7" [(ngModel)]="key"
                                        placeholder="Paste the key Kodetek sent (it starts with SNL1.)"></textarea>
                                    <button class="btn btn-primary" [disabled]="busy || !key.trim()" (click)="install()">
                                        <i class="fas" [ngClass]="busy ? 'fa-spinner fa-spin' : 'fa-check'"></i> Install</button>
                                    <div class="small text-muted mt-2">The key is checked before it replaces the current one. Menus update straight away.</div>
                                </ng-container>
                                <ng-template #notAdmin><div class="text-muted">Only administrators can install a licence key.</div></ng-template>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>`
})
export class LicenceComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/licence'], title: 'Licence'}
    ];
    s: LicenceStatus | null = null;
    key = '';
    busy = false;
    modules: AppModule[] = ALL_MODULES;
    moduleLabels = MODULE_LABELS;
    badges = STATE_BADGES;
    labels = STATE_LABELS;
    isAdmin = false;

    constructor(private licence: LicenceService, private toastr: ToastrService) {}

    ngOnInit(): void {
        const user = JSON.parse(localStorage.getItem('current_user') || 'null');
        this.isAdmin = !!user?.roles?.some((r: string) => ['administrator', 'superadministrator'].includes(r.toLowerCase()));
        this.licence.refresh().subscribe((s) => (this.s = s));
    }

    install() {
        this.busy = true;
        this.licence.install(this.key.trim()).subscribe({
            next: (s) => {
                this.busy = false;
                this.s = s;
                this.key = '';
                this.toastr.success('Licence installed. Reloading the menus...');
                // The menus and pages are built from the licence: start afresh.
                setTimeout(() => location.reload(), 1200);
            },
            error: (err) => {
                this.busy = false;
                this.toastr.error(typeof err?.error === 'string' ? err.error : 'The licence key could not be installed.');
            }
        });
    }
}
