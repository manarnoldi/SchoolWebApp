import {BreadCrumb} from '@/core/models/bread-crumb';
import {ReportName} from '@/reports/models/report-names';
import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';

@Component({
    selector: 'app-academics-report',
    templateUrl: './academics-report.component.html',
    styleUrl: './academics-report.component.scss'
})
export class AcademicsReportComponent implements OnInit {
    currentReport: ReportName = null;
    showSubReport: boolean = false;
    querySource: string = '';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/academics'], title: 'Reports: Academics'}
    ];
    dashboardTitle = 'Reports: Academics';

    constructor(private route: ActivatedRoute) {}

    ngOnInit(): void {
        this.querySource = this.route.snapshot.queryParamMap.get('source') || '';
    }

    searchItem = (rn: ReportName) => {
        this.currentReport = rn;
    };

    reportNameChanged = (rn: ReportName) => {
        if (rn.code === 'ACADEMICS002') {
            this.showSubReport = true;
        } else {
            this.showSubReport = false;
        }
        this.currentReport = rn;
    };

    subReportNameChanged = (rn: ReportName) => {
        if (this.currentReport) {
            this.currentReport.subReport = rn;
        }
    };
}
