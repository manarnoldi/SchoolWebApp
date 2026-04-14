import {Curriculum} from '@/academics/models/curriculum';
import {Subject} from '@/academics/models/subject';
import {LessonAllocation} from '@/cbe/assessments/models/lesson-allocation';
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
    selector: 'app-lesson-allocation-add-form',
    templateUrl: './lesson-allocation-add-form.component.html',
    styleUrl: './lesson-allocation-add-form.component.scss'
})
export class LessonAllocationAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;

    @Input() lessonAllocation: LessonAllocation;
    @Input() subjects: Subject[];
    @Input() learningLevels: LearningLevel[];
    @Input() curricula: Curriculum[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<LessonAllocation>();
    @Output() errorEvent = new EventEmitter<string>();
    @Output() curriculumChangedEvent = new EventEmitter<number>();
    @Output() academicYearChangedEvent = new EventEmitter<number>();
    @Output() learningLevelChangedEvent = new EventEmitter<number>();

    lessonAllocationForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.lessonAllocationForm = this.formBuilder.group({
            lessonsPerWeek: [0, [Validators.required, Validators.min(1)]],
            description: [''],
            academicYearId: [null, [Validators.required]],
            curriculumId: [null, [Validators.required]],
            learningLevelId: [null, [Validators.required]],
            subjectId: [null, [Validators.required]]
        });
    };

    setFormControls = (lessonAllocation: LessonAllocation) => {
        this.lessonAllocationForm.setValue({
            lessonsPerWeek: lessonAllocation.lessonsPerWeek,
            description: lessonAllocation.description ?? '',
            academicYearId: lessonAllocation.academicYearId ?? null,
            curriculumId: lessonAllocation.curriculumId ?? null,
            learningLevelId: lessonAllocation.learningLevelId ?? null,
            subjectId: lessonAllocation.subjectId ?? null
        });
    };

    get f() {
        return this.lessonAllocationForm.controls;
    }

    closeLessonAllocationForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    resetFormControls() {
        this.editMode = false;
        this.lessonAllocationForm.reset();
    }

    onAcademicYearChange = () => {
        let academicYearId = this.lessonAllocationForm.get('academicYearId').value;
        this.academicYearChangedEvent.emit(academicYearId);
    };

    onCurriculumChange = () => {
        let curriculumId = this.lessonAllocationForm.get('curriculumId').value;
        this.curriculumChangedEvent.emit(curriculumId);
    };

    onLearningLevelChange = () => {
        let learningLevelId = this.lessonAllocationForm.get('learningLevelId').value;
        this.learningLevelChangedEvent.emit(learningLevelId);
    };

    onSubmit = () => {
        if (this.editMode) {
            let lessonAllocationId = this.lessonAllocation.id;
            this.lessonAllocation = new LessonAllocation(this.lessonAllocationForm.value);
            this.lessonAllocation.id = lessonAllocationId;
        } else {
            this.lessonAllocation = new LessonAllocation(this.lessonAllocationForm.value);
        }
        this.addItemEvent.emit(this.lessonAllocation);
    };
}
