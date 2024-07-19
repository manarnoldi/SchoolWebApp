import {LearningLevel} from '@/class/models/learning-level';
import {AcademicYear} from '@/school/models/academic-year';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolStream} from '@/class/models/school-stream';
import {
    AfterViewInit,
    Component,
    ElementRef,
    EventEmitter,
    Input,
    OnInit,
    Output,
    ViewChild
} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {YearClassStreamComponent} from '@/shared/directives/year-class-stream/year-class-stream.component';

@Component({
    selector: 'app-school-class-add-form',
    templateUrl: './school-class-add-form.component.html',
    styleUrl: './school-class-add-form.component.scss'
})
export class SchoolClassAddFormComponent implements  AfterViewInit,OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;
    @Input() schoolClass: SchoolClass;
    @Input() learningLevels: LearningLevel[] = [];
    @Input() schoolStreams: SchoolStream[] = [];
    @Input() academicYears: AcademicYear[] = [];

    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<SchoolClass>();
    @Output() errorEvent = new EventEmitter<string>();

    @ViewChild(YearClassStreamComponent)
    yearClassStreamComponent: YearClassStreamComponent;

    schoolClassForm: FormGroup;
    constructor(private formBuilder: FormBuilder) {}
    
    ngAfterViewInit(): void {
        this.yearClassStreamComponent.initializeFormControl();
    }

    ngOnInit(): void {
        this.initializeForm();
    }

    initializeForm = () => {
        this.schoolClassForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            description: ['']
        });
        
    };

    setFormControls = (schoolClass: SchoolClass) => {
        this.schoolClassForm.patchValue({
            name: schoolClass?.name,
            description: schoolClass?.description
        });
        this.yearClassStreamComponent.setFormControls(schoolClass);
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
