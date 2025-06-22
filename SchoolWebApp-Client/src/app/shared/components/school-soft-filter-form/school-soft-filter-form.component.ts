import { Curriculum } from '@/academics/models/curriculum';
import { SchoolClass } from '@/class/models/school-class';
import { Status } from '@/core/enums/status';
import { AcademicYear } from '@/school/models/academic-year';
import { EducationLevel } from '@/school/models/educationLevel';
import { LearningMode } from '@/school/models/learning-mode';
import { EmploymentType } from '@/settings/models/employment-type';
import { StaffCategory } from '@/settings/models/staff-category';
import { SchoolSoftFilter } from '@/shared/models/school-soft-filter';
import { StaffDetails } from '@/staff/models/staff-details';
import { StudentClass } from '@/students/models/student-class';
import { formatDate } from '@angular/common';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-school-soft-filter-form',
  templateUrl: './school-soft-filter-form.component.html',
  styleUrl: './school-soft-filter-form.component.scss'
})
export class SchoolSoftFilterFormComponent implements OnInit {
    @Input() curricula: Curriculum[] = [];
    @Input() educationLevels: EducationLevel[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() staffCategories: StaffCategory[] = [];
    @Input() employmentTypes: EmploymentType[] = [];
    @Input() learningModes: LearningMode[] = [];

    @Input() months: number[] = [];
    @Input() years: number[] = [];
    @Input() studentClasses: StudentClass[] = [];
    @Input() schoolClasses: SchoolClass[] = [];
    @Input() staffDetails: StaffDetails[] = [];

    @Input() showCurriculum: boolean = false;
    @Input() showEducationLevel: boolean = false;
    @Input() showAcademicYear: boolean = false;
    @Input() showEmploymentType: boolean = false;
    @Input() showLearningMode: boolean = false;
    @Input() showPersonStatus: boolean = false;
    @Input() showStaffCategory: boolean = false;
    @Input() showSchoolClass: boolean = false;

    @Input() showMonth: boolean = false;
    @Input() showYear: boolean = false;
    @Input() showDateFrom: boolean = false;
    @Input() showDateTo: boolean = false;
    @Input() showStudentClass: boolean = false;
    @Input() showStaffDetails: boolean = false;

    @Output() searchItemEvent = new EventEmitter<SchoolSoftFilter>();

    @Output() curriculumChangedEvent = new EventEmitter<number>();
    @Output() educationLevelChangedEvent = new EventEmitter<number>();
    @Output() academicYearChangedEvent = new EventEmitter<number>();
    @Output() staffCategoryChangedEvent = new EventEmitter<number>();
    @Output() employmentTypeChangedEvent = new EventEmitter<number>();
    @Output() learningModeChangedEvent = new EventEmitter<number>();
    @Output() statusChangedEvent = new EventEmitter<number>();

    @Output() monthChangedEvent = new EventEmitter<number>();
    @Output() yearChangedEvent = new EventEmitter<Date>();
    @Output() dateFromChangedEvent = new EventEmitter<number>();
    @Output() dateToChangedEvent = new EventEmitter<number>();
    @Output() studentClassChangedEvent = new EventEmitter<number>();
    @Output() schoolClassChangedEvent = new EventEmitter<number>();
    @Output() staffDetailsChangedEvent = new EventEmitter<number>();

    schoolSoftFilterForm: FormGroup;
    cysSearch: SchoolSoftFilter;
    statuses;
    status = Status;

    constructor(private formBuilder: FormBuilder) {
        this.statuses = Object.keys(Status).filter((k) => isNaN(Number(k)));
    }

    ngOnInit(): void {
        this.refreshItems();
    }

    readonly monthNames = [
        'January',
        'February',
        'March',
        'April',
        'May',
        'June',
        'July',
        'August',
        'September',
        'October',
        'November',
        'December'
    ];

    getMonthName(month: number): string {
        return this.monthNames[month - 1];
    }

    setFormControls = (cysSearch: SchoolSoftFilter) => {
        this.schoolSoftFilterForm.setValue({
            curriculumId: cysSearch.curriculumId ?? null,
            educationLevelId: cysSearch.educationLevelId ?? null,
            academicYearId: cysSearch.academicYearId ?? null,
            staffCategoryId: cysSearch.staffCategoryId ?? null,
            employmentTypeId: cysSearch.employmentTypeId ?? null,
            learningModeId: cysSearch.learningModeId ?? null,
            status: cysSearch.status ?? null,
            studentClassId: cysSearch.studentClassId ?? null,
            schoolClassId: cysSearch.schoolClassId ?? null,
            staffDetailsId: cysSearch.staffDetailsId ?? null,
            month: cysSearch.month ?? null,
            year: cysSearch.year ?? null,
            dateFrom: cysSearch.dateFrom
                ? formatDate(cysSearch.dateFrom, 'yyyy-MM-dd', 'en')
                : null,
            dateTo: cysSearch.dateTo
                ? formatDate(cysSearch.dateTo, 'yyyy-MM-dd', 'en')
                : null
        });
    };

    refreshItems = () => {
        this.schoolSoftFilterForm = this.formBuilder.group({
            curriculumId: [null],
            educationLevelId: [null],
            academicYearId: [null],
            staffCategoryId: [null],
            employmentTypeId: [null],
            learningModeId: [null],
            status: [null],
            studentClassId: [null],
            schoolClassId: [null],
            staffDetailsId: [null],
            month: [null],
            year: [null],
            dateFrom: [null],
            dateTo: [null]
        });
    };

    curriculumChanged = () => {
        let curriculumId =
            this.schoolSoftFilterForm.get('curriculumId').value;
        this.curriculumChangedEvent.emit(curriculumId);
    };

    educationLevelChanged = () => {
        let educationLevelId =
            this.schoolSoftFilterForm.get('educationLevelId').value;
        this.educationLevelChangedEvent.emit(educationLevelId);
    };

    academicYearChanged = () => {
        let academicYearId =
            this.schoolSoftFilterForm.get('academicYearId').value;
        this.academicYearChangedEvent.emit(academicYearId);
    };

    staffCategoryChanged = () => {
        let staffCategoryId =
            this.schoolSoftFilterForm.get('staffCategoryId').value;
        this.staffCategoryChangedEvent.emit(staffCategoryId);
    };

    employmentTypeChanged = () => {
        let employmentTypeId =
            this.schoolSoftFilterForm.get('employmentTypeId').value;
        this.employmentTypeChangedEvent.emit(employmentTypeId);
    };

    learningModeChanged = () => {
        let learningModeId =
            this.schoolSoftFilterForm.get('learningModeId').value;
        this.learningModeChangedEvent.emit(learningModeId);
    };

    statusChanged = () => {
        let status = this.schoolSoftFilterForm.get('status').value;
        this.statusChangedEvent.emit(status);
    };

    monthChanged = () => {
        let month = this.schoolSoftFilterForm.get('month').value;
        this.monthChangedEvent.emit(month);
    };

    yearChanged = () => {
        let year = this.schoolSoftFilterForm.get('year').value;
        this.yearChangedEvent.emit(year);
    };

    dateFromChanged = () => {
        let dateFrom = this.schoolSoftFilterForm.get('dateFrom').value;
        this.dateFromChangedEvent.emit(dateFrom);
    };

    dateToChanged = () => {
        let dateTo = this.schoolSoftFilterForm.get('dateTo').value;
        this.dateToChangedEvent.emit(dateTo);
    };

    studentClassChanged = () => {
        let studentClassId =
            this.schoolSoftFilterForm.get('studentClassId').value;
        this.studentClassChangedEvent.emit(studentClassId);
    };

    schoolClassChanged = () => {
        let schoolClassId =
            this.schoolSoftFilterForm.get('schoolClassId').value;
        this.studentClassChangedEvent.emit(schoolClassId);
    };

    staffDetailsChanged = () => {
        let staffDetailsId =
            this.schoolSoftFilterForm.get('staffDetailsId').value;
        this.staffDetailsChangedEvent.emit(staffDetailsId);
    };

    onSubmit = () => {
        this.cysSearch = new SchoolSoftFilter(
            this.schoolSoftFilterForm.value
        );
        this.searchItemEvent.emit(this.cysSearch);
    };

}
