import {AuthService} from '@/core/services/auth.service';
import {Injectable} from '@angular/core';
import {ReportsService} from '../reports.service';
import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {SchoolDetails} from '@/school/models/school-details';
import {concatMap, from, map, Observable, switchMap} from 'rxjs';
import {ToastrService} from 'ngx-toastr';
import {StudentSubject} from '@/students/models/student-subject';
import {StaffAttendancesReport} from '../../models/staff-attendances-report';

import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
(pdfMake as any).vfs = pdfFonts;

// Mirrors StaffSubjectDetailedReportService - same batch print pipeline + table
// layout, with student-specific headings ('Adm No:' / 'Student Name:') and
// columns derived from StudentSubject -> studentClass.schoolClass + subject.
@Injectable({
    providedIn: 'root'
})
export class StudentSubjectDetailedReportService extends ResourceService<StudentSubject> {
    constructor(
        private http: HttpClient,
        private toastr: ToastrService,
        private userSvc: AuthService,
        private reportSvc: ReportsService
    ) {
        super(http, StaffAttendancesReport);
    }

    printByBatch = (
        studentSubjects: StudentSubject[][],
        school: SchoolDetails,
        reportTitle: string
    ) => {
        const batchSize = 10;
        const total = studentSubjects.length;
        const batches = Math.ceil(total / batchSize);

        from(Array.from({length: batches}, (_, i) => i))
            .pipe(
                concatMap((batchIndex) => {
                    const start = batchIndex * batchSize;
                    const end = start + batchSize;
                    const batch = studentSubjects.slice(start, end);

                    const batchDocObservables = batch.map((ss) =>
                        this.generateReport(
                            school,
                            ss.sort(
                                (a, b) =>
                                    (a.studentClass?.schoolClass?.rank ?? 0) -
                                    (b.studentClass?.schoolClass?.rank ?? 0)
                            ),
                            reportTitle
                        )
                    );
                    return this.reportSvc.printBatchDocs(batchDocObservables);
                })
            )
            .subscribe({
                complete: () => {
                    console.log('✅ All student-subject batches generated & opened!');
                },
                error: (err) => {
                    this.toastr.error(err.error || err);
                }
            });
    };

    generateReport = (
        schoolDetails: SchoolDetails,
        studentSubjects: StudentSubject[],
        reportTitle: string
    ): Observable<any> => {
        return this.reportSvc
            .loadImageAsBase64('assets/img/shule-nova-logo-only.png')
            .pipe(
                switchMap((blob) => this.reportSvc.readBlobAsBase64(blob)),
                map((base64data: string) => {
                    const dayHeaders = [
                        {text: 'Class name', style: 'tableHeader'},
                        {text: 'Subject Code', style: 'tableHeader'},
                        {text: 'Subject Name', style: 'tableHeader'},
                        {text: 'Remarks', style: 'tableHeader'}
                    ];

                    // Star ratios always sum to the available page width, so
                    // the table can never overflow even when subject names
                    // are long. Subject Name + Remarks get more space; class
                    // + code are kept compact and won't wrap. Subject name
                    // and remarks are allowed to wrap onto multiple lines.
                    const tableWidths = ['*', 60, '*', '*'];
                    const tableBody = [
                        [...dayHeaders],
                        ...studentSubjects.map((ss) => [
                            {
                                text: ss.studentClass?.schoolClass?.name ?? '',
                                noWrap: true
                            },
                            {
                                text: ss?.subject?.code ?? '',
                                noWrap: true
                            },
                            {
                                text: ss?.subject?.name ?? ''
                            },
                            {
                                text: ss?.description ?? ''
                            }
                        ])
                    ];

                    const docDefinition = {
                        pageOrientation: 'portrait',
                        pageMargins: [20, 20, 20, 40],
                        pageSize: 'A4',
                        info: {title: '', author: '', subject: ''},
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
                                                'Adm No: ' +
                                                (studentSubjects[0]
                                                    ?.studentClass?.student
                                                    ?.upi ?? ''),
                                            alignment: 'left'
                                        }
                                    ],
                                    [
                                        {
                                            text:
                                                'Student Name: ' +
                                                (studentSubjects[0]
                                                    ?.studentClass?.student
                                                    ?.fullName ?? ''),
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
