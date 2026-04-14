import {Curriculum} from '@/academics/models/curriculum';
import {Subject} from '@/academics/models/subject';
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
    selector: 'app-strand-add-form',
    templateUrl: './strand-add-form.component.html',
    styleUrl: './strand-add-form.component.scss'
})
export class StrandAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;

    @Input() strand: Strand;
    @Input() subjects: Subject[];
    @Input() learningLevels: LearningLevel[];
    @Input() curricula: Curriculum[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() themes: any[] = [];
    @Input() curriculum: Curriculum;
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<Strand>();
    @Output() errorEvent = new EventEmitter<string>();
    @Output() curriculumChangedEvent = new EventEmitter<number>();
    @Output() academicYearChangedEvent = new EventEmitter<number>();
    @Output() learningLevelChangedEvent = new EventEmitter<number>();
    @Output() subjectChangedEvent = new EventEmitter<number>();

    strandForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.strandForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            code: [''],
            rank: [0, [Validators.required]],
            description: [''],
            academicYearId: [null],
            curriculumId: [null, [Validators.required]],
            subjectId: [null, [Validators.required]],
            learningLevelId: [null, [Validators.required]],
            themeId: [null]
        });
    };

    setFormControls = (strand: Strand) => {
        this.strandForm.setValue({
            name: strand.name,
            code: strand.code ?? '',
            rank: strand.rank,
            description: strand.description,
            academicYearId: strand.academicYearId ?? null,
            curriculumId: strand.curriculumId ?? null,
            subjectId: strand.subjectId ?? null,
            learningLevelId: strand.learningLevelId ?? null,
            themeId: strand.themeId ?? null
        });
    };

    get f() {
        return this.strandForm.controls;
    }

    closeStrandForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    viewItem = (strand: Strand, action: string) => {
        this.strand = strand;
        this.setFormControls(strand);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls() {
        this.editMode = false;
        this.strandForm.reset();
    }

    onAcademicYearChange = () => {
        let academicYearId = this.strandForm.get('academicYearId').value;
        this.academicYearChangedEvent.emit(academicYearId);
    };

    onCurriculumChange = () => {
        let curriculumId = this.strandForm.get('curriculumId').value;
        this.curriculumChangedEvent.emit(curriculumId);
    };

    onLearningLevelChange = () => {
        let learningLevelId = this.strandForm.get('learningLevelId').value;
        this.learningLevelChangedEvent.emit(learningLevelId);
    };

    onSubjectChange = () => {
        let subjectId = this.strandForm.get('subjectId').value;
        this.subjectChangedEvent.emit(subjectId);
    };

    onSubmit = () => {
        if (this.editMode) {
            let strandId = this.strand.id;
            this.strand = new Strand(this.strandForm.value);
            this.strand.id = strandId;
        } else {
            this.strand = new Strand(this.strandForm.value);
        }
        this.addItemEvent.emit(this.strand);
    };
}
