import {Component} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';

@Component({
    selector: 'app-staff-attendance-page',
    template: `
        <app-dashboard-header [title]="'Staff: Attendance Report'" [breadcrumbs]="breadcrumbs"></app-dashboard-header>
        <hr>
        <section class="content"><div class="container-fluid px-1">
            <app-staff-attendance-report></app-staff-attendance-report>
        </div></section>
    `
})
export class StaffAttendancePageComponent {
    breadcrumbs: BreadCrumb[] = [{link: ['/'], title: 'Dashboard'}, {link: ['/reports/staff/attendance'], title: 'Staff: Attendance Report'}];
}
