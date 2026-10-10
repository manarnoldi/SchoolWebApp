import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {CommunicationSetting} from '@/communications/models/communication-models';
import {CommunicationSettingsService} from '@/communications/services/communication-services';

export function apiError(err: any, fallback = 'Something went wrong.'): string {
    let e = err?.error;
    if (typeof e === 'string' && e) return e;
    return e?.message || e?.title || fallback;
}

/**
 * SMS gateway (TextSMS) and SMTP account. SuperAdministrator only - the API
 * refuses everyone else. Secrets are never sent back: a blank API key or
 * password field on save keeps the stored one.
 */
@Component({
    selector: 'app-communication-settings',
    templateUrl: './communication-settings.component.html'
})
export class CommunicationSettingsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/communications/settings'], title: 'Communications Settings'}
    ];
    dashboardTitle = 'Communications: Gateway Settings';

    s: CommunicationSetting | null = null;
    loadError: string | null = null;
    saving = false;

    testPhone = '';
    testEmailTo = '';
    testing = false;
    balance: string | null = null;

    constructor(private toastr: ToastrService, private svc: CommunicationSettingsService) {}

    ngOnInit(): void {
        this.load();
    }

    load() {
        this.svc.get().subscribe({
            next: (r) => {
                this.s = r;
                this.testPhone = r.testPhoneNumber || '';
                this.testEmailTo = r.testEmail || '';
            },
            error: (err) => this.loadError = err.status === 403
                ? 'Only a SuperAdministrator can view or change the gateway settings.'
                : apiError(err, 'Could not load the settings.')
        });
    }

    save() {
        if (!this.s) return;
        this.saving = true;
        this.svc.update(this.s).subscribe({
            next: () => {
                this.saving = false;
                this.toastr.success('Communications settings saved.');
                this.load();
            },
            error: (err) => {
                this.saving = false;
                this.toastr.error(apiError(err));
            }
        });
    }

    sendTestSms() {
        if (!this.testPhone) { this.toastr.warning('Enter the phone number to send the test SMS to.'); return; }
        this.testing = true;
        this.svc.testSms(this.testPhone).subscribe({
            next: (r) => { this.testing = false; this.toastr.success(r?.message || 'Test SMS sent.'); },
            error: (err) => { this.testing = false; this.toastr.error(apiError(err)); }
        });
    }

    sendTestEmail() {
        if (!this.testEmailTo) { this.toastr.warning('Enter the email address to send the test email to.'); return; }
        this.testing = true;
        this.svc.testEmail(this.testEmailTo).subscribe({
            next: (r) => { this.testing = false; this.toastr.success(r?.message || 'Test email sent.'); },
            error: (err) => { this.testing = false; this.toastr.error(apiError(err)); }
        });
    }

    checkBalance() {
        this.balance = null;
        this.svc.smsBalance().subscribe({
            next: (r) => this.balance = this.describeBalance(r.raw),
            error: (err) => this.toastr.error(apiError(err))
        });
    }

    // The gateway does not document the balance response; show its credit
    // figure when one is recognisable, the raw reply otherwise.
    private describeBalance(raw: string): string {
        try {
            let j = JSON.parse(raw);
            let credit = j?.credit ?? j?.balance ?? j?.['credit-balance'];
            if (credit !== undefined && credit !== null) return `${credit} SMS credits`;
        } catch {}
        return raw || 'No balance returned.';
    }
}
