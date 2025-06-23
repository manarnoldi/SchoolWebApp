import { AuthService } from '@/core/services/auth.service';
import { ResourceService } from '@/core/services/resource.service';
import { StudentAttendancesReport } from '@/reports/models/student-attendance-report';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReportsService } from '../reports.service';
import { Status } from '@/core/enums/status';
import { map, Observable } from 'rxjs';
import { SchoolDetails } from '@/school/models/school-details';

import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
(pdfMake as any).vfs = pdfFonts;

@Injectable({
  providedIn: 'root'
})
export class StudentsAttendanceReportService extends ResourceService<StudentAttendancesReport> {
    constructor(
        private http: HttpClient,
        private userSvc: AuthService,
        private reportSvc: ReportsService
    ) {
        super(http, StudentAttendancesReport);
    }

    public getStudentAttendancesReport = (
        month: number,
        schoolClassId: number,
        status: Status
    ): Observable<StudentAttendancesReport[]> => {
        let searchUrl = `/studentAttendances/getAttendanceReport/${month}/${schoolClassId}/${status}`;
        return this.get(searchUrl).pipe(
            map((studentAttendances) => studentAttendances)
        );
    };

    generateReport = (
        schoolDetails: SchoolDetails,
        attends: StudentAttendancesReport[],
        reportTitle: string
    ) => {
        this.reportSvc
            .loadImageAsBase64('assets/img/shule-nova-logo-only.png')
            .subscribe({
                next: (blob) => {
                    const reader = new FileReader();
                    reader.onloadend = () => {
                        const base64data: string = reader.result as string;
                        const dayHeaders = Array.from({length: 31}, (_, i) => ({
                            text: `${i + 1}`,
                            style: 'tableHeader'
                        }));
                        const tableWidths = ['auto', ...Array(31).fill('*')];
                        const tableBody = [
                            [
                                {text: 'Student Full Name', style: 'tableHeader'},
                                ...dayHeaders
                            ],
                            ...attends.map((attend) => [
                                {
                                    text: attend.student.fullName,
                                    noWrap: true
                                },
                                ...Array.from(
                                    {length: 31},
                                    (_, i) => attend[`day${i + 1}`]
                                )
                            ])
                        ];

                        const docDefinition = {
                            pageOrientation: 'landscape',
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
