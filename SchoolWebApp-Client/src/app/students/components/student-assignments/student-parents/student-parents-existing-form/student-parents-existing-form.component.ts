import {Relationship} from '@/settings/models/relationship';
import {Parent} from '@/students/models/parent';
import {StudentDetails} from '@/students/models/student-details';
import {StudentParent} from '@/students/models/student-parent';
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
    selector: 'app-student-parents-existing-form',
    templateUrl: './student-parents-existing-form.component.html',
    styleUrl: './student-parents-existing-form.component.scss'
})
export class StudentParentsExistingFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() studentParent: StudentParent;
    @Input() statuses;
    @Input() student: StudentDetails;

    @Input() parents: Parent[] = [];
    @Input() relationShips: Relationship[] = [];
    action: string = 'add';

    @Output() addItemEvent = new EventEmitter<StudentParent>();
    @Output() errorEvent = new EventEmitter<string>();

    studentExistingParentForm: FormGroup;
    parent: Parent;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.studentExistingParentForm = this.formBuilder.group({
            relationShipId: [null, [Validators.required]],
            studentId: [this.student?.id, [Validators.required]],
            parentId: [null, [Validators.required]],
            otherDetails: ['']
        });
    };

    setFormControls = (studentParent: StudentParent) => {
        this.studentExistingParentForm.setValue({
            relationShipId: studentParent.relationShipId ?? null,
            studentId: studentParent.studentId ?? null,
            parentId: studentParent.parentId ?? null,
            otherDetails: studentParent.otherDetails
        });
    };

    get f() {
        return this.studentExistingParentForm.controls;
    }

    loadParent = () => {
        this.parent = this.studentExistingParentForm.get('parentId').value;
    };

    closeExistingStudentParentForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
        this.refreshItems();
    };

    resetFormControls() {
        this.action = 'add';
        this.studentExistingParentForm.reset();
    }

    onSubmit = () => {
        if (this.action == 'edit') {
            let studentParentId = this.studentParent.id;
            this.studentParent = new StudentParent(
                this.studentExistingParentForm.value
            );
            this.studentParent.id = studentParentId;
        } else {
            this.studentParent = new StudentParent(
                this.studentExistingParentForm.value
            );
        }
        this.studentParent.parentId =
            this.studentExistingParentForm.get('parentId').value.id;
        this.addItemEvent.emit(this.studentParent);
    };
}
