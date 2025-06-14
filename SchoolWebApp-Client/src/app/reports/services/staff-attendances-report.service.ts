import {Injectable} from '@angular/core';
import {StaffAttendancesReport} from '../models/staff-attendances-report';
import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {map, Observable} from 'rxjs';
import {SchoolDetails} from '@/school/models/school-details';
import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
import {AuthService} from '@/core/services/auth.service';
import {ReportsService} from './reports.service';
import {Status} from '@/core/enums/status';
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
                        const tableWidths = ['auto', ...Array(31).fill('*')];
                        const tableBody = [
                            [
                                {text: 'Staff Full Name', style: 'tableHeader'},
                                ...dayHeaders
                            ],
                            ...attends.map((attend) => [
                                {
                                    text: attend.staffDetail.fullName,
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
