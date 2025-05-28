import {SchoolClass} from '@/class/models/school-class';
import {AcademicYear} from '@/school/models/academic-year';
import {DateMonthYear} from '@/shared/models/date-month-year';
import {StudentClass} from '@/students/models/student-class';
import {DatePipe, formatDate} from '@angular/common';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup} from '@angular/forms';

@Component({
    selector: 'app-date-month-year-filter-form',
    templateUrl: './date-month-year-filter-form.component.html',
    styleUrl: './date-month-year-filter-form.component.scss'
})
export class DateMonthYearFilterFormComponent implements OnInit {
    @Input() months: number[] = [];
    @Input() years: number[] = [];
    @Input() studentClasses: StudentClass[] = [];

    @Input() showMonth: boolean = false;
    @Input() showYear: boolean = false;
    @Input() showDateFrom: boolean = false;
    @Input() showDateTo: boolean = false;
    @Input() showSchoolClass: boolean = false;

    @Output() searchItemEvent = new EventEmitter<DateMonthYear>();
    @Output() monthChangedEvent = new EventEmitter<number>();
    @Output() yearChangedEvent = new EventEmitter<Date>();
    @Output() dateFromChangedEvent = new EventEmitter<number>();
    @Output() dateToChangedEvent = new EventEmitter<number>();
    @Output() schoolClassChangedEvent = new EventEmitter<number>();

    dateMonthYearFilterForm: FormGroup;
    dmySearch: DateMonthYear;

    constructor(
        private formBuilder: FormBuilder,
        private dateP: DatePipe
    ) {}

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

    setFormControls = (dmySearch: DateMonthYear) => {
        this.dateMonthYearFilterForm.setValue({
            studentClassId: dmySearch.studentClassId ?? null,
            month: dmySearch.month ?? null,
            year: dmySearch.year ?? null,
            dateFrom: dmySearch.dateFrom
                ? formatDate(dmySearch.dateFrom, 'yyyy-MM-dd', 'en')
                : null,
            dateTo: dmySearch.dateTo
                ? formatDate(dmySearch.dateTo, 'yyyy-MM-dd', 'en')
                : null
        });
    };

    refreshItems = () => {
        this.dateMonthYearFilterForm = this.formBuilder.group({
            studentClassId: [null],
            month: [null],
            year: [null],
            dateFrom: [null],
            dateTo: [null]
        });
    };

    monthChanged = () => {
        let month = this.dateMonthYearFilterForm.get('month').value;
        this.monthChangedEvent.emit(month);
    };

    yearChanged = () => {
        let year = this.dateMonthYearFilterForm.get('year').value;
        this.yearChangedEvent.emit(year);
    };

    dateFromChanged = () => {
        let dateFrom = this.dateMonthYearFilterForm.get('dateFrom').value;
        this.dateFromChangedEvent.emit(dateFrom);
    };

    dateToChanged = () => {
        let dateTo = this.dateMonthYearFilterForm.get('dateTo').value;
        this.dateToChangedEvent.emit(dateTo);
    };

    studentClassChanged = () => {
        let studentClassId =
            this.dateMonthYearFilterForm.get('studentClassId').value;
        this.schoolClassChangedEvent.emit(studentClassId);
    };

    onSubmit = () => {
        this.dmySearch = new DateMonthYear(this.dateMonthYearFilterForm.value);
        this.searchItemEvent.emit(this.dmySearch);
    };
}
