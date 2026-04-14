import {Component} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';

@Component({
    selector: 'app-results-analysis-page',
    template: `
        <app-dashboard-header [title]="'Academics: Results Analysis'" [breadcrumbs]="breadcrumbs"></app-dashboard-header>
        <hr>
        <section class="content"><div class="container-fluid px-1">
            <app-broadsheet></app-broadsheet>
        </div></section>
    `
})
export class ResultsAnalysisPageComponent {
    breadcrumbs: BreadCrumb[] = [{link: ['/'], title: 'Dashboard'}, {link: ['/reports/academics/results-analysis'], title: 'Academics: Results Analysis'}];
}
