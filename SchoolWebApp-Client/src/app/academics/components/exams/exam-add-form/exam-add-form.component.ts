import {Curriculum} from '@/academics/models/curriculum';
import {Exam} from '@/academics/models/exam';
import {ExamType} from '@/academics/models/exam-type';
import {Subject} from '@/academics/models/subject';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {EducationLevelSubjectService} from '@/academics/services/education-level-subject.service';
import {ExamTypesService} from '@/academics/services/exam-types.service';
import {ExamsService} from '@/academics/services/exams.service';
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
import {DatePipe} from '@angular/common';
import {Component, Input, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';

import {ToastrService} from 'ngx-toastr';
import {catchError, forkJoin, map, Observable, of} from 'rxjs';
import Swal from 'sweetalert2';

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
    examId: number;
    eduLevelId: number;
    exam: Exam;
    editMode = false;
    contributingMarksError: string[] = [];

    constructor(
        private formBuilder: FormBuilder,
        private sessionSvc: SessionsService,
        private curriculaSvc: CurriculumService,
        private academicYearsSvc: AcademicYearsService,
        private examTypesSvc: ExamTypesService,
        private examsSvc: ExamsService,
        private subjectsSvc: SubjectsService,
        private router: Router,
        private route: ActivatedRoute,
        private educationLevelSubjectSvc: EducationLevelSubjectService,
        private schoolClassesSvc: SchoolClassesService,
        private educationLevelSvc: EducationLevelService,
        private toastr: ToastrService,
        private datePipe: DatePipe
    ) {}

    ngOnInit(): void {
        this.initializeForm();
        this.refreshItems();
    }
    refreshItems() {
        this.route.queryParams.subscribe((params) => {
            this.examId = parseInt(params['id']);
            this.eduLevelId = parseInt(params['eduLevelId']);

            let curriculaReq = this.curriculaSvc.get('/curricula');
            let academicYearsReq = this.academicYearsSvc.get('/academicYears');
            let examTypesReq = this.examTypesSvc.get('/examTypes');
            let educationLevelReq =
                this.educationLevelSvc.get('/educationLevels');
            let examsReq = this.examsSvc.getById(this.examId, '/exams');

            forkJoin([
                curriculaReq,
                academicYearsReq,
                examTypesReq,
                educationLevelReq,
                this.examId && this.examId > 0 ? examsReq : of(null)
            ]).subscribe(
                ([
                    curricula,
                    academicYears,
                    examTypes,
                    educationLevels,
                    exam
                ]) => {
                    this.curricula = curricula.sort((a, b) => a.rank - b.rank);
                    this.academicYears = academicYears.sort(
                        (a, b) => b.rank - a.rank
                    );
                    this.examTypes = examTypes;
                    this.educationLevels = educationLevels.sort(
                        (a, b) => a.rank - b.rank
                    );

                    if (this.examId && this.examId > 0) {
                        this.exam = exam;
                        this.editMode = true;
                        this.subjects = [];
                        this.schoolClasses = [];

                        let subjectsReq = this.subjectsSvc.getById(
                            exam.subjectId,
                            '/subjects'
                        );
                        let schoolClassReq = this.schoolClassesSvc.getById(
                            exam.schoolClassId,
                            '/schoolClasses'
                        );
                        let sessionsReq = this.sessionSvc.get('/sessions');

                        forkJoin([
                            subjectsReq,
                            schoolClassReq,
                            sessionsReq
                        ]).subscribe({
                            next: ([subject, schoolClass, sessions]) => {
                                subject.isSelected = true;
                                schoolClass.isSelected = true;
                                this.subjects.push(subject);
                                this.schoolClasses.push(schoolClass);
                                this.setFormControls(exam);
                                this.sessions = sessions;
                                this.isLoading = true;
                            },
                            error: (err) => {
                                this.toastr.error(err.error);
                            }
                        });
                    }
                    this.isLoading = true;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    }

    checkIfContributingMarkIsOk = (
        examToBeSaved: Exam,
        currentContributingMark: number,
        educationLevelId: number,
        prevContributingMark: number
    ): Observable<string> => {
        let httpUrl = `/exams/examSearch?academicYearId=${this.sessions.find((s) => s.id == examToBeSaved.sessionId.toString()).academicYearId}`;
        httpUrl += `&curriculumId=${this.sessions.find((s) => s.id == examToBeSaved.sessionId.toString()).curriculumId}`;
        httpUrl += `&sessionId=${examToBeSaved.sessionId}`;
        httpUrl += `&schoolClassId=${examToBeSaved.schoolClassId ?? ''}`;
        httpUrl += `&subjectId=${examToBeSaved.subjectId ?? ''}`;
        httpUrl += `&examTypeId=${examToBeSaved.examTypeId ?? ''}`;
        return this.examsSvc.get(httpUrl).pipe(
            map((examsFound) => {
                let totalContrMark = 0;
                let contributingMarksError: string = ''; // Create a local array instead of using `this.contributingMarksError`

                examsFound.forEach((ef) => {
                    totalContrMark += ef.contributingMark;
                });
                if (this.editMode) {
                    totalContrMark =
                        totalContrMark -
                        prevContributingMark +
                        currentContributingMark;
                } else {
                    totalContrMark += currentContributingMark;
                }
                if (totalContrMark > 100) {
                    contributingMarksError = `
                        Exam: ${examToBeSaved.name} of type: ${this.examTypes.find((E) => E.id == examToBeSaved.examTypeId.toString()).name}, 
                        Class: ${this.schoolClasses.find((E) => E.id == examToBeSaved.schoolClassId.toString()).name}, 
                        Subject: ${this.subjects.find((s) => (s.id = examToBeSaved.subjectId.toString())).name}, 
                        Curriculum: ${this.sessions.find((c) => c.id == examToBeSaved.sessionId.toString()).curriculum?.name}, 
                        Year: ${this.sessions.find((c) => c.id == examToBeSaved.sessionId.toString()).academicYear?.name}, 
                        Session: ${this.sessions.find((c) => c.id == examToBeSaved.sessionId.toString()).sessionName}, 
                        Education level: ${this.educationLevels.find((e) => e.id == educationLevelId.toString())?.name} 
                        contributing marks will be ${totalContrMark}!<br>`;
                }
                return contributingMarksError; // Return the array
            }),
            catchError((err) => {
                this.toastr.error(err.error);
                return of(''); // Return an empty array in case of error
            })
        );
    };

    setFormControls = (exam: Exam) => {
        this.examsAddForm.setValue({
            academicYearId: exam?.session?.academicYearId ?? null,
            sessionId: exam?.session?.id ?? null,
            curriculumId: exam?.session?.curriculumId ?? null,
            educationLevelId: this.eduLevelId ?? null,
            examTypeId: exam?.examType.id ?? null,
            examMark: exam?.examMark ?? null,
            contributingMark: exam?.contributingMark ?? null,
            name: exam?.name ?? null,
            otherDetails: exam?.otherDetails ?? null,
            examStartDate: this.exam?.examStartDate
                ? this.datePipe.transform(
                      this.exam?.examStartDate,
                      'yyyy-MM-dd'
                  )
                : null,
            examEndDate: this.exam?.examEndDate
                ? this.datePipe.transform(this.exam?.examEndDate, 'yyyy-MM-dd')
                : null,
            examMarkEntryEndDate: this.exam?.examMarkEntryEndDate
                ? this.datePipe.transform(
                      this.exam?.examMarkEntryEndDate,
                      'yyyy-MM-dd'
                  )
                : null
        });
    };

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
            otherDetails: [''],
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

    curriculumYearChanged = (curriculumId: number ,academicYearId: number) => {
        let sessionsReq = this.sessionSvc.getByCurriculumYear(
            curriculumId,
            academicYearId
        );

        forkJoin([sessionsReq]).subscribe(
            ([sessions]) => {
                this.sessions = sessions.sort((a, b) => a.rank - b.rank);
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    };

    academicYearCurriculumChanged = () => {
        this.subjects = [];
        this.schoolClasses = [];
        this.sessions = [];
        this.examsAddForm.get('educationLevelId').reset();
        let curriculumId = this.examsAddForm.get('curriculumId').value;
        let academicYearId = this.examsAddForm.get('academicYearId').value;

        if (
            !curriculumId ||
            curriculumId == null ||
            !academicYearId ||
            academicYearId == null
        ) {
            return;
        }
        this.curriculumYearChanged(curriculumId,academicYearId);
    };

    educationLevelChanged = () => {
        this.subjects = [];
        this.schoolClasses = [];
        if (
            !this.examsAddForm.get('educationLevelId').value ||
            !this.examsAddForm.get('academicYearId').value
        ) {
            return;
        }
        let eduLevelId = this.examsAddForm.get('educationLevelId').value;
        let acadYearId = this.examsAddForm.get('academicYearId').value;

        let educationLevelSubjectsReq = this.educationLevelSubjectSvc.get(
            '/educationLevelSubjects/byEducationLevelYearId/' +
                eduLevelId +
                '/' +
                acadYearId
        );
        let schoolClassReq = this.schoolClassesSvc.getByEducationLevelandYear(
            eduLevelId,
            acadYearId
        );

        forkJoin([educationLevelSubjectsReq, schoolClassReq]).subscribe({
            next: ([educationLevelSubjects, schoolClasses]) => {
                educationLevelSubjects.forEach((els) => {
                    this.subjects.push(els.subject);
                });
                this.subjects.sort((a, b) => a.rank - b.rank);
                this.schoolClasses = schoolClasses.sort(
                    (a, b) => a.rank - b.rank
                );
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    get f() {
        return this.examsAddForm.controls;
    }

    updateExam = (exam: Exam, educationLevelId: number) => {
        this.examsSvc.getById(parseInt(exam.id), '/exams').subscribe({
            next: (exm) => {
                this.checkIfContributingMarkIsOk(
                    exam,
                    exam.contributingMark,
                    educationLevelId,
                    exm.contributingMark
                ).subscribe({
                    next: (contributingMarksResults) => {
                        if (
                            contributingMarksResults &&
                            contributingMarksResults != '' &&
                            contributingMarksResults.length > 0
                        ) {
                            this.toastr.error(
                                'Exam type contribution marks greater than 100%!<br>' +
                                    contributingMarksResults.toString(),
                                'Contribution marks exceeded 100% error!',
                                {
                                    disableTimeOut: true,
                                    positionClass: 'toast-top-full-width',
                                    enableHtml: true
                                }
                            );
                            return;
                        }
                        this.examsSvc.update('/exams', exam).subscribe({
                            next: (res) => {
                                this.toastr.success(
                                    'Exam information updated successfully!'
                                );

                                this.examsAddForm.reset();
                                this.editMode = false;
                                this.schoolClasses = [];
                                this.subjects = [];
                                this.router.navigateByUrl(
                                    '/academics/exams?id=' +
                                        this.examId +
                                        '&eduLevelId=' +
                                        this.eduLevelId
                                );
                            },
                            error: (err) => {
                                this.toastr.error(err.error);
                            }
                        });
                    },
                    error: (err) => {
                        this.toastr.error(err.error);
                    }
                });
            },
            error: (err) => {
                this.toastr.error(err.error);
            }
        });
    };

    onSubmit = () => {
        if (!this.schoolClasses.some((sc) => sc.isSelected)) {
            this.toastr.error('There is no class selected!');
            return;
        } else if (!this.subjects.some((sc) => sc.isSelected)) {
            this.toastr.error('There is no subject selected!');
            return;
        }
        Swal.fire({
            title: `${this.editMode ? 'Update Examination' : 'Add examinations'}?`,
            text: `Confirm if you want to ${this.editMode ? 'update examination' : 'add examinations'}.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.editMode ? 'Update Examination' : 'Add Examinations'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let academicYearId =
                    this.examsAddForm.get('academicYearId').value;
                let educationLevelId =
                    this.examsAddForm.get('educationLevelId').value;
                let curriculumId = this.examsAddForm.get('curriculumId').value;
                let sessionId = this.examsAddForm.get('sessionId').value;
                let examTypeId = this.examsAddForm.get('examTypeId').value;
                let examName = this.examsAddForm.get('name').value;
                let contributingMark =
                    this.examsAddForm.get('contributingMark').value;
                let examStartDate =
                    this.examsAddForm.get('examStartDate').value;
                let examEndDate = this.examsAddForm.get('examEndDate').value;
                let examMarkEntryEndDate = this.examsAddForm.get(
                    'examMarkEntryEndDate'
                ).value;
                let examMark = this.examsAddForm.get('examMark').value;
                let otherDetails = this.examsAddForm.get('otherDetails').value;

                if (this.editMode) {
                    this.exam.examMark = examMark;
                    this.exam.contributingMark = contributingMark;
                    this.exam.examStartDate = examStartDate;
                    this.exam.examEndDate = examEndDate;
                    this.exam.examMarkEntryEndDate = examMarkEntryEndDate;
                    this.exam.otherDetails = otherDetails;
                    this.updateExam(this.exam, educationLevelId);
                    return;
                }

                let recordExistsReq = [];
                let validateContributingMarkReq = [];

                let reqToProcess = [];
                let toSaveItem = new Exam();
                toSaveItem.contributingMark = contributingMark;
                toSaveItem.examEndDate = examEndDate;
                toSaveItem.examMark = examMark;
                toSaveItem.examStartDate = examStartDate;
                toSaveItem.examMarkEntryEndDate = examMarkEntryEndDate;
                toSaveItem.examTypeId = examTypeId;
                toSaveItem.name = examName;
                toSaveItem.otherDetails = otherDetails;
                toSaveItem.sessionId = sessionId;
                this.contributingMarksError = [];
                this.subjects
                    .filter((s) => s.isSelected)
                    .forEach((s) => {
                        this.schoolClasses
                            .filter((s) => s.isSelected)
                            .forEach((sc) => {
                                toSaveItem.subjectId = parseInt(s.id);
                                toSaveItem.schoolClassId = parseInt(sc.id);
                                recordExistsReq.push(
                                    this.examsSvc.get(
                                        `/exams/examSearch?academicYearId=${academicYearId}&curriculumId=${curriculumId}&sessionId=
                    ${sessionId}&schoolClassId=${sc.id ?? ''}&subjectId=${s.id ?? ''}&examTypeId=${examTypeId ?? ''}&examName=${examName ?? ''}`
                                    )
                                );
                                validateContributingMarkReq.push(
                                    this.checkIfContributingMarkIsOk(
                                        toSaveItem,
                                        contributingMark,
                                        educationLevelId,
                                        contributingMark
                                    )
                                );
                                reqToProcess.push(
                                    this.examsSvc.create('/exams', toSaveItem)
                                );
                            });
                    });

                forkJoin(validateContributingMarkReq).subscribe({
                    next: (contributingMarksResults) => {
                        let errorInContrMarkFound = false;

                        contributingMarksResults.forEach((cm) => {
                            if (cm && cm != '' && cm.length > 0) {
                                errorInContrMarkFound = true;
                            }
                        });

                        if (errorInContrMarkFound) {
                            this.toastr.error(
                                'Exam type contribution marks greater than 100%!<br>' +
                                    contributingMarksResults.toString(),
                                'Contribution marks exceeded 100% error!',
                                {
                                    disableTimeOut: true,
                                    positionClass: 'toast-top-full-width',
                                    enableHtml: true
                                }
                            );
                            return;
                        }

                        forkJoin(recordExistsReq).subscribe({
                            next: (existsResults) => {
                                let itemExists = false;
                                let itemExistsError = '';

                                existsResults.forEach((element) => {
                                    if (element.length > 0) {
                                        itemExists = true;
                                        itemExistsError += `Exam: ${examName} of type: ${element[0]?.examType?.name} 
                                        ,Class: ${element[0]?.schoolClass?.name}, Subject:${element[0]?.subject?.name} 
                                        ,Curiculum: ${this.curricula.find((c) => c.id == curriculumId).name}, Year: 
                                        ${this.academicYears.find((y) => y.id == academicYearId).name}
                                        ,Session: ${element[0]?.session?.sessionName}, Education level: 
                                        ${this.educationLevels.find((e) => e.id == educationLevelId).name} alreay exists!<br>`;
                                    }
                                });

                                if (itemExists) {
                                    this.toastr.error(
                                        'Some records already exist!<br>' +
                                            itemExistsError,
                                        'Items Exist in database error!',
                                        {
                                            disableTimeOut: true,
                                            positionClass:
                                                'toast-top-full-width',
                                            enableHtml: true
                                        }
                                    );
                                    return;
                                }
                                forkJoin(reqToProcess).subscribe({
                                    next: (res) => {
                                        this.toastr.success(
                                            'Examinations saved successfully.'
                                        );
                                        this.examsAddForm.get('name').reset();
                                        this.examsAddForm
                                            .get('contributingMark')
                                            .reset();
                                        this.examsAddForm
                                            .get('examMark')
                                            .reset();
                                    },
                                    error: (err) => {
                                        this.toastr.error(err.error);
                                    }
                                });
                            },
                            error: (err) => {
                                this.toastr.error(err.error);
                            }
                        });
                    },
                    error: (err) => {
                        this.toastr.error(err.error);
                    }
                });
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };
}
