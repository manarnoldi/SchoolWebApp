import {LearningLevel} from '@/class/models/learning-level';
import {SchoolStream} from '@/class/models/school-stream';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {SchoolStreamsService} from '@/class/services/school-streams.service';
import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {StudentDetails} from '@/students/models/student-details';
import {StudentDetailsService} from '@/students/services/student-details.service';
import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-student-class',
    templateUrl: './student-class.component.html',
    styleUrl: './student-class.component.scss'
})
export class StudentClassComponent implements OnInit {
    studentId: number = 0;
    sourceLink: string = '';
    student: StudentDetails;

    years: AcademicYear[] = [];
    learningLevels: LearningLevel[] = [];
    streams: SchoolStream[] = [];

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {
            link: ['/student/' + this.sourceLink],
            title: 'Student: ' + this.sourceLink
        }
    ];

    dashboardTitle = 'Student ' + this.sourceLink;
    backLinkUrl: string;
    status = Status;
    statuses;

    constructor(
        private toastr: ToastrService,
        private studentsSvc: StudentDetailsService,
        private route: ActivatedRoute,
        private yearsSvc: AcademicYearsService,
        private learningLevelsSvc: LearningLevelsService,
        private streamsSvc: SchoolStreamsService
    ) {
        this.statuses = Object.keys(this.status).filter((k) =>
            isNaN(Number(k))
        );
    }
    ngOnInit(): void {
        this.loadSelectedStudent();
    }

    loadSelectedStudent = () => {
        this.route.queryParams.subscribe((params) => {
            this.studentId = params['id'];
            this.sourceLink = params['action'];
            this.backLinkUrl = '/students/' + this.sourceLink;
            let studentByIdReq = this.studentsSvc.getById(
                this.studentId,
                '/students'
            );
            let yearsReq = this.yearsSvc.get('/academicYears');
            let learningLevelsReq =
                this.learningLevelsSvc.get('/learningLevels');
            let streamsReq = this.streamsSvc.get('/schoolStreams');

            forkJoin([
                studentByIdReq,
                yearsReq,
                learningLevelsReq,
                streamsReq
            ]).subscribe(
                ([student, years, learningLevels, streams]) => {
                    this.student = student;
                    this.years = years;
                    this.learningLevels = learningLevels;
                    this.streams = streams;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };
}
