import {StudentClass} from '@/students/models/student-class';
import {StudentDetails} from '@/students/models/student-details';
import {StudentClassService} from '@/students/services/student-class.service';
import {
    Component,
    EventEmitter,
    Host,
    Input,
    OnInit,
    Optional,
    Output,
    ViewChild
} from '@angular/core';
import {
    ControlContainer,
    FormControl,
    FormGroupDirective
} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';

@Component({
    selector: 'app-student-classes',
    templateUrl: './student-classes.component.html',
    styleUrl: './student-classes.component.scss',
    viewProviders: [
        {provide: ControlContainer, useExisting: FormGroupDirective}
    ]
})
export class StudentClassesComponent implements OnInit {
    @Input() student: StudentDetails;
    @Output() studentClassChanged = new EventEmitter<any>();

    @Input() controlName!: string;
    @Input() controlTitle!: string;
    @Input() controlInstruction!: string;

    studentClasses: StudentClass[];

    constructor(
        private studentClassesSvc: StudentClassService,
        private toastrSvc: ToastrService,
        @Optional() @Host() public controlContainer: ControlContainer
    ) {}

    ngOnInit() {
        this.studentClassesSvc
            .get('/studentClasses/byStudentId/' + this.student?.id)
            .subscribe(
                (studentClasses) => {
                    this.studentClasses = studentClasses;
                },
                (err) => {
                    this.toastrSvc.error(err.error?.message);
                }
            );
        const control = this.controlContainer?.control?.get(
            this.controlName
        ) as FormControl;
        if (control) {
            control.valueChanges.subscribe((value) => {
                this.studentClassChanged.emit(value); // Emit new value
            });
        }


    }
}
