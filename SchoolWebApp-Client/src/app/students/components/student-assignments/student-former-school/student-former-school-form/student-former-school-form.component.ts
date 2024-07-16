import { Curriculum } from '@/academics/models/curriculum';
import { EducationLevel } from '@/school/models/educationLevel';
import {StudentDetails} from '@/students/models/student-details';
import {StudentFormerSchool} from '@/students/models/student-former-school';
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
    selector: 'app-student-former-school-form',
    templateUrl: './student-former-school-form.component.html',
    styleUrl: './student-former-school-form.component.scss'
})
export class StudentFormerSchoolFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() studentFormerSchool: StudentFormerSchool;
    @Input() statuses;
    @Input() student: StudentDetails;

    @Input() curricula: Curriculum[] = [];
    @Input() educationLevels: EducationLevel[] = [];
    action: string = 'add';

    @Output() addItemEvent = new EventEmitter<StudentFormerSchool>();
    @Output() errorEvent = new EventEmitter<string>();

    studentFormerSchoolForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.studentFormerSchoolForm = this.formBuilder.group({
            studentId: [this.student?.id, [Validators.required]],
            description: [''],
            schoolName: ['', [Validators.required]],
            classDetails: ['', [Validators.required]],
            score: [''],
            position: [''],            
            curriculumId: [null, [Validators.required]],
            educationLevelId: [null, [Validators.required]]
        });
    };

    setFormControls = (studentFormerSchool: StudentFormerSchool) => {
        this.studentFormerSchoolForm.setValue({
            description: studentFormerSchool.description,
            schoolName: studentFormerSchool.schoolName,
            classDetails: studentFormerSchool.classDetails,
            score: studentFormerSchool.score,
            position: studentFormerSchool.position,
            studentId: studentFormerSchool.studentId ?? null,
            curriculumId: studentFormerSchool.curriculumId ?? null,
            educationLevelId: studentFormerSchool.educationLevelId ?? null
        });
    };

    get f() {
        return this.studentFormerSchoolForm.controls;
    }

    closeStudentFormerSchoolForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
        this.refreshItems();        
    };  

    resetFormControls() {
        this.action = 'add';
        this.studentFormerSchoolForm.reset();
    }

    onSubmit = () => {
        if ( this.action == 'edit') {
            let studentFormerSchoolId = this.studentFormerSchool.id;
            this.studentFormerSchool = new StudentFormerSchool(
                this.studentFormerSchoolForm.value
            );
            this.studentFormerSchool.id = studentFormerSchoolId;
        } else {
            this.studentFormerSchool = new StudentFormerSchool(
                this.studentFormerSchoolForm.value
            );
        }
        this.addItemEvent.emit(this.studentFormerSchool);
    };
}
