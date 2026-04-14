import {Component} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';

@Component({
    selector: 'app-staff-subject-page',
    template: `
        <app-dashboard-header [title]="'Staff: Subject Allocation Report'" [breadcrumbs]="breadcrumbs"></app-dashboard-header>
        <hr>
        <section class="content"><div class="container-fluid px-1">
            <app-staff-subject-detailed-report></app-staff-subject-detailed-report>
        </div></section>
    `
})
export class StaffSubjectPageComponent {
    breadcrumbs: BreadCrumb[] = [{link: ['/'], title: 'Dashboard'}, {link: ['/reports/staff/subject-allocation'], title: 'Staff: Subject Allocation'}];
}
