import {BreadCrumb} from '@/core/models/bread-crumb';
import {ReportName} from '@/reports/models/report-names';
import {Component} from '@angular/core';

@Component({
    selector: 'app-class-report',
    templateUrl: './class-report.component.html',
    styleUrl: './class-report.component.scss'
})
export class ClassReportComponent {
    currentReport: ReportName = null;
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/class'], title: 'Reports: Class'}
    ];
    dashboardTitle = 'Reports: Class';

    searchItem = (rn: ReportName) => {
        this.currentReport = rn;
    };

    reportNameChanged = (rn: ReportName) => {
        this.currentReport = null;
    };
}
