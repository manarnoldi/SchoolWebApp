import {EducationLevel} from '@/school/models/educationLevel';
import {LearningLevel} from '@/school/models/learning-level';
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
    selector: 'app-learning-levels-form',
    templateUrl: './learning-levels-form.component.html',
    styleUrl: './learning-levels-form.component.scss'
})
export class LearningLevelsFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;

    @Input() learningLevel: LearningLevel;
    @Input() educationLevels: EducationLevel[];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<LearningLevel>();
    @Output() errorEvent = new EventEmitter<string>();

    learningLevelForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.learningLevelForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            description: [''],
            educationLevelId: [null, [Validators.required]]
        });
    };

    setFormControls = (learningLevel: LearningLevel) => {
        this.learningLevelForm.setValue({
            name: learningLevel.name,
            description: learningLevel.description,
            educationLevelId: learningLevel.educationLevelId ?? null
        });
    };

    get f() {
        return this.learningLevelForm.controls;
    }

    closeLearningLevelForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    viewItem = (learningLevel: LearningLevel, action: string) => {
        this.learningLevel = learningLevel;
        this.setFormControls(learningLevel);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls() {
        this.editMode = false;
        this.learningLevelForm.reset();
    }

    onSubmit = () => {
        if (this.editMode) {
            let learningLevelId = this.learningLevel.id;
            this.learningLevel = new LearningLevel(
                this.learningLevelForm.value
            );
            this.learningLevel.id = learningLevelId;
        } else {
            this.learningLevel = new LearningLevel(
                this.learningLevelForm.value
            );
        }
        this.addItemEvent.emit(this.learningLevel);
    };
}
