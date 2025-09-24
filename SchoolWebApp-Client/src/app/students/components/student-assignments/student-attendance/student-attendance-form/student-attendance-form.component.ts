import {SchoolClassesService} from '@/class/services/school-classes.service';
import {StudentAttendance} from '@/students/models/student-attendance';
import {StudentDetails} from '@/students/models/student-details';
import {formatDate} from '@angular/common';
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
    selector: 'app-student-attendance-form',
    templateUrl: './student-attendance-form.component.html',
    styleUrl: './student-attendance-form.component.scss'
})
export class StudentAttendanceFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() studentAttendance: StudentAttendance;
    @Input() statuses;
    @Input() student: StudentDetails;

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
    ) { }
    
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
        this.studentAttendanceForm.setValue({
            date: formatDate(
                new Date(studentAttendance == null || studentAttendance == undefined ? new Date() : studentAttendance.date ),
                'yyyy-MM-dd',
                'en'
            ),
            present: studentAttendance?.present ?? null,
            remarks: studentAttendance?.remarks ?? null,
            studentClassId: studentAttendance?.studentClassId ?? null
        });
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

    studentClassChanged (studentClassId: number) {

    }

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
        this.studentAttendance.present = this.studentAttendance.present === null ? false : this.studentAttendance.present;
        this.addItemEvent.emit(this.studentAttendance);
    };

    goToRegisteredClasses = () => {
        this.closeButton.nativeElement.click();
        this.router.navigate(['/class/classes']);
    };
}
