import {StudentAssessment} from '@/cbe/assessments/models/student-assessment';
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
    selector: 'app-student-assessment-add-form',
    templateUrl: './student-assessment-add-form.component.html',
    styleUrl: './student-assessment-add-form.component.scss'
})
export class StudentAssessmentAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;

    @Input() studentAssessment: StudentAssessment;
    @Input() students: any[] = [];
    @Input() schoolClasses: any[] = [];
    @Input() specificOutcomes: any[] = [];
    @Input() grades: any[] = [];
    @Input() sessions: any[] = [];
    @Input() assessmentTypes: any[] = [];
    @Input() staffDetails: any[] = [];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<StudentAssessment>();
    @Output() errorEvent = new EventEmitter<string>();
    @Output() schoolClassChangedEvent = new EventEmitter<number>();

    onSchoolClassChange = () => {
        let schoolClassId = this.studentAssessmentForm.get('schoolClassId').value;
        this.studentAssessmentForm.get('studentId').setValue(null);
        this.schoolClassChangedEvent.emit(schoolClassId);
    };

    studentAssessmentForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.studentAssessmentForm = this.formBuilder.group({
            studentId: [null, [Validators.required]],
            schoolClassId: [null, [Validators.required]],
            specificOutcomeId: [null, [Validators.required]],
            gradeId: [null, [Validators.required]],
            sessionId: [null, [Validators.required]],
            assessmentTypeId: [null, [Validators.required]],
            assessmentDate: [''],
            staffDetailsId: [null, [Validators.required]],
            description: ['']
        });
    };

    setFormControls = (sa: StudentAssessment) => {
        this.studentAssessmentForm.setValue({
            studentId: sa.studentId ?? null,
            schoolClassId: sa.schoolClassId ?? null,
            specificOutcomeId: sa.specificOutcomeId ?? null,
            gradeId: sa.gradeId ?? null,
            sessionId: sa.sessionId ?? null,
            assessmentTypeId: sa.assessmentTypeId ?? null,
            assessmentDate: sa.assessmentDate ?? '',
            staffDetailsId: sa.staffDetailsId ?? null,
            description: sa.description ?? ''
        });
    };

    get f() {
        return this.studentAssessmentForm.controls;
    }

    closeForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    resetFormControls() {
        this.editMode = false;
        this.studentAssessmentForm.reset();
    }

    onSubmit = () => {
        if (this.editMode) {
            let saId = this.studentAssessment.id;
            this.studentAssessment = new StudentAssessment(this.studentAssessmentForm.value);
            this.studentAssessment.id = saId;
        } else {
            this.studentAssessment = new StudentAssessment(this.studentAssessmentForm.value);
        }
        this.addItemEvent.emit(this.studentAssessment);
    };
}
