import {Curriculum} from '@/academics/models/curriculum';
import {Subject} from '@/academics/models/subject';
import {Theme} from '@/cbe/assessments/models/theme';
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
    selector: 'app-theme-add-form',
    templateUrl: './theme-add-form.component.html',
    styleUrl: './theme-add-form.component.scss'
})
export class ThemeAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;

    @Input() theme: Theme;
    @Input() subjects: Subject[];
    @Input() learningLevels: LearningLevel[];
    @Input() curricula: Curriculum[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() curriculum: Curriculum;
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<Theme>();
    @Output() errorEvent = new EventEmitter<string>();
    @Output() curriculumChangedEvent = new EventEmitter<number>();
    @Output() academicYearChangedEvent = new EventEmitter<number>();
    @Output() learningLevelChangedEvent = new EventEmitter<number>();
    @Output() subjectChangedEvent = new EventEmitter<number>();

    themeForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.themeForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            code: [''],
            rank: [0, [Validators.required]],
            description: [''],
            academicYearId: [null, [Validators.required]],
            curriculumId: [null, [Validators.required]],
            subjectId: [null, [Validators.required]],
            learningLevelId: [null, [Validators.required]]
        });
    };

    setFormControls = (theme: Theme) => {
        this.themeForm.setValue({
            name: theme.name,
            code: theme.code ?? '',
            rank: theme.rank,
            description: theme.description,
            academicYearId: null,
            curriculumId: theme.curriculumId ?? null,
            subjectId: theme.subjectId ?? null,
            learningLevelId: theme.learningLevelId ?? null
        });
    };

    get f() {
        return this.themeForm.controls;
    }

    closeThemeForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    viewItem = (theme: Theme, action: string) => {
        this.theme = theme;
        this.setFormControls(theme);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls() {
        this.editMode = false;
        this.themeForm.reset();
    }

    onAcademicYearChange = () => {
        let academicYearId = this.themeForm.get('academicYearId').value;
        this.academicYearChangedEvent.emit(academicYearId);
    };

    onCurriculumChange = () => {
        let curriculumId = this.themeForm.get('curriculumId').value;
        this.curriculumChangedEvent.emit(curriculumId);
    };

    onLearningLevelChange = () => {
        let learningLevelId = this.themeForm.get('learningLevelId').value;
        this.learningLevelChangedEvent.emit(learningLevelId);
    };

    onSubjectChange = () => {
        let subjectId = this.themeForm.get('subjectId').value;
        this.subjectChangedEvent.emit(subjectId);
    };

    onSubmit = () => {
        if (this.editMode) {
            let themeId = this.theme.id;
            this.theme = new Theme(this.themeForm.value);
            this.theme.id = themeId;
        } else {
            this.theme = new Theme(this.themeForm.value);
        }
        this.addItemEvent.emit(this.theme);
    };
}
