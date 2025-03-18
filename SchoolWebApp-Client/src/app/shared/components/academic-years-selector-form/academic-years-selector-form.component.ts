import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import {EducationLevelYear} from '@/shared/models/education-level-year';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';

@Component({
    selector: 'app-academic-years-selector-form',
    templateUrl: './academic-years-selector-form.component.html',
    styleUrl: './academic-years-selector-form.component.scss'
})
export class AcademicYearsSelectorFormComponent implements OnInit {
    @Input() academicYears: AcademicYear[] = [];
    @Input() educationLevels: EducationLevel[] = [];
    @Output() searchItemEvent = new EventEmitter<EducationLevelYear>();
    @Output() clearListEvent = new EventEmitter<void>();

    academicYearSelectorForm: FormGroup;
    educationLevelYear: EducationLevelYear;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.academicYearSelectorForm = this.formBuilder.group({
            academicYearId: [null],
            educationLevelId: [null]
        });
    };

    setFormControls = (el: EducationLevelYear) => {
        this.academicYearSelectorForm.setValue({
            academicYearId: el?.academicYearId ?? null,
            educationLevelId: el?.educationLevelId ?? null
        });
    };

    clearList = () => {
        this.clearListEvent.emit();
    };

    onSubmit = () => {
        this.educationLevelYear = new EducationLevelYear(
            this.academicYearSelectorForm.value
        );
        this.searchItemEvent.emit(this.educationLevelYear);
    };

    academicYearEdulevelChanged = () => {
        this.clearListEvent.emit();
    };
}
