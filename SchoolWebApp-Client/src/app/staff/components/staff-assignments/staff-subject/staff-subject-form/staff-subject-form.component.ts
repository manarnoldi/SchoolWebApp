import {Subject} from '@/academics/models/subject';
import {SchoolClass} from '@/class/models/school-class';
import {AcademicYear} from '@/school/models/academic-year';
import {StaffDetails} from '@/staff/models/staff-details';
import {StaffSubject} from '@/staff/models/staff-subject';
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
    selector: 'app-staff-subject-form',
    templateUrl: './staff-subject-form.component.html',
    styleUrl: './staff-subject-form.component.scss'
})
export class StaffSubjectFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    buttonSubmitActive: boolean = true;
    @Input() staffSubject: StaffSubject;
    @Input() statuses;
    @Input() staff: StaffDetails;

    @Input() subjects: Subject[] = [];

    @Input() academicYears: AcademicYear[] = [];
    @Input() schoolClasses: SchoolClass[] = [];

    action: string = 'add';
    schoolClassName: string;

    @Output() addItemEvent = new EventEmitter<StaffSubject>();
    @Output() errorEvent = new EventEmitter<string>();
    @Output() yearChangedEvent = new EventEmitter<number>();
    @Output() classChangedEvent = new EventEmitter<number>();

    staffSubjectForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.staffSubjectForm = this.formBuilder.group({
            staffDetailsId: [this.staff?.id, [Validators.required]],
            academicYearId: [null, [Validators.required]],
            schoolClassId: [null, [Validators.required]],
            description: [''],
            subjectId: [null, [Validators.required]]
        });
    };

    setFormControls = (staffSubject: StaffSubject) => {
        this.staffSubjectForm.setValue({
            staffDetailsId: this.staff?.id,
            academicYearId: staffSubject.schoolClass?.academicYearId,
            schoolClassId: staffSubject.schoolClassId,
            description: staffSubject.description,
            subjectId: staffSubject.subjectId
        });
        this.schoolClassName = staffSubject.schoolClass?.name;
    };

    get f() {
        return this.staffSubjectForm.controls;
    }

    closeStaffSubjectForm = () => {
        this.resetFormControls();
    };

    resetFormControls() {
        this.action = 'add';
        this.staffSubjectForm.reset();
        this.subjects = [];
        this.staffSubjectForm.get('staffDetailsId').setValue(this.staff?.id);
    }

    yearChanged = () => {
        this.staffSubjectForm.get('schoolClassId').reset();
        this.staffSubjectForm.get('subjectId').reset();
        let academicYearId = this.staffSubjectForm.get('academicYearId').value;
        this.yearChangedEvent.emit(academicYearId);
    };

    classChanged = () => {
        this.staffSubjectForm.get('subjectId').reset();
        let schoolClassId = this.staffSubjectForm.get('schoolClassId').value;
        let schoolClass = this.schoolClasses.find(
            (sc) => sc.id == schoolClassId
        );

        this.classChangedEvent.emit(
            schoolClass?.learningLevel?.educationLevelId
        );
    };

    onSubmit = () => {
        let staffSubjectId = this.staffSubject?.id;
        this.staffSubject = new StaffSubject(this.staffSubjectForm.value);
        this.staffSubject.id = staffSubjectId;
        this.addItemEvent.emit(this.staffSubject);
    };
}
