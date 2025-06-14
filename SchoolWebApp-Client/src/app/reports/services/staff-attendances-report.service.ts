import {Injectable} from '@angular/core';
import {StaffAttendancesReport} from '../models/staff-attendances-report';
import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {map, Observable} from 'rxjs';
import {SchoolDetailsService} from '@/school/services/school-details.service';
import {SchoolDetails} from '@/school/models/school-details';
import pdfMake from 'pdfmake/build/pdfmake';
import pdfFonts from 'pdfmake/build/vfs_fonts';
import {UsersService} from '@/core/services/users.service';
import {AuthService} from '@/core/services/auth.service';
(pdfMake as any).vfs = pdfFonts;

@Injectable({
    providedIn: 'root'
})
export class StaffAttendancesReportService extends ResourceService<StaffAttendancesReport> {
    constructor(
        private http: HttpClient,
        private userSvc: AuthService
    ) {
        super(http, StaffAttendancesReport);
    }

    public getBase64ImageFromAsset(imagePath: string): Promise<string> {
        return this.http
            .get(imagePath, {responseType: 'blob'})
            .toPromise()
            .then((blob) => {
                return this.convertBlobToBase64(blob);
            });
    }

    private convertBlobToBase64(blob: Blob): Promise<string> {
        return new Promise<string>((resolve, reject) => {
            const reader = new FileReader();
            reader.onerror = reject;
            reader.onload = () => {
                resolve(reader.result as string);
            };
            reader.readAsDataURL(blob);
        });
    }

    public getStaffAttendancesReport = (
        month: number,
        year: number,
        staffCategoryId: number
    ): Observable<StaffAttendancesReport[]> => {
        let searchUrl = `/staffAttendances/getAttendanceReport/${month}/${year}/${staffCategoryId}`;
        return this.get(searchUrl).pipe(
            map((staffAttendances) => staffAttendances)
        );
    };

    loadImageAsBase64(
        schoolDetails: SchoolDetails,
        attendances: StaffAttendancesReport[],
        reportTitle: string
    ): void {
        this.http
            .get('assets/img/shule-nova-logo-only.png', {responseType: 'blob'})
            .subscribe((blob) => {
                const reader = new FileReader();
                reader.onloadend = () => {
                    const base64data = reader.result as string;
                    this.generateReport(
                        schoolDetails,
                        attendances,
                        base64data,
                        reportTitle
                    );
                };
                reader.readAsDataURL(blob);
            });
    }

    generateReport = (
        schoolDetails: SchoolDetails,
        attends: StaffAttendancesReport[],
        base64Image: string,
        reportTitle: string
    ) => {
        const DIVIDER = {
            table: {
                headerRows: 1,
                widths: ['100%'],
                body: [[''], ['']]
            },
            layout: 'headerLineOnly',
            marginBottom: 2,
            color: '#002D62'
        };

        const HEADER_STYLE = {
            bold: true,
            fontSize: 10,
            color: 'white',
            fillColor: '#4169E1',
            alignment: 'center'
        };

        const tableLayout = {
            hLineWidth: () => 0.1,
            vLineWidth: () => 0.1,
            hLineColor: () => '#4169E1',
            vLineColor: () => '#4169E1',
            paddingLeft: () => 5,
            paddingRight: () => 5,
            paddingTop: () => 5,
            paddingBottom: () => 5
        };

        // Generate headers for days 1–31
        const dayHeaders = Array.from({length: 31}, (_, i) => ({
            text: `${i + 1}`,
            style: 'tableHeader'
        }));

        // Generate widths: first column 'auto', rest '*'
        const tableWidths = ['auto', ...Array(31).fill('*')];

        const tableBody = [
            [{text: 'Staff Full Name', style: 'tableHeader'}, ...dayHeaders],
            ...attends.map((attend) => [
                {text: attend.staffDetail.fullName, noWrap: true},
                ...Array.from({length: 31}, (_, i) => attend[`day${i + 1}`])
            ])
        ];

        const docDefinition = {
            pageOrientation: 'landscape',
            pageMargins: [5, 5, 5, 5],
            info: {
                title: '',
                author: '',
                subject: ''
            },
            watermark: {
                text: 'ShuleNova - ' + schoolDetails?.name,
                color: 'blue',
                opacity: 0.1,
                bold: true,
                italics: false,
                angle: -45
            },
            footer: {
                text: 'This is a system generated document.',
                alignment: 'center',
                fontSize: 10,
                color: '#002D62'
            },
            images: {
                systemLogo: base64Image,
                schoolLogo: schoolDetails.logoAsBase64
            },
            styles: {
                tableHeader: HEADER_STYLE
            },
            content: [
                {...DIVIDER},
                {
                    columns: [
                        {image: 'schoolLogo', width: 70},
                        [
                            {
                                text: schoolDetails?.name?.toUpperCase(),
                                bold: true,
                                fontSize: 12,
                                alignment: 'center',
                                marginBottom: 2
                            },
                            {
                                text: schoolDetails?.address,
                                alignment: 'center',
                                marginBottom: 2
                            },
                            {
                                text: schoolDetails?.website,
                                alignment: 'center',
                                marginBottom: 2
                            },
                            {
                                text: schoolDetails?.telephone,
                                alignment: 'center',
                                marginBottom: 2
                            }
                        ],
                        {image: 'systemLogo', width: 70, alignment: 'right'}
                    ],
                    color: '#002D62'
                },
                {...DIVIDER, marginBottom: 1},
                {
                    columns: [
                        [
                            {
                                text: reportTitle,
                                alignment: 'center',
                                fontSize: 13,
                                marginBottom: 1,
                                bold: true,
                                color: '#002D62'
                            }
                        ]
                    ]
                },
                {...DIVIDER, marginBottom: 1},
                {
                    layout: tableLayout,
                    table: {
                        headerRows: 1,
                        widths: tableWidths,
                        body: tableBody
                    },
                    marginBottom: 2,
                    color: '#002D62',
                    fontSize: 10
                },
                {...DIVIDER},
                {
                    columns: [
                        [
                            {
                                text:
                                    'Print by: ' +
                                    this.userSvc?.currentUser?.userName,
                                alignment: 'left'
                            }
                        ],
                        [
                            {
                                text: `Print date: ${new Date().toLocaleString('en-GB')}`,
                                alignment: 'right'
                            }
                        ]
                    ],
                    fontSize: 10,
                    color: '#002D62'
                }
            ]
        };

        pdfMake.createPdf(docDefinition).open();
    };
}
