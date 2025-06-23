import { AuthService } from '@/core/services/auth.service';
import { ResourceService } from '@/core/services/resource.service';
import { StudentAttendance } from '@/students/models/student-attendance';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { ReportsService } from '../reports.service';
import { DatePipe } from '@angular/common';
import { StudentAttendancesReport } from '@/reports/models/student-attendance-report';
import { SchoolDetails } from '@/school/models/school-details';
import { concatMap, from, map, Observable, switchMap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class StudentsAttendanceReportDetailsService  extends ResourceService<StudentAttendance> {
    constructor(
        private http: HttpClient,
        private toastr: ToastrService,
        private userSvc: AuthService,
        private reportSvc: ReportsService,
        private datePipe: DatePipe
    ) {
        super(http, StudentAttendancesReport);
    }

    printByBatch = (
        studentAttendances: StudentAttendance[][],
        school: SchoolDetails,
        reportTitle: string
    ) => {
        const batchSize = 10;
        const total = studentAttendances.length;
        const batches = Math.ceil(total / batchSize);

        from(Array.from({length: batches}, (_, i) => i))
            .pipe(
                concatMap((batchIndex) => {
                    const start = batchIndex * batchSize;
                    const end = start + batchSize;
                    const batch = studentAttendances.slice(start, end);

                    const batchDocObservables = batch.map((studentAttendances) =>
                        this.generateReport(
                            school,
                            studentAttendances.sort(
                                (a, b) =>
                                    new Date(a.date).getTime() -
                                    new Date(b.date).getTime()
                            ),
                            reportTitle
                        )
                    );
                    return this.reportSvc.printBatchDocs(batchDocObservables);
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
        attends: StudentAttendance[],
        reportTitle: string
    ): Observable<any> => {
        return this.reportSvc
            .loadImageAsBase64('assets/img/shule-nova-logo-only.png')
            .pipe(
                switchMap((blob) => this.reportSvc.readBlobAsBase64(blob)),
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
                                                attends[0].studentClass?.student?.upi,
                                            alignment: 'left'
                                        }
                                    ],
                                    [
                                        {
                                            text:
                                                'Staff Name: ' +
                                                attends[0].studentClass?.student?.fullName,
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
