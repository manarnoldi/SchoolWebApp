import {Component} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';

@Component({
    selector: 'app-class-list-page',
    template: `
        <app-dashboard-header [title]="'Class: Class List Report'" [breadcrumbs]="breadcrumbs"></app-dashboard-header>
        <hr>
        <section class="content"><div class="container-fluid px-1">
            <app-class-lists-report></app-class-lists-report>
        </div></section>
    `
})
export class ClassListPageComponent {
    breadcrumbs: BreadCrumb[] = [{link: ['/'], title: 'Dashboard'}, {link: ['/reports/class/class-list'], title: 'Class: Class List'}];
}
