import {Component} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';

@Component({
    selector: 'app-missing-marks-page',
    template: `
        <app-dashboard-header [title]="'Academics: Missing Marks Report'" [breadcrumbs]="breadcrumbs"></app-dashboard-header>
        <hr>
        <section class="content"><div class="container-fluid px-1">
            <app-missing-marks-report></app-missing-marks-report>
        </div></section>
    `
})
export class MissingMarksPageComponent {
    breadcrumbs: BreadCrumb[] = [{link: ['/'], title: 'Dashboard'}, {link: ['/reports/academics/missing-marks'], title: 'Academics: Missing Marks'}];
}
