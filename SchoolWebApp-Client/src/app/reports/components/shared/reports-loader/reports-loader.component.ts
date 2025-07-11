import {ReportName} from '@/reports/models/report-names';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

@Component({
    selector: 'app-reports-loader',
    templateUrl: './reports-loader.component.html',
    styleUrl: './reports-loader.component.scss'
})
export class ReportsLoaderComponent implements OnInit {
    @Input() reportTitle: string = '';
    @Input() showSubreport: boolean = false;
    @Output() searchItemEvent = new EventEmitter<ReportName>();
    @Output() reportNameChangedEvent = new EventEmitter<ReportName>();
    @Output() subReportNameChangedEvent = new EventEmitter<ReportName>();

    reportLoaderForm: FormGroup;
    reportNames: ReportName[] = [];
    subReports: ReportName[] = [];

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.reportNames = [];
        this.subReports = [];

        this.subReports.push(
            new ReportName({
                id: 100,
                title: 'Broadsheet Marks report - Per stream',
                category: 'academics-subreport',
                code: 'ACADSUB100',
                rank: 1
            })
        );
        this.subReports.push(
            new ReportName({
                id: 200,
                title: 'Broadsheet Marks report - Per class',
                category: 'academics-subreport',
                code: 'ACADSUB200',
                rank: 2
            })
        );

        this.subReports.push(
            new ReportName({
                id: 300,
                title: 'Broadsheet Grades report - Per stream',
                category: 'academics-subreport',
                code: 'ACADSUB300',
                rank: 3
            })
        );

        this.subReports.push(
            new ReportName({
                id: 400,
                title: 'Broadsheet Grades report - Per class',
                category: 'academics-subreport',
                code: 'ACADSUB400',
                rank: 4
            })
        );

        this.subReports.push(
            new ReportName({
                id: 500,
                title: 'Broadsheet Points report - Per stream',
                category: 'academics-subreport',
                code: 'ACADSUB500',
                rank: 5
            })
        );

        this.subReports.push(
            new ReportName({
                id: 600,
                title: 'Broadsheet Points report - Per Class',
                category: 'academics-subreport',
                code: 'ACADSUB600',
                rank: 6
            })
        );

        this.subReports.push(
            new ReportName({
                id: 700,
                title: 'Subject summary report',
                category: 'academics-subreport',
                code: 'ACADSUB700',
                rank: 7
            })
        );
        this.subReports.push(
            new ReportName({
                id: 800,
                title: 'School summary report',
                category: 'academics-subreport',
                code: 'ACADSUB800',
                rank: 8
            })
        );

        this.subReports.push(
            new ReportName({
                id: 900,
                title: 'Learners progress report',
                category: 'academics-subreport',
                code: 'ACADSUB900',
                rank: 9
            })
        );

        if (this.reportTitle.toLowerCase() == 'staff') {
            this.reportNames.push(
                new ReportName({
                    id: 1,
                    title: 'Staff attendance report',
                    category: 'staff',
                    code: 'STAFF001',
                    rank: 1
                })
            );
            this.reportNames.push(
                new ReportName({
                    id: 2,
                    title: 'Staff attendance report - detailed',
                    category: 'staff',
                    code: 'STAFF002',
                    rank: 2
                })
            );
            this.reportNames.push(
                new ReportName({
                    id: 3,
                    title: 'Staff subject allocation report',
                    category: 'staff',
                    code: 'STAFF003',
                    rank: 3
                })
            );
        } else if (this.reportTitle.toLowerCase() == 'class') {
            this.reportNames.push(
                new ReportName({
                    id: 4,
                    title: 'Class list report',
                    category: 'class',
                    code: 'CLASS001',
                    rank: 1
                })
            );
            this.reportNames.push(
                new ReportName({
                    id: 5,
                    title: 'Class attendance report',
                    category: 'class',
                    code: 'CLASS002',
                    rank: 2
                })
            );
            this.reportNames.push(
                new ReportName({
                    id: 6,
                    title: 'Class attendance report - details',
                    category: 'class',
                    code: 'CLASS003',
                    rank: 3
                })
            );
        } else if (this.reportTitle.toLowerCase() == 'academics') {
            this.reportNames.push(
                new ReportName({
                    id: 6,
                    title: 'Missing marks report',
                    category: 'academics',
                    code: 'ACADEMICS001',
                    rank: 1
                })
            );
            this.reportNames.push(
                new ReportName({
                    id: 7,
                    title: 'Results analysis Report',
                    category: 'academics',
                    code: 'ACADEMICS002',
                    rank: 2
                })
            );
            this.reportNames.push(
                new ReportName({
                    id: 8,
                    title: 'Report forms',
                    category: 'academics',
                    code: 'ACADEMICS003',
                    rank: 3
                })
            );
        }
        this.refreshItems();
    }

    refreshItems = () => {
        this.reportLoaderForm = this.formBuilder.group({
            reportName: [null, [Validators.required]],
            subReportName: [null]
        });
    };

    reportNameChanged = () => {
        this.reportLoaderForm.get('subReportName').reset();

        let reportName = new ReportName(
            this.reportLoaderForm.get('reportName').value
        );
        this.reportNameChangedEvent.emit(reportName);
    };

    subReportNameChanged = () => {
        let subReportName = new ReportName(
            this.reportLoaderForm.get('subReportName').value
        );
        this.subReportNameChangedEvent.emit(subReportName);
    };

    onSubmit = () => {
        let reportName = new ReportName(
            this.reportLoaderForm.get('reportName').value
        );
        this.searchItemEvent.emit(reportName);
    };
}
