import {Curriculum} from '@/academics/models/curriculum';
import {Subject} from '@/academics/models/subject';
import {SubStrand} from '@/cbe/assessments/models/sub-strand';
import {Strand} from '@/cbe/assessments/models/strand';
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
    selector: 'app-sub-strand-add-form',
    templateUrl: './sub-strand-add-form.component.html',
    styleUrl: './sub-strand-add-form.component.scss'
})
export class SubStrandAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;

    @Input() subStrand: SubStrand;
    @Input() strands: Strand[];
    @Input() subjects: Subject[];
    @Input() learningLevels: LearningLevel[];
    @Input() curricula: Curriculum[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<SubStrand>();
    @Output() errorEvent = new EventEmitter<string>();
    @Output() curriculumChangedEvent = new EventEmitter<number>();
    @Output() academicYearChangedEvent = new EventEmitter<number>();
    @Output() learningLevelChangedEvent = new EventEmitter<number>();
    @Output() subjectChangedEvent = new EventEmitter<number>();

    subStrandForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.subStrandForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            code: [''],
            rank: [0, [Validators.required]],
            description: [''],
            academicYearId: [null, [Validators.required]],
            curriculumId: [null, [Validators.required]],
            subjectId: [null, [Validators.required]],
            learningLevelId: [null, [Validators.required]],
            strandId: [null, [Validators.required]]
        });
    };

    setFormControls = (subStrand: SubStrand) => {
        this.subStrandForm.setValue({
            name: subStrand.name,
            code: subStrand.code ?? '',
            rank: subStrand.rank,
            description: subStrand.description,
            academicYearId: subStrand.academicYearId ?? null,
            curriculumId: subStrand.curriculumId ?? null,
            subjectId: subStrand.subjectId ?? null,
            learningLevelId: subStrand.learningLevelId ?? null,
            strandId: subStrand.strandId ?? null
        });
    };

    get f() {
        return this.subStrandForm.controls;
    }

    closeSubStrandForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    resetFormControls() {
        this.editMode = false;
        this.subStrandForm.reset();
    }

    onAcademicYearChange = () => {
        let academicYearId = this.subStrandForm.get('academicYearId').value;
        this.academicYearChangedEvent.emit(academicYearId);
    };

    onCurriculumChange = () => {
        let curriculumId = this.subStrandForm.get('curriculumId').value;
        this.curriculumChangedEvent.emit(curriculumId);
    };

    onLearningLevelChange = () => {
        let learningLevelId = this.subStrandForm.get('learningLevelId').value;
        this.learningLevelChangedEvent.emit(learningLevelId);
    };

    onSubjectChange = () => {
        let subjectId = this.subStrandForm.get('subjectId').value;
        this.subjectChangedEvent.emit(subjectId);
    };

    onSubmit = () => {
        if (this.editMode) {
            let subStrandId = this.subStrand.id;
            this.subStrand = new SubStrand(this.subStrandForm.value);
            this.subStrand.id = subStrandId;
        } else {
            this.subStrand = new SubStrand(this.subStrandForm.value);
        }
        this.addItemEvent.emit(this.subStrand);
    };
}
