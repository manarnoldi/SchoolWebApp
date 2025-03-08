import {LearningLevel} from '@/class/models/learning-level';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolStream} from '@/class/models/school-stream';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {AcademicYear} from '@/school/models/academic-year';
import {YearClassStreamComponent} from '@/shared/directives/year-class-stream/year-class-stream.component';
import {StudentAttendance} from '@/students/models/student-attendance';
import {StudentDetails} from '@/students/models/student-details';
import {formatDate} from '@angular/common';
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
    selector: 'app-student-attendance-form',
    templateUrl: './student-attendance-form.component.html',
    styleUrl: './student-attendance-form.component.scss'
})
export class StudentAttendanceFormComponent implements OnInit, AfterViewInit {
    @ViewChild('yearClassStream')
    yearClassStreamComponent: YearClassStreamComponent;
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() studentAttendance: StudentAttendance;
    @Input() statuses;
    @Input() student: StudentDetails;

    @Input() academicYears: AcademicYear[];
    @Input() schoolStreams: SchoolStream[];
    @Input() learningLevels: LearningLevel[];
    action: string = 'add';
    buttonSubmitActive: boolean = true;

    editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<StudentAttendance>();
    @Output() errorEvent = new EventEmitter<string>();

    studentAttendanceForm: FormGroup;

    constructor(
        private formBuilder: FormBuilder,
        private toastrSvc: ToastrService,
        private schoolClassSvc: SchoolClassesService,
        private router: Router
    ) {}

    ngAfterViewInit(): void {
        this.yearClassStreamComponent.initializeFormControl();
    }

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.studentAttendanceForm = this.formBuilder.group({
            date: [
                formatDate(new Date(), 'yyyy-MM-dd', 'en'),
                [Validators.required]
            ],
            present: [true],
            remarks: [''],
            studentClassId: [this.studentAttendance?.studentClassId, [Validators.required]]
        });
    };

    setFormControls = (studentAttendance: StudentAttendance) => {
        this.schoolClassSvc
            .getById(
                studentAttendance?.studentClass?.schoolClassId,
                '/schoolClasses'
            )
            .subscribe(
                (schoolClass: SchoolClass) => {
                    this.studentAttendanceForm.patchValue({
                        date: formatDate(
                            new Date(studentAttendance.date),
                            'yyyy-MM-dd',
                            'en'
                        ),
                        present: studentAttendance.present,
                        remarks: studentAttendance.remarks,
                        studentClassId: studentAttendance.studentClassId ?? null
                    });
                    this.yearClassStreamComponent.setFormControls({
                        academicYearId: schoolClass?.academicYearId,
                        learningLevelId: schoolClass?.learningLevelId,
                        schoolStreamId: schoolClass?.schoolStreamId
                    });
                },
                (err) => {
                    this.toastrSvc.error(err.error?.message);
                }
            );
    };

    get f() {
        return this.studentAttendanceForm.controls;
    }

    closeStudentAttendanceForm = () => {
        this.closeButton.nativeElement.click();
        this.refreshItems();
    };

    viewItem = (studentAttendance: StudentAttendance, action: string) => {
        this.studentAttendance = studentAttendance;
        this.setFormControls(studentAttendance);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls() {
        this.editMode = false;
        this.studentAttendanceForm.reset();
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

    onSubmit = () => {
        if (this.editMode) {
            let studentAttendanceId = this.studentAttendance.id;
            this.studentAttendance = new StudentAttendance(
                this.studentAttendanceForm.value
            );
            this.studentAttendance.id = studentAttendanceId;
        } else {
            this.studentAttendance = new StudentAttendance(
                this.studentAttendanceForm.value
            );
        }
        this.addItemEvent.emit(this.studentAttendance);
    };

    goToRegisteredClasses = () => {
        this.closeButton.nativeElement.click();
        this.router.navigate(['/class/classes']);
    };
}
