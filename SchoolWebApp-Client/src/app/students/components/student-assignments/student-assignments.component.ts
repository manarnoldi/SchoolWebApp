import {Curriculum} from '@/academics/models/curriculum';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {EducationLevel} from '@/school/models/educationLevel';
import {EducationLevelService} from '@/school/services/education-level.service';
import {StudentDetails} from '@/students/models/student-details';
import {StudentDetailsService} from '@/students/services/student-details.service';
import {Component, Input, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-student-assignments',
    templateUrl: './student-assignments.component.html',
    styleUrl: './student-assignments.component.scss'
})
export class StudentAssignmentsComponent implements OnInit {
    studentId: number = 0;
    sourceLink: string = '';
    student: StudentDetails;

    curricula: Curriculum[] = [];
    educationLevels: EducationLevel[] = [];

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {
            link: ['/staff/' + this.sourceLink],
            title: 'Staff: ' + this.sourceLink
        }
    ];

    dashboardTitle = 'Staff ' + this.sourceLink;
    backLinkUrl: string;
    status = Status;
    statuses;

    constructor(
        private toastr: ToastrService,
        private studentsSvc: StudentDetailsService,
        private route: ActivatedRoute,
        private currciulumSvc: CurriculumService,
        private educationLevelsSvc: EducationLevelService
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
            let curriculaReq = this.currciulumSvc.get('/curricula');
            let educationLevelsReq =
                this.educationLevelsSvc.get('/educationLevels');

            forkJoin([
                studentByIdReq,
                curriculaReq,
                educationLevelsReq
            ]).subscribe(
                ([student, curricula, educationLevels]) => {
                    this.student = student;
                    this.curricula = curricula;
                    this.educationLevels = educationLevels;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };
}
