import {LearningLevel} from '@/class/models/learning-level';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolStream} from '@/class/models/school-stream';
import {AcademicYear} from '@/school/models/academic-year';
import {StudentClass} from '@/students/models/student-class';
import {StudentDetails} from '@/students/models/student-details';
import {
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
export class StudentClassFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() studentClass: StudentClass;
    @Input() statuses;
    @Input() student: StudentDetails;

    @Input() academicYears: AcademicYear[];
    @Input() schoolStreams: SchoolStream[];
    @Input() learningLevels: LearningLevel[];
    @Input() schoolClasses: SchoolClass[] = [];
    action: string = 'add';

    buttonSubmitActive: boolean = true;
    schoolClassName: string;

    @Output() addItemEvent = new EventEmitter<StudentClass>();
    @Output() errorEvent = new EventEmitter<string>();
    @Output() yearChangedEvent = new EventEmitter<number>();

    studentClassForm: FormGroup;

    constructor(
        private formBuilder: FormBuilder
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.studentClassForm = this.formBuilder.group({
            studentId: [this.student?.id, [Validators.required]],
            academicYearId: [null, [Validators.required]],
            schoolClassId: [null, [Validators.required]],
            description: ['']
        });
    };

    setFormControls = (studentClass: StudentClass) => {
        this.studentClassForm.setValue({
            studentId: this.student?.id,
            academicYearId: studentClass.schoolClass?.academicYearId,
            schoolClassId: studentClass.schoolClassId,
            description: studentClass.description
        });
        this.schoolClassName = studentClass.schoolClass?.name;
    };

    yearChanged = () => {
        this.studentClassForm.get('schoolClassId').reset();
        let academicYearId = this.studentClassForm.get('academicYearId').value;
        this.yearChangedEvent.emit(academicYearId);
    };

    get f() {
        return this.studentClassForm.controls;
    }

    closeStudentClassForm = () => {
        this.resetFormControls();
    };

    resetFormControls() {
        this.action = 'add';
        this.studentClassForm.reset();
        this.studentClassForm.patchValue({studentId: this.student?.id});
    }

    onSubmit = () => {
        let studentClassId = this.studentClass?.id;
        this.studentClass = new StudentClass(this.studentClassForm.value);
        this.studentClass.id = studentClassId;
        this.addItemEvent.emit(this.studentClass);
    };
}
