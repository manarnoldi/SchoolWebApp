import {Curriculum} from '@/academics/models/curriculum';
import {CurriculumYear} from '@/academics/models/curriculum-year';
import {ExamType} from '@/academics/models/exam-type';
import {Subject} from '@/academics/models/subject';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {ExamTypesService} from '@/academics/services/exam-types.service';
import {SubjectsService} from '@/academics/services/subjects.service';
import {SchoolClass} from '@/class/models/school-class';
import {Session} from '@/class/models/session';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {SessionsService} from '@/class/services/sessions.service';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import {AcademicYearsService} from '@/school/services/academic-years.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {Component, Input, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-exam-add-form',
    templateUrl: './exam-add-form.component.html',
    styleUrl: './exam-add-form.component.scss'
})
export class ExamAddFormComponent implements OnInit {
    academicYears: AcademicYear[] = [];
    curricula: Curriculum[] = [];
    examTypes: ExamType[] = [];
    sessions: Session[] = [];
    subjects: Subject[] = [];
    educationLevels: EducationLevel[] = [];
    schoolClasses: SchoolClass[] = [];
    isLoading: boolean = false;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/academics/exams'], title: 'Exams List'},
        {link: ['/academics/exams/add'], title: 'Add Exams'}
    ];
    dashboardTitle = 'Academics: Add Exams';
    examsAddForm: FormGroup;

    constructor(
        private formBuilder: FormBuilder,
        private sessionSvc: SessionsService,
        private curriculaSvc: CurriculumService,
        private academicYearsSvc: AcademicYearsService,
        private examTypesSvc: ExamTypesService,
        private subjectsSvc: SubjectsService,
        private schoolClassesSvc: SchoolClassesService,
        private educationLevelSvc: EducationLevelService,
        private toastr: ToastrService
    ) {}

    ngOnInit(): void {
        this.initializeForm();
        this.refreshItems();
    }
    refreshItems() {
        let curriculaReq = this.curriculaSvc.get('/curricula');
        let academicYearsReq = this.academicYearsSvc.get('/academicYears');
        let examTypesReq = this.examTypesSvc.get('/examTypes');
        let educationLevelReq = this.educationLevelSvc.get('/educationLevels');

        forkJoin([
            curriculaReq,
            academicYearsReq,
            examTypesReq,
            educationLevelReq
        ]).subscribe(
            ([curricula, academicYears, examTypes, educationLevels]) => {
                this.curricula = curricula;
                this.academicYears = academicYears;
                this.examTypes = examTypes;
                this.educationLevels = educationLevels.sort(
                    (a, b) => a.rank - b.rank
                );
                this.isLoading = true;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    }

    initializeForm = () => {
        this.examsAddForm = this.formBuilder.group({
            academicYearId: [null, Validators.required],
            sessionId: [null, Validators.required],
            curriculumId: [null, Validators.required],
            educationLevelId: [null, Validators.required],
            examTypeId: [null, Validators.required],
            examMark: ['', Validators.required],
            contributingMark: ['', Validators.required],
            name: ['', Validators.required],
            examStartDate: [
                new Date().toISOString().substring(0, 10),
                Validators.required
            ],
            examEndDate: [
                new Date().toISOString().substring(0, 10),
                Validators.required
            ],
            examMarkEntryEndDate: [
                new Date().toISOString().substring(0, 10),
                Validators.required
            ]
        });
    };

    curriculumYearChanged = (cy: CurriculumYear) => {
        this.sessions = [];
        let sessionsReq = this.sessionSvc.get(
            `/sessions/byCurriculumYearId/${cy.curriculumId}/${cy.academicYearId}`
        );

        forkJoin([sessionsReq]).subscribe(
            ([sessions]) => {
                this.sessions = sessions;
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    };

    academicYearCurriculumChanged = () => {
        let curriculumYearId = this.examsAddForm.get('curriculumId').value;
        let academicYearId = this.examsAddForm.get('academicYearId').value;

        if (
            !curriculumYearId ||
            curriculumYearId == null ||
            !academicYearId ||
            academicYearId == null
        ) {
            return;
        }
        const curriculumYear = new CurriculumYear(
            curriculumYearId,
            academicYearId
        );
        this.curriculumYearChanged(curriculumYear);
    };

    get f() {
        return this.examsAddForm.controls;
    }

    // sessionExamTypeChanged = () => {
    //     this.examsAddForm.patchValue({
    //         name: ''
    //     });
    //     let academicYearId = this.examsAddForm.get('academicYearId').value;
    //     let curriculumId = this.examsAddForm.get('curriculumId').value;
    //     let sessionId = this.examsAddForm.get('sessionId').value;
    //     let examTypeId = this.examsAddForm.get('examTypeId').value;

    //     if (!academicYearId) this.toastr.warning('Select academic year!');
    //     else if (!curriculumId) this.toastr.warning('Select curriculum!');
    //     else if (!sessionId) this.toastr.warning('Select session!');
    //     else if (!examTypeId) this.toastr.warning('Select exam type!');
    //     else {
    //       let examName = this.curricula.find(c=>c.id ==curriculumId).code +"-"+this.
    //         this.examsAddForm.patchValue({
    //             name: examName
    //         });
    //     }
    // };

    onSubmit = () => {};
}
