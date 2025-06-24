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
    @Output() searchItemEvent = new EventEmitter<ReportName>();
    @Output() reportNameChangedEvent = new EventEmitter<ReportName>();

    reportLoaderForm: FormGroup;
    reportNames: ReportName[] = [];

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.reportNames = [];
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
            reportName: [null, [Validators.required]]
        });
    };

    reportNameChanged = () => {
        let reportName = new ReportName(
            this.reportLoaderForm.get('reportName').value
        );
        this.reportNameChangedEvent.emit(reportName);
    };
    onSubmit = () => {
        let reportName = new ReportName(
            this.reportLoaderForm.get('reportName').value
        );
        this.searchItemEvent.emit(reportName);
    };
}
