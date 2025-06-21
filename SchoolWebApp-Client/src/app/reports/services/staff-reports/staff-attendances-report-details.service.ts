import {AuthService} from '@/core/services/auth.service';
import {Injectable} from '@angular/core';
import {ReportsService} from '../reports.service';
import {StaffAttendancesReport} from '../../models/staff-attendances-report';
import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {SchoolDetails} from '@/school/models/school-details';
import {StaffAttendance} from '@/staff/models/staff-attendance';

import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
import {DatePipe} from '@angular/common';
import {concatMap, forkJoin, from, map, Observable, of, switchMap} from 'rxjs';
import {ToastrService} from 'ngx-toastr';
(pdfMake as any).vfs = pdfFonts;

@Injectable({
    providedIn: 'root'
})
export class StaffAttendancesReportDetailsService extends ResourceService<StaffAttendance> {
    constructor(
        private http: HttpClient,
        private toastr: ToastrService,
        private userSvc: AuthService,
        private reportSvc: ReportsService,
        private datePipe: DatePipe
    ) {
        super(http, StaffAttendancesReport);
    }

    readBlobAsBase64(blob: Blob): Observable<string> {
        return new Observable<string>((observer) => {
            const reader = new FileReader();
            reader.onloadend = () => {
                observer.next(reader.result as string);
                observer.complete();
            };
            reader.onerror = (err) => observer.error(err);
            reader.readAsDataURL(blob);
        });
    }

    printByBatch = (
        staffAttendances: StaffAttendance[][],
        school: SchoolDetails,
        reportTitle: string
    ) => {
        const batchSize = 10;
        const total = staffAttendances.length;
        const batches = Math.ceil(total / batchSize);

        from(Array.from({length: batches}, (_, i) => i))
            .pipe(
                concatMap((batchIndex) => {
                    const start = batchIndex * batchSize;
                    const end = start + batchSize;
                    const batch = staffAttendances.slice(start, end);

                    const batchDocObservables = batch.map((att) =>
                        this.generateReport(school, att, reportTitle)
                    );

                    return forkJoin(batchDocObservables).pipe(
                        concatMap((docDefs) => {
                            const combinedContent = docDefs.flatMap(
                                (def, index) => {
                                    if (index === 0) {
                                        return def.content;
                                    } else {
                                        return [
                                            {
                                                text: '',
                                                pageBreak: 'before'
                                            },
                                            ...def.content
                                        ];
                                    }
                                }
                            );
                            const firstDoc = docDefs[0];
                            const mergedDocDef = {
                                content: combinedContent,
                                pageOrientation: firstDoc.pageOrientation,
                                pageMargins: firstDoc.pageMargins,
                                pageSize: firstDoc.pageSize,
                                info: firstDoc.info,
                                watermark: firstDoc.watermark,
                                footer: firstDoc.footer,
                                images: firstDoc.images,
                                styles: firstDoc.styles
                            };
                            return new Observable<void>((observer) => {
                                pdfMake.createPdf(mergedDocDef).print();
                                observer.next();
                                observer.complete();
                            });
                        })
                    );
                })
            )
            .subscribe({
                complete: () => {
                    console.log('✅ All batches generated & opened!');
                },
                error: (err) => {
                    this.toastr.error(err.error || err);
                }
            });
    };
    
    generateReport = (
        schoolDetails: SchoolDetails,
        attends: StaffAttendance[],
        reportTitle: string
    ): Observable<any> => {
        return this.reportSvc
            .loadImageAsBase64('assets/img/shule-nova-logo-only.png')
            .pipe(
                switchMap((blob) => this.readBlobAsBase64(blob)),
                map((base64data: string) => {
                    const dayHeaders = [
                        {text: 'Date', style: 'tableHeader'},
                        {text: 'Month', style: 'tableHeader'},
                        {text: 'Year', style: 'tableHeader'},
                        {text: 'Present', style: 'tableHeader'},
                        {text: 'Remarks', style: 'tableHeader'}
                    ];

                    const tableWidths = ['auto', '*', '*', '*', 'auto'];

                    const tableBody = [
                        [...dayHeaders],
                        ...attends.map((attend) => [
                            {
                                text: this.datePipe.transform(
                                    attend.date,
                                    'dd/MM/yyyy - EEEE'
                                ),
                                noWrap: true
                            },
                            {
                                text: this.datePipe.transform(
                                    attend.date,
                                    'MMMM'
                                ),
                                noWrap: true
                            },
                            {
                                text: this.datePipe.transform(
                                    attend.date,
                                    'yyyy'
                                ),
                                noWrap: true
                            },
                            {
                                text:
                                    attend.present === null
                                        ? ''
                                        : attend.present
                                          ? 'Present'
                                          : 'Absent',
                                noWrap: true
                            },
                            {
                                text: attend.remarks,
                                noWrap: false
                            }
                        ])
                    ];

                    const docDefinition = {
                        pageOrientation: 'portrait',
                        pageMargins: [20, 20, 20, 40],
                        pageSize: 'A4',
                        info: {
                            title: '',
                            author: '',
                            subject: ''
                        },
                        watermark: this.reportSvc.getWatermark(
                            'ShuleNova - ' + schoolDetails?.name
                        ),
                        footer: this.reportSvc.getFooter('portrait'),
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
                                columns: [
                                    [
                                        {
                                            text:
                                                'Staff No: ' +
                                                attends[0].staffDetails?.upi,
                                            alignment: 'left'
                                        }
                                    ],
                                    [
                                        {
                                            text:
                                                'Staff Name: ' +
                                                attends[0].staffDetails
                                                    ?.fullName,
                                            alignment: 'right'
                                        }
                                    ]
                                ],
                                bold: true,
                                color: '#002D62',
                                marginBottom: 1
                            },
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
                                marginBottom: 1,
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

                    return docDefinition;
                })
            );
    };

}
