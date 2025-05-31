import {BreadCrumb} from '@/core/models/bread-crumb';
import {Component} from '@angular/core';

@Component({
    selector: 'app-academics-report',
    templateUrl: './academics-report.component.html',
    styleUrl: './academics-report.component.scss'
})
export class AcademicsReportComponent {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/academics'], title: 'Reports: Academics'}
    ];
    dashboardTitle = 'Reports: Academics';
}
