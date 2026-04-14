import {Component} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';

@Component({
    selector: 'app-class-attendance-page',
    template: `
        <app-dashboard-header [title]="'Class: Attendance Report'" [breadcrumbs]="breadcrumbs"></app-dashboard-header>
        <hr>
        <section class="content"><div class="container-fluid px-1">
            <app-class-attendance-report></app-class-attendance-report>
        </div></section>
    `
})
export class ClassAttendancePageComponent {
    breadcrumbs: BreadCrumb[] = [{link: ['/'], title: 'Dashboard'}, {link: ['/reports/class/attendance'], title: 'Class: Attendance Report'}];
}
