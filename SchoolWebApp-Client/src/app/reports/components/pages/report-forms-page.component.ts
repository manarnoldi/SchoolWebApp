import {Component} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';

@Component({
    selector: 'app-report-forms-page',
    template: `
        <section class="content"><div class="container-fluid px-1">
            <app-report-form></app-report-form>
        </div></section>
    `
})
export class ReportFormsPageComponent {
    breadcrumbs: BreadCrumb[] = [{link: ['/'], title: 'Dashboard'}, {link: ['/reports/academics/report-forms'], title: 'Academics: Report Forms'}];
}
