import {Component} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';

// Mirror of StaffSubjectPageComponent - the route component that wraps the
// student-subject detailed report with breadcrumbs + dashboard header.
@Component({
    selector: 'app-student-subject-page',
    template: `
        <app-dashboard-header [title]="'Student: Subject Allocation Report'" [breadcrumbs]="breadcrumbs"></app-dashboard-header>
        <hr>
        <section class="content"><div class="container-fluid px-1">
            <app-student-subject-detailed-report></app-student-subject-detailed-report>
        </div></section>
    `
})
export class StudentSubjectPageComponent {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/academics/student-subject-allocation'], title: 'Student: Subject Allocation'}
    ];
}
