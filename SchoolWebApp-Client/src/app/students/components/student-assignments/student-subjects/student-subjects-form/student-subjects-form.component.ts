import {Subject} from '@/academics/models/subject';
import {AcademicYear} from '@/school/models/academic-year';
import {StudentClass} from '@/students/models/student-class';
import {StudentDetails} from '@/students/models/student-details';
import {StudentSubject} from '@/students/models/student-subject';
import {StudentSubjectsService} from '@/students/services/student-subjects.service';
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
import {ToastrService} from 'ngx-toastr';

@Component({
    selector: 'app-student-subjects-form',
    templateUrl: './student-subjects-form.component.html',
    styleUrl: './student-subjects-form.component.scss'
})
export class StudentSubjectsFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() studentSubject: StudentSubject;
    @Input() statuses;
    @Input() student: StudentDetails;
    @Input() academicYears: AcademicYear[] = [];
    @Input() studentClasses: StudentClass[] = [];
    @Input() subjects: Subject[] = [];

    showSubjectsTbl: boolean = false;
    action: string = 'add';
    studentClassId: number = 0;

    @Output() addItemEvent = new EventEmitter<StudentSubject[]>();
    @Output() errorEvent = new EventEmitter<string>();

    studentSubjectsForm: FormGroup;

    constructor(
        private formBuilder: FormBuilder,
        private studentSubjectsSvc: StudentSubjectsService,
        private toastr: ToastrService
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.studentSubjectsForm = this.formBuilder.group({
            studentId: [this.student?.id, [Validators.required]],
            studentClassId: [null, [Validators.required]]
        });
    };

    setFormControls = (studentSubject: StudentSubject) => {
        this.studentSubjectsForm.setValue({
            studentClassId: studentSubject.studentClass ?? null,
            studentId: this.student?.id ?? null
        });
    };

    get f() {
        return this.studentSubjectsForm.controls;
    }

    closeStudentFormerSchoolForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
        this.refreshItems();
    };

    resetFormControls() {
        this.action = 'add';
        this.studentSubjectsForm.reset();
    }

    onSubmit = () => {
        let studentSubjts: StudentSubject[] = [];

        this.subjects.forEach((s) => {
            if (s.isSelected && !s.isOriginallySelected) {
                studentSubjts.push(
                    new StudentSubject({
                        subjectId: parseInt(s.id),
                        studentClassId: parseInt(
                            this.studentSubjectsForm.value?.studentClassId
                        ),
                        studentClass: null,
                        description: ''
                    })
                );
            }
        });
        this.addItemEvent.emit(studentSubjts);
    };

    academicYearChanged = () => {};

    studentClassChanged = (studentClassId: number) => {
        if (studentClassId == null) {
            return
        }
        this.showSubjectsTbl = true;
        this.studentClassId = studentClassId;
        this.subjects.forEach((s) => (s.isSelected = false));
        this.studentSubjectsSvc
            .get('/studentSubjects/byStudentClassId/' + this.studentClassId)
            .subscribe(
                (studentSubjects) => {
                    this.subjects.forEach((s) => {
                        if (
                            studentSubjects.some(
                                (ss) => ss.subjectId.toString() == s.id
                            )
                        ) {
                            s.isOriginallySelected = true;
                            s.isSelected = true;
                        } else {
                            s.isOriginallySelected = false;
                            s.isSelected = false;
                        }
                    });
                },
                (err) => {
                    this.toastr.error(err.message);
                }
            );
    };

    closeStudentSubjectsForm = () => {
        // this.showSubjectsTbl = false;
        // this.studentSubjectsForm.reset();
        // this.refreshItems();
    };
}
