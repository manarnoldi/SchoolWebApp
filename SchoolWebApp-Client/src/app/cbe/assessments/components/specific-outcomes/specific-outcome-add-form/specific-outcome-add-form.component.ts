import {Curriculum} from '@/academics/models/curriculum';
import {Subject} from '@/academics/models/subject';
import {SpecificOutcome} from '@/cbe/assessments/models/specific-outcome';
import {Strand} from '@/cbe/assessments/models/strand';
import {SubStrand} from '@/cbe/assessments/models/sub-strand';
import {BroadOutcome} from '@/cbe/assessments/models/broad-outcome';
import {GeneralOutcome} from '@/cbe/assessments/models/general-outcome';
import {LearningLevel} from '@/class/models/learning-level';
import {AcademicYear} from '@/school/models/academic-year';
import {Session} from '@/class/models/session';
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
    selector: 'app-specific-outcome-add-form',
    templateUrl: './specific-outcome-add-form.component.html',
    styleUrl: './specific-outcome-add-form.component.scss'
})
export class SpecificOutcomeAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;

    @Input() specificOutcome: SpecificOutcome;
    @Input() strands: Strand[];
    @Input() subStrands: SubStrand[];
    @Input() broadOutcomes: BroadOutcome[];
    @Input() generalOutcomes: GeneralOutcome[];
    @Input() subjects: Subject[];
    @Input() learningLevels: LearningLevel[];
    @Input() curricula: Curriculum[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() sessions: Session[] = [];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<SpecificOutcome>();
    @Output() errorEvent = new EventEmitter<string>();
    @Output() curriculumChangedEvent = new EventEmitter<number>();
    @Output() academicYearChangedEvent = new EventEmitter<number>();
    @Output() learningLevelChangedEvent = new EventEmitter<number>();
    @Output() subjectChangedEvent = new EventEmitter<number>();
    @Output() strandChangedEvent = new EventEmitter<number>();

    specificOutcomeForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.specificOutcomeForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            description: [''],
            academicYearId: [null, [Validators.required]],
            curriculumId: [null, [Validators.required]],
            subjectId: [null, [Validators.required]],
            learningLevelId: [null, [Validators.required]],
            strandId: [null, [Validators.required]],
            subStrandId: [null, [Validators.required]],
            broadOutcomeId: [null, [Validators.required]],
            generalOutcomeId: [null, [Validators.required]]
        });
    };

    setFormControls = (specificOutcome: SpecificOutcome) => {
        this.specificOutcomeForm.setValue({
            name: specificOutcome.name,
            rank: specificOutcome.rank,
            description: specificOutcome.description,
            academicYearId: specificOutcome.academicYearId ?? null,
            curriculumId: specificOutcome.curriculumId ?? null,
            subjectId: specificOutcome.subjectId ?? null,
            learningLevelId: specificOutcome.learningLevelId ?? null,
            strandId: specificOutcome.strandId ?? null,
            subStrandId: specificOutcome.subStrandId ?? null,
            broadOutcomeId: specificOutcome.broadOutcomeId ?? null,
            generalOutcomeId: specificOutcome.generalOutcomeId ?? null
        });
    };

    get f() {
        return this.specificOutcomeForm.controls;
    }

    closeSpecificOutcomeForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    resetFormControls() {
        this.editMode = false;
        this.specificOutcomeForm.reset();
    }

    onAcademicYearChange = () => {
        let academicYearId = this.specificOutcomeForm.get('academicYearId').value;
        this.academicYearChangedEvent.emit(academicYearId);
    };

    onCurriculumChange = () => {
        let curriculumId = this.specificOutcomeForm.get('curriculumId').value;
        this.curriculumChangedEvent.emit(curriculumId);
    };

    onLearningLevelChange = () => {
        let learningLevelId = this.specificOutcomeForm.get('learningLevelId').value;
        this.learningLevelChangedEvent.emit(learningLevelId);
    };

    onSubjectChange = () => {
        let subjectId = this.specificOutcomeForm.get('subjectId').value;
        this.subjectChangedEvent.emit(subjectId);
    };

    onStrandChange = () => {
        let strandId = this.specificOutcomeForm.get('strandId').value;
        this.strandChangedEvent.emit(strandId);
    };

    onSubmit = () => {
        if (this.editMode) {
            let specificOutcomeId = this.specificOutcome.id;
            this.specificOutcome = new SpecificOutcome(this.specificOutcomeForm.value);
            this.specificOutcome.id = specificOutcomeId;
        } else {
            this.specificOutcome = new SpecificOutcome(this.specificOutcomeForm.value);
        }
        this.addItemEvent.emit(this.specificOutcome);
    };
}
