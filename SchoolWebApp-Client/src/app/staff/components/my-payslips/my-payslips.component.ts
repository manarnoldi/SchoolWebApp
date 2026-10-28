import {Component} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';

/** A staff member's own payslips (approved payrolls), from their profile - no staff-records access needed. */
@Component({
    selector: 'app-my-payslips',
    template: `
        <app-dashboard-header [title]="'My Payslips'" [breadcrumbs]="breadcrumbs"></app-dashboard-header>
        <hr class="mb-1">
        <section class="content">
            <div class="container-fluid px-1">
                <div class="card card-outline card-success">
                    <div class="card-header py-2"><h6 class="mb-0"><i class="fas fa-file-invoice-dollar text-success me-1"></i>My payslips</h6></div>
                    <div class="card-body p-2">
                        <app-staff-pay *ngIf="personId; else noStaff" [staffId]="personId"></app-staff-pay>
                        <ng-template #noStaff><div class="text-muted">Your sign-in is not linked to a staff record, so there are no payslips to show.</div></ng-template>
                    </div>
                </div>
            </div>
        </section>`
})
export class MyPayslipsComponent {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/profile'], title: 'My Profile'},
        {link: ['/staff/my-payslips'], title: 'My Payslips'}
    ];
    personId: number | null = JSON.parse(localStorage.getItem('current_user') || 'null')?.personId ?? null;
}
