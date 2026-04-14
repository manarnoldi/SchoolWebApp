import {Curriculum} from '@/academics/models/curriculum';
import {Subject} from '@/academics/models/subject';
import {LearningExperience} from '@/cbe/assessments/models/learning-experience';
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
    selector: 'app-learning-experience-add-form',
    templateUrl: './learning-experience-add-form.component.html',
    styleUrl: './learning-experience-add-form.component.scss'
})
export class LearningExperienceAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;

    @Input() learningExperience: LearningExperience;
    @Input() strands: Strand[];
    @Input() subStrands: SubStrand[];
    @Input() subjects: Subject[];
    @Input() learningLevels: LearningLevel[];
    @Input() curricula: Curriculum[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<LearningExperience>();
    @Output() errorEvent = new EventEmitter<string>();
    @Output() curriculumChangedEvent = new EventEmitter<number>();
    @Output() academicYearChangedEvent = new EventEmitter<number>();
    @Output() learningLevelChangedEvent = new EventEmitter<number>();
    @Output() subjectChangedEvent = new EventEmitter<number>();
    @Output() strandChangedEvent = new EventEmitter<number>();

    learningExperienceForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.learningExperienceForm = this.formBuilder.group({
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

    setFormControls = (learningExperience: LearningExperience) => {
        this.learningExperienceForm.setValue({
            name: learningExperience.name,
            rank: learningExperience.rank,
            description: learningExperience.description,
            academicYearId: learningExperience.academicYearId ?? null,
            curriculumId: learningExperience.curriculumId ?? null,
            subjectId: learningExperience.subjectId ?? null,
            learningLevelId: learningExperience.learningLevelId ?? null,
            strandId: learningExperience.strandId ?? null,
            subStrandId: learningExperience.subStrandId ?? null
        });
    };

    get f() {
        return this.learningExperienceForm.controls;
    }

    closeLearningExperienceForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    resetFormControls() {
        this.editMode = false;
        this.learningExperienceForm.reset();
    }

    onAcademicYearChange = () => {
        let academicYearId = this.learningExperienceForm.get('academicYearId').value;
        this.academicYearChangedEvent.emit(academicYearId);
    };

    onCurriculumChange = () => {
        let curriculumId = this.learningExperienceForm.get('curriculumId').value;
        this.curriculumChangedEvent.emit(curriculumId);
    };

    onLearningLevelChange = () => {
        let learningLevelId = this.learningExperienceForm.get('learningLevelId').value;
        this.learningLevelChangedEvent.emit(learningLevelId);
    };

    onSubjectChange = () => {
        let subjectId = this.learningExperienceForm.get('subjectId').value;
        this.subjectChangedEvent.emit(subjectId);
    };

    onStrandChange = () => {
        let strandId = this.learningExperienceForm.get('strandId').value;
        this.strandChangedEvent.emit(strandId);
    };

    onSubmit = () => {
        if (this.editMode) {
            let learningExperienceId = this.learningExperience.id;
            this.learningExperience = new LearningExperience(this.learningExperienceForm.value);
            this.learningExperience.id = learningExperienceId;
        } else {
            this.learningExperience = new LearningExperience(this.learningExperienceForm.value);
        }
        this.addItemEvent.emit(this.learningExperience);
    };
}
