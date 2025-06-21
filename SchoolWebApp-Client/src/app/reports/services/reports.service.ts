import {SchoolDetails} from '@/school/models/school-details';
import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {map, Observable} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ReportsService {
    constructor(private http: HttpClient) {}

    public getWatermark = (waterMarkTitle: string) => {
        return {
            text: waterMarkTitle,
            color: 'blue',
            opacity: 0.1,
            bold: true,
            italics: false,
            angle: -45
        };
    };

    public getFooter = (pageOrientation: string) => {
        return (currentPage: number, pageCount: number) => ({
            margin: [20, 10], // [left, top] adjust as needed
            stack: [
                // 1️⃣ Canvas line
                {
                    canvas: [
                        {
                            type: 'line',
                            x1: 0,
                            y1: 0,
                            x2:
                                pageOrientation.toLowerCase() == 'landscape'
                                    ? 800
                                    : 555,
                            y2: 0, // width depends on page size (A4 landscape: ~800, portrait: ~515)
                            lineWidth: 1,
                            lineColor: '#002D62'
                        }
                    ]
                },
                // 2️⃣ Footer content (columns)
                {
                    columns: [
                        {
                            text: 'This is a system generated document.',
                            alignment: 'left',
                            fontSize: 10,
                            color: '#002D62'
                        },
                        {
                            text: `Page ${currentPage} of ${pageCount}`,
                            alignment: 'right',
                            fontSize: 10,
                            color: '#002D62'
                        }
                    ]
                }
            ]
        });
    };

    public getDIVIDER = () => {
        return {
            table: {
                headerRows: 1,
                widths: ['100%'],
                body: [[''], ['']]
            },
            layout: 'headerLineOnly',
            marginBottom: 2,
            color: '#002D62'
        };
    };

    public getHEADER_STYLE = () => {
        return {
            bold: true,
            fontSize: 10,
            color: 'white',
            fillColor: '#4169E1',
            alignment: 'center'
        };
    };

    public getTableLayout = () => {
        return {
            hLineWidth: () => 0.1,
            vLineWidth: () => 0.1,
            hLineColor: () => '#4169E1',
            vLineColor: () => '#4169E1',
            paddingLeft: () => 5,
            paddingRight: () => 5,
            paddingTop: () => 5,
            paddingBottom: () => 5
        };
    };

    public getReportHeader = (schoolDetails: SchoolDetails) => {
        return {
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
        };
    };

    public getReportTitle = (reportTitle: string) => {
        return {
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
        };
    };

    public getPrintDetails = (printBy: string, printDate: string) => {
        return {
            columns: [
                [
                    {
                        text: 'Print by: ' + printBy,
                        alignment: 'left'
                    }
                ],
                [
                    {
                        text: `Print date: ${printDate}`,
                        alignment: 'right'
                    }
                ]
            ],
            fontSize: 10,
            color: '#002D62'
        };
    };

    public loadImageAsBase64(imgUrl: string): Observable<Blob> {
        return this.http
            .get(imgUrl, {responseType: 'blob'})
            .pipe(map((blob) => blob));
    }
}
