import {Curriculum} from '@/academics/models/curriculum';
import {Subject} from '@/academics/models/subject';
import {KeyQuestion} from '@/cbe/assessments/models/key-question';
import {Strand} from '@/cbe/assessments/models/strand';
import {SubStrand} from '@/cbe/assessments/models/sub-strand';
import {LearningLevel} from '@/class/models/learning-level';
import {AcademicYear} from '@/school/models/academic-year';
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
    selector: 'app-key-question-add-form',
    templateUrl: './key-question-add-form.component.html',
    styleUrl: './key-question-add-form.component.scss'
})
export class KeyQuestionAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;

    @Input() keyQuestion: KeyQuestion;
    @Input() strands: Strand[];
    @Input() subStrands: SubStrand[];
    @Input() subjects: Subject[];
    @Input() learningLevels: LearningLevel[];
    @Input() curricula: Curriculum[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<KeyQuestion>();
    @Output() errorEvent = new EventEmitter<string>();
    @Output() curriculumChangedEvent = new EventEmitter<number>();
    @Output() academicYearChangedEvent = new EventEmitter<number>();
    @Output() learningLevelChangedEvent = new EventEmitter<number>();
    @Output() subjectChangedEvent = new EventEmitter<number>();
    @Output() strandChangedEvent = new EventEmitter<number>();

    keyQuestionForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.keyQuestionForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            description: [''],
            academicYearId: [null, [Validators.required]],
            curriculumId: [null, [Validators.required]],
            subjectId: [null, [Validators.required]],
            learningLevelId: [null, [Validators.required]],
            strandId: [null, [Validators.required]],
            subStrandId: [null, [Validators.required]]
        });
    };

    setFormControls = (keyQuestion: KeyQuestion) => {
        this.keyQuestionForm.setValue({
            name: keyQuestion.name,
            rank: keyQuestion.rank,
            description: keyQuestion.description,
            academicYearId: keyQuestion.academicYearId ?? null,
            curriculumId: keyQuestion.curriculumId ?? null,
            subjectId: keyQuestion.subjectId ?? null,
            learningLevelId: keyQuestion.learningLevelId ?? null,
            strandId: keyQuestion.strandId ?? null,
            subStrandId: keyQuestion.subStrandId ?? null
        });
    };

    get f() {
        return this.keyQuestionForm.controls;
    }

    closeKeyQuestionForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    resetFormControls() {
        this.editMode = false;
        this.keyQuestionForm.reset();
    }

    onAcademicYearChange = () => {
        let academicYearId = this.keyQuestionForm.get('academicYearId').value;
        this.academicYearChangedEvent.emit(academicYearId);
    };

    onCurriculumChange = () => {
        let curriculumId = this.keyQuestionForm.get('curriculumId').value;
        this.curriculumChangedEvent.emit(curriculumId);
    };

    onLearningLevelChange = () => {
        let learningLevelId = this.keyQuestionForm.get('learningLevelId').value;
        this.learningLevelChangedEvent.emit(learningLevelId);
    };

    onSubjectChange = () => {
        let subjectId = this.keyQuestionForm.get('subjectId').value;
        this.subjectChangedEvent.emit(subjectId);
    };

    onStrandChange = () => {
        let strandId = this.keyQuestionForm.get('strandId').value;
        this.strandChangedEvent.emit(strandId);
    };

    onSubmit = () => {
        if (this.editMode) {
            let keyQuestionId = this.keyQuestion.id;
            this.keyQuestion = new KeyQuestion(this.keyQuestionForm.value);
            this.keyQuestion.id = keyQuestionId;
        } else {
            this.keyQuestion = new KeyQuestion(this.keyQuestionForm.value);
        }
        this.addItemEvent.emit(this.keyQuestion);
    };
}
