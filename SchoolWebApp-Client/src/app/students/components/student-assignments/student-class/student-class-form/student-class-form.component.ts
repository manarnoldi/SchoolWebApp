import {SchoolClass} from '@/class/models/school-class';
import { YearClassStreamComponent } from '@/shared/directives/year-class-stream/year-class-stream.component';
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

    @Input() schoolClasses: SchoolClass[] = [];
    action: string = 'add';

    @ViewChild(YearClassStreamComponent)
    yearClassStreamComponent: YearClassStreamComponent;

    @Output() addItemEvent = new EventEmitter<StudentClass>();
    @Output() errorEvent = new EventEmitter<string>();

    studentClassForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.studentClassForm = this.formBuilder.group({
            studentId: [this.student?.id, [Validators.required]],
            description: [''],
            schoolClassId: [null, [Validators.required]]
        });
    };

    setFormControls = (studentClass: StudentClass) => {
        this.studentClassForm.setValue({
            description: studentClass.description,
            studentId: studentClass.studentId ?? null,
            curriculumId: studentClass.schoolClassId ?? null,
        });
        this.yearClassStreamComponent.setFormControls(studentClass);
    };

    get f() {
        return this.studentClassForm.controls;
    }

    closeStudentClassForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    resetFormControls() {
        this.action = 'add';
        this.studentClassForm.reset();
    }

    onSubmit = () => {
        if (this.action == 'edit') {
            let studentClassId = this.studentClass.id;
            this.studentClass = new StudentClass(
                this.studentClassForm.value
            );
            this.studentClass.id = studentClassId;
        } else {
            this.studentClass = new StudentClass(
                this.studentClassForm.value
            );
        }
        this.addItemEvent.emit(this.studentClass);
    };
}
