import {Curriculum} from '@/academics/models/curriculum';
import {AcademicYear} from '@/school/models/academic-year';
import {LearningMode} from '@/school/models/learning-mode';
import {EmploymentType} from '@/settings/models/employment-type';
import {StaffCategory} from '@/settings/models/staff-category';
import {CurriculumYearStaff} from '@/shared/models/curriculum-year-staff';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';

@Component({
    selector: 'app-curriculum-year-filter-form',
    templateUrl: './curriculum-year-filter-form.component.html',
    styleUrl: './curriculum-year-filter-form.component.scss'
})
export class CurriculumYearFilterFormComponent implements OnInit {
    @Input() curricula: Curriculum[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() staffCategories: StaffCategory[] = [];
    @Input() employmentTypes: EmploymentType[] = [];
    @Input() learningModes: LearningMode[] = [];

    @Input() showCurriculum: boolean = false;
    @Input() showAcademicYear: boolean = false;
    @Input() showStaffCategory: boolean = false;
    @Input() showEmploymentType: boolean = false;
    @Input() showLearningMode: boolean = false;

    @Output() searchItemEvent = new EventEmitter<CurriculumYearStaff>();
    @Output() curriculumChangedEvent = new EventEmitter<number>();
    @Output() academicYearChangedEvent = new EventEmitter<number>();
    @Output() staffCategoryChangedEvent = new EventEmitter<number>();
    @Output() employmentTypeChangedEvent = new EventEmitter<number>();
    @Output() learningModeChangedChangedEvent = new EventEmitter<number>();

    curriculumYearStaffFilterForm: FormGroup;
    cysSearch: CurriculumYearStaff;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    setFormControls = (cysSearch: CurriculumYearStaff) => {
        this.curriculumYearStaffFilterForm.setValue({
            curriculumId: cysSearch.curriculumId ?? null,
            academicYearId: cysSearch.academicYearId ?? null,
            staffCategoryId: cysSearch.staffCategoryId ?? null,
            employmentTypeId: cysSearch.employmentTypeId ?? null,
            learningModeId: cysSearch.learningModeId ?? null
        });
    };

    refreshItems = () => {
        this.curriculumYearStaffFilterForm = this.formBuilder.group({
            curriculumId: [null],
            academicYearId: [null],
            staffCategoryId: [null],
            employmentTypeId: [null],
            learningModeId: [null]
        });
    };

    curriculumChanged = () => {
        let curriculumId =
            this.curriculumYearStaffFilterForm.get('curriculumId').value;
        this.curriculumChangedEvent.emit(curriculumId);
    };

    academicYearChanged = () => {
        let academicYearId =
            this.curriculumYearStaffFilterForm.get('academicYearId').value;
        this.academicYearChangedEvent.emit(academicYearId);
    };

    staffCategoryChanged = () => {
        let staffCategoryId =
            this.curriculumYearStaffFilterForm.get('staffCategoryId').value;
        this.staffCategoryChangedEvent.emit(staffCategoryId);
    };

    employmentTypeChanged = () => {
        let employmentTypeId =
            this.curriculumYearStaffFilterForm.get('employmentTypeId').value;
        this.employmentTypeChangedEvent.emit(employmentTypeId);
    };

    learningModeChanged = () => {
        let learningModeId =
            this.curriculumYearStaffFilterForm.get('learningModeId').value;
        this.academicYearChangedEvent.emit(learningModeId);
    };

    onSubmit = () => {
        this.cysSearch = new CurriculumYearStaff(
            this.curriculumYearStaffFilterForm.value
        );
        this.searchItemEvent.emit(this.cysSearch);
    };
}
