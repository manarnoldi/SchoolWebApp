import {SchoolDetails} from '@/school/models/school-details';
import {Injectable} from '@angular/core';
import {ReportsService} from '../reports.service';
import {AuthService} from '@/core/services/auth.service';
import {MissingMarksStudent} from '@/reports/models/missing-marks-student';

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
        missingResults: MissingMarksStudent[],
        reportTitle: string
    ) => {
        this.reportSvc
            .loadImageAsBase64('assets/img/shule-nova-logo-only.png')
            .subscribe({
                next: (blob) => {
                    const reader = new FileReader();
                    reader.onloadend = () => {
                        const base64data: string = reader.result as string;
                        // Group students by class so each class prints on its own
                        // page. Incoming rows are already ordered by class rank
                        // then admission no, so a class's rows are contiguous.
                        const classGroups: {className: string; rows: MissingMarksStudent[]}[] = [];
                        missingResults.forEach((r) => {
                            let last = classGroups[classGroups.length - 1];
                            if (!last || last.className !== r.className) {
                                last = {className: r.className, rows: []};
                                classGroups.push(last);
                            }
                            last.rows.push(r);
                        });

                        const classBlocks: any[] = [];
                        classGroups.forEach((cg, ci) => {
                            const body: any[] = [
                                [
                                    {text: '#', style: 'tableHeader'},
                                    {text: 'Adm#', style: 'tableHeader'},
                                    {text: 'Student name', style: 'tableHeader'},
                                    {text: 'Missing subjects (codes)', style: 'tableHeader'}
                                ]
                            ];
                            cg.rows.forEach((r, idx) =>
                                body.push([
                                    {text: idx + 1, noWrap: true},
                                    {text: r.upi, noWrap: true},
                                    {text: r.studentName, noWrap: false},
                                    {text: (r.subjectCodes || []).join(', '), noWrap: false}
                                ])
                            );
                            // Each class (after the first) starts on a new page.
                            classBlocks.push({
                                text: `${cg.className}  (${cg.rows.length} student(s))`,
                                bold: true,
                                fontSize: 11,
                                color: '#002D62',
                                marginBottom: 2,
                                pageBreak: ci === 0 ? undefined : 'before'
                            });
                            classBlocks.push({
                                layout: this.reportSvc.getTableLayout(),
                                table: {
                                    headerRows: 1,
                                    widths: ['auto', 'auto', '*', '*'],
                                    body
                                },
                                marginBottom: 2,
                                color: '#002D62',
                                fontSize: 10
                            });
                        });

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
                                {...this.reportSvc.getDIVIDER('landscape')},
                                this.reportSvc.getReportHeader(schoolDetails),
                                {
                                    ...this.reportSvc.getDIVIDER('landscape'),
                                    marginBottom: 1
                                },
                                this.reportSvc.getReportTitle(reportTitle),
                                {
                                    ...this.reportSvc.getDIVIDER('landscape'),
                                    marginBottom: 1
                                },
                                ...classBlocks,
                                {...this.reportSvc.getDIVIDER('landscape')},
                                this.reportSvc.getPrintDetails(
                                    this.userSvc?.currentUser?.firstName +
                                        ' ' +
                                        this.userSvc?.currentUser?.lastName,
                                    new Date().toLocaleString('en-GB')
                                )
                            ]
                        };

                        pdfMake.createPdf(docDefinition).getBlob((blob) => {
                            const url = URL.createObjectURL(blob);
                            window.open(url, '_blank');
                        });
                    };

                    reader.readAsDataURL(blob);
                },
                error: (err) => {
                    console.log(err);
                }
            });
    };
}
