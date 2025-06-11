import {BreadCrumb} from '@/core/models/bread-crumb';
import {ReportName} from '@/reports/models/report-names';
import {Component} from '@angular/core';

@Component({
    selector: 'app-staff-report',
    templateUrl: './staff-report.component.html',
    styleUrl: './staff-report.component.scss'
})
export class StaffReportComponent {
    currentReport: ReportName = null;
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/staff'], title: 'Reports: Staff'}
    ];
    dashboardTitle = 'Reports: Staff';

    searchItem = (rn: ReportName) => {
        this.currentReport = rn;
    };

    reportNameChanged = (rn: ReportName) => {
        this.currentReport = null;
    };
}
