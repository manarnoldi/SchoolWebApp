import {Curriculum} from '@/academics/models/curriculum';
import {Status} from '@/core/enums/status';
import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import {LearningMode} from '@/school/models/learning-mode';
import {EmploymentType} from '@/settings/models/employment-type';
import {StaffCategory} from '@/settings/models/staff-category';
import {CurriculumYearPerson} from '@/shared/models/curriculum-year-person';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';

@Component({
    selector: 'app-curriculum-year-filter-form',
    templateUrl: './curriculum-year-filter-form.component.html',
    styleUrl: './curriculum-year-filter-form.component.scss'
})
export class CurriculumYearFilterFormComponent implements OnInit {
    @Input() curricula: Curriculum[] = [];
    @Input() educationLevels: EducationLevel[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() staffCategories: StaffCategory[] = [];
    @Input() employmentTypes: EmploymentType[] = [];
    @Input() learningModes: LearningMode[] = [];

    @Input() showCurriculum: boolean = false;
    @Input() showEducationLevel: boolean = false;
    @Input() showAcademicYear: boolean = false;
    @Input() showStaffCategory: boolean = false;
    @Input() showEmploymentType: boolean = false;
    @Input() showLearningMode: boolean = false;
    @Input() showPersonStatus: boolean = false;

    @Output() searchItemEvent = new EventEmitter<CurriculumYearPerson>();
    @Output() curriculumChangedEvent = new EventEmitter<number>();
    @Output() educationLevelChangedEvent = new EventEmitter<number>();
    @Output() academicYearChangedEvent = new EventEmitter<number>();
    @Output() staffCategoryChangedEvent = new EventEmitter<number>();
    @Output() employmentTypeChangedEvent = new EventEmitter<number>();
    @Output() learningModeChangedEvent = new EventEmitter<number>();
    @Output() statusChangedEvent = new EventEmitter<number>();

    curriculumYearStaffFilterForm: FormGroup;
    cysSearch: CurriculumYearPerson;
    statuses;
    status = Status;

    constructor(private formBuilder: FormBuilder) {
        this.statuses = Object.keys(Status).filter((k) =>
            isNaN(Number(k))
        );
    }

    ngOnInit(): void {
        this.refreshItems();
    }

    setFormControls = (cysSearch: CurriculumYearPerson) => {
        this.curriculumYearStaffFilterForm.setValue({
            curriculumId: cysSearch.curriculumId ?? null,
            educationLevelId: cysSearch.educationLevelId ?? null,
            academicYearId: cysSearch.academicYearId ?? null,
            staffCategoryId: cysSearch.staffCategoryId ?? null,
            employmentTypeId: cysSearch.employmentTypeId ?? null,
            learningModeId: cysSearch.learningModeId ?? null,
            status: cysSearch.status ?? null
        });
    };

    refreshItems = () => {
        this.curriculumYearStaffFilterForm = this.formBuilder.group({
            curriculumId: [null],
            educationLevelId: [null],
            academicYearId: [null],
            staffCategoryId: [null],
            employmentTypeId: [null],
            learningModeId: [null],
            status: [null]
        });
    };

    curriculumChanged = () => {
        let curriculumId =
            this.curriculumYearStaffFilterForm.get('curriculumId').value;
        this.curriculumChangedEvent.emit(curriculumId);
    };

    educationLevelChanged = () => {
        let educationLevelId =
            this.curriculumYearStaffFilterForm.get('educationLevelId').value;
        this.educationLevelChangedEvent.emit(educationLevelId);
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
        this.learningModeChangedEvent.emit(learningModeId);
    };

    statusChanged = () => {
        let status = this.curriculumYearStaffFilterForm.get('status').value;
        this.statusChangedEvent.emit(status);
    };

    onSubmit = () => {
        this.cysSearch = new CurriculumYearPerson(
            this.curriculumYearStaffFilterForm.value
        );
        this.searchItemEvent.emit(this.cysSearch);
    };
}
