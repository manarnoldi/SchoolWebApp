import {AuthService} from '@/core/services/auth.service';
import {ResourceService} from '@/core/services/resource.service';
import {StudentClass} from '@/students/models/student-class';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {ReportsService} from '../reports.service';
import {SchoolDetails} from '@/school/models/school-details';
import {concatMap, from, map, Observable, switchMap} from 'rxjs';
import {SchoolClass} from '@/class/models/school-class';
import {ClassLeadership} from '@/class/models/class-leadership';
import {ClassLeadershipsService} from '@/class/services/class-leaderships.service';

@Injectable({
    providedIn: 'root'
})
export class ClassListReportService extends ResourceService<StudentClass> {
    constructor(
        private http: HttpClient,
        private toastr: ToastrService,
        private userSvc: AuthService,
        private reportSvc: ReportsService,
        private classLeadershipsSvc: ClassLeadershipsService
    ) {
        super(http, StudentClass);
    }

    printByBatch = (
        studentClasses: StudentClass[][],
        school: SchoolDetails,
        reportTitle: string
    ) => {
        const batchSize = 10;
        const total = studentClasses.length;
        const batches = Math.ceil(total / batchSize);

        from(Array.from({length: batches}, (_, i) => i))
            .pipe(
                concatMap((batchIndex) => {
                    const start = batchIndex * batchSize;
                    const end = start + batchSize;
                    const batch = studentClasses.slice(start, end);

                    const batchDocObservables = batch.map((studentClass) => {
                        const sortedStudentClass = studentClass.sort((a, b) =>
                            a.student?.upi.localeCompare(b.student?.upi)
                        );
                        const classId = studentClass[0]?.schoolClass?.id;
                        return this.classLeadershipsSvc
                            .getBySchoolClassId(parseInt(classId))
                            .pipe(
                                concatMap((classLeaders) =>
                                    this.generateReport(
                                        school,
                                        sortedStudentClass,
                                        reportTitle,
                                        classLeaders.sort(
                                            (a, b) =>
                                                a.classLeadershipRole?.rank -
                                                b.classLeadershipRole?.rank
                                        )
                                    )
                                )
                            );
                    });
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
        studentClasses: StudentClass[],
        reportTitle: string,
        schoolClassLeaders: ClassLeadership[]
    ): Observable<any> => {
        return this.reportSvc
            .loadImageAsBase64('assets/img/shule-nova-logo-only.png')
            .pipe(
                switchMap((blob) => this.reportSvc.readBlobAsBase64(blob)),
                map((base64data: string) => {
                    const dayHeaders = [
                        {text: 'Adm No', style: 'tableHeader'},
                        {text: 'Student full name', style: 'tableHeader'},
                        {text: '', style: 'tableHeader'},
                        {text: '', style: 'tableHeader'},
                        {text: '', style: 'tableHeader'},
                        {text: '', style: 'tableHeader'}
                    ];

                    const tableWidths = ['auto', 'auto', '*', '*', '*', '*'];
                    const tableBody = [
                        [...dayHeaders],
                        ...studentClasses.map((studentClass) => [
                            {
                                text: studentClass.student?.upi,
                                noWrap: true
                            },
                            {
                                text: studentClass.student?.fullName,
                                noWrap: true
                            },
                            {
                                text: '',
                                noWrap: true
                            },
                            {
                                text: '',
                                noWrap: false
                            },
                            {
                                text: '',
                                noWrap: false
                            },
                            {
                                text: '',
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
                                        ...schoolClassLeaders.map((leader) => [
                                            {
                                                text:
                                                    leader.person?.fullName +
                                                    ' - [' +
                                                    leader.classLeadershipRole
                                                        ?.name +
                                                    ']',
                                                noWrap: true
                                            }
                                        ])
                                    ],
                                    [
                                        {
                                            text:
                                                'Class name: ' +
                                                studentClasses[0].schoolClass
                                                    ?.name,
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
