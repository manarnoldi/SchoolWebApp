import {BreadCrumb} from '@/core/models/bread-crumb';
import {ReportName} from '@/reports/models/report-names';
import {Component} from '@angular/core';

@Component({
    selector: 'app-staff-report',
    templateUrl: './staff-report.component.html',
    styleUrl: './staff-report.component.scss'
})
export class StaffReportComponent {
    currentReport: number = 0;
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/staff'], title: 'Reports: Staff'}
    ];
    dashboardTitle = 'Reports: Staff';

    searchItem = (rn: ReportName) => {
        this.currentReport = rn.id;
    };

    reportNameChanged = (rn: ReportName) => {
        this.currentReport = 0;
    };
}
