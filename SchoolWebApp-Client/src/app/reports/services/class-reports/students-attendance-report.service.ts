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
                        // Name column fixed; day columns share the rest equally
                        // ('*') so all 31 days fit across A4 landscape and the
                        // table spans the full width with symmetric margins. With
                        // 'auto'/noWrap the long names pushed day 31 off the page.
                        const tableWidths = [110, ...Array(31).fill('*')];
                        // Tighter cell padding than the shared layout to leave
                        // room for 31 day columns.
                        const compactLayout = {
                            hLineWidth: () => 0.1,
                            vLineWidth: () => 0.1,
                            hLineColor: () => '#4169E1',
                            vLineColor: () => '#4169E1',
                            paddingLeft: () => 2,
                            paddingRight: () => 2,
                            paddingTop: () => 3,
                            paddingBottom: () => 3
                        };
                        const tableBody = [
                            [
                                {text: 'Student Full Name', style: 'tableHeader'},
                                ...dayHeaders
                            ],
                            ...attends.map((attend) => [
                                {
                                    text: attend.student.fullName
                                },
                                ...Array.from(
                                    {length: 31},
                                    (_, i) => ({text: attend[`day${i + 1}`] ?? '', alignment: 'center'})
                                )
                            ])
                        ];

                        const docDefinition = {
                            pageOrientation: 'landscape',
                            pageMargins: [20, 15, 20, 40],
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
                                {...this.reportSvc.getDIVIDER('landscape')},
                                this.reportSvc.getReportHeader(schoolDetails),
                                {
                                    ...this.reportSvc.getDIVIDER('landscape'),
                                    marginBottom: 1
                                },
                                this.reportSvc.getReportTitle(reportTitle),
                                {
                                    ...this.reportSvc.getDIVIDER('landscape'),
                                    marginTop: 5,
                                    marginBottom: 3
                                },
                                {
                                    layout: compactLayout,
                                    table: {
                                        headerRows: 1,
                                        widths: tableWidths,
                                        body: tableBody
                                    },
                                    marginBottom: 2,
                                    color: '#002D62',
                                    fontSize: 8
                                },
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
