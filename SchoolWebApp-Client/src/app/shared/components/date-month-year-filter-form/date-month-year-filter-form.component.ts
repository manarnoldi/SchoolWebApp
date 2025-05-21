import {DateMonthYear} from '@/shared/models/date-month-year';
import {DatePipe} from '@angular/common';
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

    @Input() showMonth: boolean = false;
    @Input() showYear: boolean = false;
    @Input() showDateFrom: boolean = false;
    @Input() showDateTo: boolean = false;

    @Output() searchItemEvent = new EventEmitter<DateMonthYear>();
    @Output() monthChangedEvent = new EventEmitter<number>();
    @Output() yearChangedEvent = new EventEmitter<Date>();
    @Output() dateFromChangedEvent = new EventEmitter<number>();
    @Output() dateToChangedEvent = new EventEmitter<number>();

    dateMonthYearFilterForm: FormGroup;
    dmySearch: DateMonthYear;

    constructor(
        private formBuilder: FormBuilder,
        private dateP: DatePipe
    ) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    setFormControls = (dmySearch: DateMonthYear) => {
        this.dateMonthYearFilterForm.setValue({
            month: dmySearch.month ?? null,
            year: dmySearch.year ?? null,
            dateFrom: dmySearch.dateFrom ?? null,
            dateTo: dmySearch.dateTo ?? null
        });
    };

    refreshItems = () => {
        this.dateMonthYearFilterForm = this.formBuilder.group({
            month: [null],
            year: [null],
            dateFrom: [null],
            dateTo: [null]
        });
    };

    monthChanged = () => {
        let month = this.dateMonthYearFilterForm.get('month').value;
        if (!month || month == '') return;
        this.monthChangedEvent.emit(month);
    };

    yearChanged = () => {
        let year = this.dateMonthYearFilterForm.get('year').value;
        if (!year || year == '') return;
        this.yearChangedEvent.emit(year);
    };

    dateFromChanged = () => {
        let dateFrom = this.dateMonthYearFilterForm.get('dateFrom').value;
        if (!dateFrom || dateFrom == '') return;
        this.dateFromChangedEvent.emit(dateFrom);
    };

    dateToChanged = () => {
        let dateTo = this.dateMonthYearFilterForm.get('dateTo').value;
        if (!dateTo || dateTo == '') return;
        this.dateToChangedEvent.emit(dateTo);
    };

    onSubmit = () => {
        this.dmySearch = new DateMonthYear(this.dateMonthYearFilterForm.value);
        this.searchItemEvent.emit(this.dmySearch);
    };
}
