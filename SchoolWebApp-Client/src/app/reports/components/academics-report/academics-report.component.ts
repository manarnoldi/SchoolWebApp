import {BreadCrumb} from '@/core/models/bread-crumb';
import {ReportName} from '@/reports/models/report-names';
import {Component} from '@angular/core';

@Component({
    selector: 'app-academics-report',
    templateUrl: './academics-report.component.html',
    styleUrl: './academics-report.component.scss'
})
export class AcademicsReportComponent {
    currentReport: ReportName = null;
    showSubReport: boolean = false;
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/academics'], title: 'Reports: Academics'}
    ];
    dashboardTitle = 'Reports: Academics';

    searchItem = (rn: ReportName) => {
        this.currentReport = rn;
    };

    reportNameChanged = (rn: ReportName) => {
        if (rn.code === 'ACADEMICS002') {
            this.showSubReport = true;
        } else {
            this.showSubReport = false;
        }
        this.currentReport = null;
    };

    subReportNameChanged = (rn: ReportName) => {
        this.currentReport = null;
    };
}
