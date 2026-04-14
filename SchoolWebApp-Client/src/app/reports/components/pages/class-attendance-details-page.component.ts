import {Component} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';

@Component({
    selector: 'app-class-attendance-details-page',
    template: `
        <app-dashboard-header [title]="'Class: Attendance Report - Detailed'" [breadcrumbs]="breadcrumbs"></app-dashboard-header>
        <hr>
        <section class="content"><div class="container-fluid px-1">
            <app-class-attendance-details-report></app-class-attendance-details-report>
        </div></section>
    `
})
export class ClassAttendanceDetailsPageComponent {
    breadcrumbs: BreadCrumb[] = [{link: ['/'], title: 'Dashboard'}, {link: ['/reports/class/attendance-details'], title: 'Class: Attendance Detailed'}];
}
