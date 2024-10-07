import {Subject} from '@/academics/models/subject';
import {SchoolClass} from '@/class/models/school-class';
import {StudentClass} from '@/students/models/student-class';
import {StudentSubject} from '@/students/models/student-subject';
import {AfterViewInit, Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

@Component({
    selector: 'app-student-subjects-load-form',
    templateUrl: './student-subjects-load-form.component.html',
    styleUrl: './student-subjects-load-form.component.scss'
})
export class StudentSubjectsLoadFormComponent implements OnInit {
    @Input() studentClasses: StudentClass[] = [];
    @Input() subjects: Subject[] = [];
    @Output() loadClicked = new EventEmitter<any>();

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

    setFormControls = (studentClass: StudentClass) => {
        this.studentSubjectsLoadForm.setValue({
            studentClassId: studentClass ?? null
        });
    };

    // onSubmit = () => {
    //     let schoolClass = new SchoolClass(this.staffSubjectsLoadForm.value?.schoolClassId);
    //     this.loadClicked.emit(schoolClass);
    // };

    schoolClassChanged = () => {
        if (this.studentSubjectsLoadForm.value?.studentClassId) {
            let studentClass = new StudentClass(
                this.studentSubjectsLoadForm.value?.studentClassId
            );
            this.loadClicked.emit(studentClass);
        } else {
            this.loadClicked.emit(null);
        }
    };
}
