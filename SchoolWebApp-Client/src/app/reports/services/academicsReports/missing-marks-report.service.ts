import {ExamResult} from '@/academics/models/exam-result';
import {SchoolDetails} from '@/school/models/school-details';
import {Injectable} from '@angular/core';
import {ReportsService} from '../reports.service';
import {AuthService} from '@/core/services/auth.service';

import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
(pdfMake as any).vfs = pdfFonts;

@Injectable({
    providedIn: 'root'
})
export class MissingMarksReportService {
    constructor(
        private reportSvc: ReportsService,
        private userSvc: AuthService
    ) {}

    generateReport = (
        schoolDetails: SchoolDetails,
        missingResults: ExamResult[],
        reportTitle: string
    ) => {
        this.reportSvc
            .loadImageAsBase64('assets/img/shule-nova-logo-only.png')
            .subscribe({
                next: (blob) => {
                    const reader = new FileReader();
                    reader.onloadend = () => {
                        const base64data: string = reader.result as string;
                        const colHeaders = [
                            {text: '#', style: 'tableHeader'},
                            {text: 'Class', style: 'tableHeader'},
                            {text: 'Subject', style: 'tableHeader'},
                            {text: 'Adm#', style: 'tableHeader'},
                            {text: 'Student name', style: 'tableHeader'},
                            {text: 'Exam Type', style: 'tableHeader'},
                            {text: 'Exam Name', style: 'tableHeader'},
                            {text: 'Out of', style: 'tableHeader'},
                            {text: 'Contr', style: 'tableHeader'}
                        ];
                        const tableWidths = [
                            'auto',
                            'auto',
                            '*',
                            'auto',
                            '*',
                            'auto',
                            '*',
                            'auto',
                            'auto'
                        ];
                        const tableBody = [
                            [...colHeaders],
                            ...missingResults.map((mmR, indexNum) => [
                                {
                                    text: indexNum + 1,
                                    noWrap: true
                                },
                                {
                                    text: mmR.exam?.schoolClass?.name,
                                    noWrap: true
                                },
                                {
                                    text: mmR.exam?.subject?.name,
                                    noWrap: false
                                },
                                {
                                    text: mmR.student?.upi,
                                    noWrap: true
                                },
                                {
                                    text: mmR.student?.fullName,
                                    noWrap: false
                                },
                                {
                                    text: mmR.exam?.examName?.examType?.name,
                                    noWrap: false
                                },
                                {
                                    text: mmR.exam?.examName?.name,
                                    noWrap: false
                                },
                                {
                                    text: mmR.exam?.examMark,
                                    noWrap: true
                                },
                                {
                                    text: mmR.exam?.contributingMark,
                                    noWrap: true
                                }
                            ])
                        ];

                        const docDefinition = {
                            pageOrientation: 'landscape',
                            pageMargins: [20, 20, 20, 40],
                            pageSize: 'A4',
                            info: {
                                title: 'Missing marks report',
                                author:
                                    this.userSvc?.currentUser?.firstName +
                                    ' ' +
                                    this.userSvc?.currentUser?.lastName,
                                subject: reportTitle
                            },
                            watermark: this.reportSvc.getWatermark(
                                'ShuleNova - ' + schoolDetails?.name
                            ),
                            footer: this.reportSvc.getFooter('landscape'),
                            images: {
                                systemLogo: base64data,
                                schoolLogo: schoolDetails.logoAsBase64
                            },
                            styles: {
                                tableHeader: this.reportSvc.getHEADER_STYLE()
                            },
                            content: [
                                {...this.reportSvc.getDIVIDER()},
                                this.reportSvc.getReportHeader(schoolDetails),
                                {
                                    ...this.reportSvc.getDIVIDER(),
                                    marginBottom: 1
                                },
                                this.reportSvc.getReportTitle(reportTitle),
                                {
                                    ...this.reportSvc.getDIVIDER(),
                                    marginBottom: 1
                                },
                                {
                                    layout: this.reportSvc.getTableLayout(),
                                    table: {
                                        headerRows: 1,
                                        widths: tableWidths,
                                        body: tableBody
                                    },
                                    marginBottom: 2,
                                    color: '#002D62',
                                    fontSize: 10
                                },
                                {...this.reportSvc.getDIVIDER()},
                                this.reportSvc.getPrintDetails(
                                    this.userSvc?.currentUser?.firstName +
                                        ' ' +
                                        this.userSvc?.currentUser?.lastName,
                                    new Date().toLocaleString('en-GB')
                                )
                            ]
                        };

                        pdfMake.createPdf(docDefinition).open();
                    };

                    reader.readAsDataURL(blob);
                },
                error: (err) => {
                    console.log(err);
                }
            });
    };
}
