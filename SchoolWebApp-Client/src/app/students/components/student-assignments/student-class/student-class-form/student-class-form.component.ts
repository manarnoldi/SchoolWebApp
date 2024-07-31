import {LearningLevel} from '@/class/models/learning-level';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolStream} from '@/class/models/school-stream';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {AcademicYear} from '@/school/models/academic-year';
import {YearClassStreamComponent} from '@/shared/directives/year-class-stream/year-class-stream.component';
import {StudentClass} from '@/students/models/student-class';
import {StudentDetails} from '@/students/models/student-details';
import {
    AfterViewInit,
    Component,
    ElementRef,
    EventEmitter,
    Input,
    OnInit,
    Output,
    ViewChild
} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';

@Component({
    selector: 'app-student-class-form',
    templateUrl: './student-class-form.component.html',
    styleUrl: './student-class-form.component.scss'
})
export class StudentClassFormComponent implements OnInit, AfterViewInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() studentClass: StudentClass;
    @Input() statuses;
    @Input() student: StudentDetails;

    @Input() academicYears: AcademicYear[];
    @Input() schoolStreams: SchoolStream[];
    @Input() learningLevels: LearningLevel[];
    action: string = 'add';

    buttonSubmitActive: boolean = true;

    @ViewChild('yearClassStream')
    yearClassStreamComponent: YearClassStreamComponent;

    @Output() addItemEvent = new EventEmitter<StudentClass>();
    @Output() errorEvent = new EventEmitter<string>();

    studentClassForm: FormGroup;

    constructor(
        private formBuilder: FormBuilder,
        private schoolClassSvc: SchoolClassesService,
        private toastrSvc: ToastrService,
        private router: Router
    ) {}

    ngAfterViewInit(): void {
        this.yearClassStreamComponent.initializeFormControl();
    }

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.studentClassForm = this.formBuilder.group({
            studentId: [this.student?.id, [Validators.required]],
            description: ['']
        });
    };

    setFormControls = (studentClass: StudentClass) => {
        this.schoolClassSvc
            .getById(parseInt(studentClass.id), '/studentClasses')
            .subscribe(
                (sClass: StudentClass) => {
                    this.studentClassForm.patchValue({
                        description: studentClass.description,
                        studentId: studentClass.studentId ?? null
                    });
                    this.yearClassStreamComponent.setFormControls({
                        academicYearId: sClass.schoolClass?.academicYearId,
                        learningLevelId: sClass.schoolClass?.learningLevelId,
                        schoolStreamId: sClass.schoolClass?.schoolStreamId
                    });
                },
                (err) => {
                    this.toastrSvc.error(err.error?.message);
                }
            );
    };

    get f() {
        return this.studentClassForm.controls;
    }

    closeStudentClassForm = () => {
        // this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    resetFormControls() {
        this.action = 'add';
        this.studentClassForm.reset();
        this.studentClassForm.patchValue({studentId: this.student?.id});
    }

    yearClassStreamUpdated = (yearClassStream: any) => {
        this.yearClassStreamComponent
            .checkIfExists(
                yearClassStream.academicYearId,
                yearClassStream.learningLevelId,
                yearClassStream.schoolStreamId
            )
            .subscribe(
                (schoolCl) => {
                    this.buttonSubmitActive = true;
                    if (!schoolCl) {
                        this.toastrSvc.error(
                            'The class selected is not added yet. Ask administrator to register the class!'
                        );
                        this.buttonSubmitActive = false;
                    }
                },
                (err) => {
                    this.buttonSubmitActive = false;
                    this.toastrSvc.error(
                        'The class selected is not added yet. Ask administrator to register the class!'
                    );
                }
            );
    };

    goToRegisteredClasses = () => {
        this.closeButton.nativeElement.click();
        this.router.navigate(['/class/classes']);
    };

    onSubmit = () => {
        this.yearClassStreamComponent
            ?.checkIfExists(
                this.studentClassForm.value?.academicYearId,
                this.studentClassForm.value?.learningLevelId,
                this.studentClassForm.value?.schoolStreamId
            )
            .subscribe(
                (schoolCl) => {
                    if (this.action == 'edit') {
                        let studentClassId = this.studentClass.id;
                        this.studentClass = new StudentClass(
                            this.studentClassForm.value
                        );
                        this.studentClass.schoolClassId = parseInt(schoolCl.id);
                        this.studentClass.id = studentClassId;
                    } else {
                        this.studentClass = new StudentClass(
                            this.studentClassForm.value
                        );
                        this.studentClass.schoolClassId = parseInt(schoolCl.id);
                    }
                    this.addItemEvent.emit(this.studentClass);
                },
                (err) => {
                    this.toastrSvc.error(err.error?.message);
                }
            );
    };
}
