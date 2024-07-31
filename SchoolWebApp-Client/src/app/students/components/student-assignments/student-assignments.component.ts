import {Curriculum} from '@/academics/models/curriculum';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {LearningLevel} from '@/class/models/learning-level';
import {SchoolStream} from '@/class/models/school-stream';
import {LearningLevelsService} from '@/class/services/learning-levels.service';
import {SchoolStreamsService} from '@/class/services/school-streams.service';
import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {OccurenceType} from '@/settings/models/occurence-type';
import {Outcome} from '@/settings/models/outcome';
import {OccurenceTypeService} from '@/settings/services/occurence-type.service';
import {OutcomesService} from '@/settings/services/outcomes.service';
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

    curricula: Curriculum[];
    educationLevels: EducationLevel[];
    outcomes: Outcome[] = [];
    occurenceTypes: OccurenceType[] = [];

    academicYears: AcademicYear[];
    learningLevels: LearningLevel[];
    schoolStreams: SchoolStream[];
    breadcrumbs: BreadCrumb[];

    dashboardTitle = 'Student details';
    backLinkUrl: string;
    status = Status;
    statuses;
    activeNav: string = 'formerSchool';

    constructor(
        private toastr: ToastrService,
        private studentsSvc: StudentDetailsService,
        private route: ActivatedRoute,
        private currciulumSvc: CurriculumService,
        private educationLevelsSvc: EducationLevelService,
        private academicYrsSvc: AcademicYearsService,
        private learningLvlsSvc: LearningLevelsService,
        private schoolStreamsSvc: SchoolStreamsService,
        private outcomesSvc: OutcomesService,
        private occurenceTypesSvc: OccurenceTypeService
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
            this.activeNav = params['activeNav']
                ? params['activeNav']
                : 'formerSchool';
            this.breadcrumbs = [
                {link: ['/'], title: 'Home'},
                {link: ['/students/' + this.sourceLink], title: 'Students'},
                {
                    link: [
                        '/students/' +
                            this.sourceLink +
                            '/add?id=' +
                            this.studentId +
                            '&action=' +
                            this.sourceLink
                    ],
                    title: 'Students: ' + this.sourceLink
                }
            ];
            this.backLinkUrl = '/students/' + this.sourceLink;
            let studentByIdReq = this.studentsSvc.getById(
                this.studentId,
                '/students'
            );
            let curriculaReq = this.currciulumSvc.get('/curricula');
            let educationLevelsReq =
                this.educationLevelsSvc.get('/educationLevels');
            let academicYrsReq = this.academicYrsSvc.get('/academicYears');
            let learningLvlsReq = this.learningLvlsSvc.get('/learningLevels');
            let schoolStreamsReq = this.schoolStreamsSvc.get('/schoolStreams');
            let outcomesReq = this.outcomesSvc.get('/outcomes');
            let occurenceTypesReq =
                this.occurenceTypesSvc.get('/occurenceTypes');

            forkJoin([
                studentByIdReq,
                curriculaReq,
                educationLevelsReq,
                academicYrsReq,
                learningLvlsReq,
                schoolStreamsReq,
                outcomesReq,
                occurenceTypesReq
            ]).subscribe(
                ([
                    student,
                    curricula,
                    educationLevels,
                    academicYears,
                    learningLevels,
                    schoolStreams,
                    outcomes,
                    occurenceTypes
                ]) => {
                    this.student = student;
                    this.curricula = curricula;
                    this.educationLevels = educationLevels;
                    this.academicYears = academicYears;
                    this.learningLevels = learningLevels;
                    this.schoolStreams = schoolStreams;
                    this.outcomes = outcomes;
                    this.occurenceTypes = occurenceTypes;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };
}
