import {Subject} from '@/academics/models/subject';
import {SubjectGroup} from '@/academics/models/subject-group';
import {Department} from '@/school/models/department';
import {StaffDetails} from '@/staff/models/staff-details';
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
    selector: 'app-subjects-add-form',
    templateUrl: './subjects-add-form.component.html',
    styleUrl: './subjects-add-form.component.scss'
})
export class SubjectsAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() subject: Subject;
    @Input() subjectGroups: SubjectGroup[] = [];
    @Input() staffs: StaffDetails[] = [];
    @Input() departments: Department[] = [];

    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<Subject>();
    @Output() errorEvent = new EventEmitter<string>();

    subjectForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.subjectForm = this.formBuilder.group({
            code: ['', [Validators.required]],
            name: ['', [Validators.required]],
            abbr: ['', [Validators.required]],
            rank: ['', [Validators.required]],
            numOfLessons: [0, [Validators.required]],
            description: [''],
            optional: [false, [Validators.required]],
            subjectGroupId: [null, [Validators.required]],
            departmentId: [null, [Validators.required]],
            staffDetailsId: [null, [Validators.required]]
        });
    };

    setFormControls = (subject: Subject) => {
        this.subjectForm.setValue({
            code: subject?.code,
            name: subject?.name,
            abbr: subject?.abbr,
            rank: subject?.rank,
            numOfLessons: subject?.numOfLessons,
            description: subject?.description,
            optional: subject?.optional,
            subjectGroupId: subject?.subjectGroupId,
            departmentId: subject?.departmentId,
            staffDetailsId: subject?.staffDetails
        });
    };

    get f() {
        return this.subjectForm.controls;
    }

    closeSubjectForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    viewItem = (subject: Subject, action: string) => {
        this.subject = subject;
        this.setFormControls(subject);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls() {
        this.editMode = false;
        this.subjectForm.reset();
    }

    onSubmit = () => {
        let subjectId = this.subject?.id;
        this.subject = new Subject(this.subjectForm.value);
        if (this.editMode) this.subject.id = subjectId;
        this.subject.staffDetailsId =
            this.subjectForm.get('staffDetailsId').value.id;
        this.addItemEvent.emit(this.subject);
    };
}
