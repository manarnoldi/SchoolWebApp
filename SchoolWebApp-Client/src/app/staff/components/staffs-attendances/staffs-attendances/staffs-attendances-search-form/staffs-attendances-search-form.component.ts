import {EmploymentType} from '@/settings/models/employment-type';
import {StaffCategory} from '@/settings/models/staff-category';
import {StaffAttendanceSearch} from '@/staff/models/staff-attendance-search';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';

@Component({
    selector: 'app-staffs-attendances-search-form',
    templateUrl: './staffs-attendances-search-form.component.html',
    styleUrl: './staffs-attendances-search-form.component.scss'
})
export class StaffsAttendancesSearchFormComponent implements OnInit {
    @Input() staffCategories: StaffCategory[] = [];
    @Input() employmentTypes: EmploymentType[] = [];

    @Output() searchItemEvent = new EventEmitter<StaffAttendanceSearch>();
    @Output() employmentTypeChangedEvent = new EventEmitter<number>();
    @Output() staffCategoryChangedEvent = new EventEmitter<number>();
    @Output() dateChangedEvent = new EventEmitter<Date>();

    staffAttendanceSearchForm: FormGroup;
    saSearch: StaffAttendanceSearch;
    today = new Date().toISOString().split('T')[0];

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.staffAttendanceSearchForm = this.formBuilder.group({
            staffCategoryId: [null],
            employmentTypeId: [null],
            attendanceDate: [this.today]
        });
    };

    staffCategoryChanged = () => {
        let staffCategoryId =
            this.staffAttendanceSearchForm.get('staffCategoryId').value;
        this.staffCategoryChangedEvent.emit(staffCategoryId);
    };

    employmentTypeChanged = () => {
        let employmentTypeId =
            this.staffAttendanceSearchForm.get('employmentTypeId').value;
        this.employmentTypeChangedEvent.emit(employmentTypeId);
    };

    onSubmit = () => {
        this.saSearch = new StaffAttendanceSearch(
            this.staffAttendanceSearchForm.value
        );
        this.searchItemEvent.emit(this.saSearch);
    };

    dateChanged = () => {
        let selectedDate =
            this.staffAttendanceSearchForm.get('attendanceDate').value;
        this.dateChangedEvent.emit(selectedDate);
    };
}
