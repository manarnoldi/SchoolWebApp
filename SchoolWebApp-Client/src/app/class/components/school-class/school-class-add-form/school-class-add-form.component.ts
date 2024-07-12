import { LearningLevel } from '@/class/models/learning-level';
import {AcademicYear} from '@/school/models/academic-year';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolStream} from '@/class/models/school-stream';
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
    selector: 'app-school-class-add-form',
    templateUrl: './school-class-add-form.component.html',
    styleUrl: './school-class-add-form.component.scss'
})
export class SchoolClassAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() schoolClass: SchoolClass;
    @Input() learningLevels: LearningLevel[] = [];
    @Input() schoolStreams: SchoolStream[] = [];
    @Input() academicYears: AcademicYear[] = [];

    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<SchoolClass>();
    @Output() errorEvent = new EventEmitter<string>();

    schoolClassForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.initializeForm();
    }

    initializeForm = () => {
        this.schoolClassForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            description: [''],
            learningLevelId: [null, [Validators.required]],
            schoolStreamId: [null, [Validators.required]],
            academicYearId: [null, [Validators.required]]
        });
    };

    setFormControls = (schoolClass: SchoolClass) => {
        this.schoolClassForm.setValue({
            name: schoolClass?.name,
            description: schoolClass?.description,
            learningLevelId: schoolClass?.learningLevelId ?? null,
            schoolStreamId: schoolClass?.schoolStreamId ?? null,
            academicYearId: schoolClass?.academicYearId ?? null
        });
    };

    get f() {
        return this.schoolClassForm.controls;
    }

    closeSchoolClassForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    viewItem = (schoolClass: SchoolClass, action: string) => {
        this.schoolClass = schoolClass;
        this.setFormControls(schoolClass);
        if (action == 'View') {
            this.editMode = false;
        } else if (action == 'Edit') {
            this.editMode = true;
        }
    };

    resetFormControls = () => {
        this.editMode = false;
        this.schoolClassForm.reset();
    };

    onSubmit = () => {
        let schoolClassId = this.schoolClass?.id;
        this.schoolClass = new SchoolClass(this.schoolClassForm.value);
        if (this.editMode) this.schoolClass.id = schoolClassId;
        this.addItemEvent.emit(this.schoolClass);
    };
}
