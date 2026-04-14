import {Component} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';

@Component({
    selector: 'app-staff-attendance-details-page',
    template: `
        <app-dashboard-header [title]="'Staff: Attendance Report - Detailed'" [breadcrumbs]="breadcrumbs"></app-dashboard-header>
        <hr>
        <section class="content"><div class="container-fluid px-1">
            <app-staff-attendance-details-report></app-staff-attendance-details-report>
        </div></section>
    `
})
export class StaffAttendanceDetailsPageComponent {
    breadcrumbs: BreadCrumb[] = [{link: ['/'], title: 'Dashboard'}, {link: ['/reports/staff/attendance-details'], title: 'Staff: Attendance Detailed'}];
}
