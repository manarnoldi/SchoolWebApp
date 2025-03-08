import {Subject} from '@/academics/models/subject';
import {SchoolClass} from '@/class/models/school-class';
import {StudentClass} from '@/students/models/student-class';
import {StudentDetails} from '@/students/models/student-details';
import {StudentSubject} from '@/students/models/student-subject';
import {
    AfterViewInit,
    Component,
    EventEmitter,
    Input,
    OnInit,
    Output
} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

@Component({
    selector: 'app-student-subjects-load-form',
    templateUrl: './student-subjects-load-form.component.html',
    styleUrl: './student-subjects-load-form.component.scss'
})
export class StudentSubjectsLoadFormComponent implements OnInit {
    @Input() student: StudentDetails;
    @Output() studentClassChanged = new EventEmitter<any>();

    studentSubjectsLoadForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}
    ngOnInit(): void {
        this.refreshItems();
    }

    get f() {
        return this.studentSubjectsLoadForm.controls;
    }

    refreshItems = () => {
        this.studentSubjectsLoadForm = this.formBuilder.group({
            studentClassId: [null]
        });
    };

    setFormControls = (studentClassId: number) => {
        this.studentSubjectsLoadForm.setValue({
            studentClassId: studentClassId ?? null
        });
    };

    studentClassUpdated = (studentClassId: number) => {
        if (studentClassId) this.studentClassChanged.emit(studentClassId);
        else this.studentClassChanged.emit(null);
    };
}
