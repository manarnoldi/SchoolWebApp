import {Injectable} from '@angular/core';
import {StaffAttendancesReport} from '../../models/staff-attendances-report';
import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {map, Observable} from 'rxjs';
import {SchoolDetails} from '@/school/models/school-details';
import {AuthService} from '@/core/services/auth.service';
import {ReportsService} from '../reports.service';
import { Status } from '@/core/enums/status';

import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
(pdfMake as any).vfs = pdfFonts;

@Injectable({
    providedIn: 'root'
})
export class StaffAttendancesReportService extends ResourceService<StaffAttendancesReport> {
    constructor(
        private http: HttpClient,
        private userSvc: AuthService,
        private reportSvc: ReportsService
    ) {
        super(http, StaffAttendancesReport);
    }

    public getStaffAttendancesReport = (
        month: number,
        year: number,
        staffCategoryId: number,
        status: Status
    ): Observable<StaffAttendancesReport[]> => {
        let searchUrl = `/staffAttendances/getAttendanceReport/${month}/${year}/${staffCategoryId}/${status}`;
        return this.get(searchUrl).pipe(
            map((staffAttendances) => staffAttendances)
        );
    };

    generateReport = (
        schoolDetails: SchoolDetails,
        attends: StaffAttendancesReport[],
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
                        // Fixed compact widths so all 31 day columns fit across
                        // an A4 landscape page: a capped (wrapping) name column +
                        // 31 narrow day columns. With 'auto'/'*' the long names
                        // forced the table past the page edge, dropping day 31.
                        // Name column fixed; day columns share the rest equally
                        // ('*') so the table spans the full content width and the
                        // left/right gaps stay symmetric. With the name capped,
                        // 31 star columns still fit comfortably.
                        const tableWidths = [110, ...Array(31).fill('*')];
                        // Tighter cell padding than the shared layout to claw back
                        // horizontal space for the day columns.
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
                                {text: 'Staff Full Name', style: 'tableHeader'},
                                ...dayHeaders
                            ],
                            ...attends.map((attend) => [
                                {
                                    text: attend.staffDetail.fullName
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
