import {Exam} from '@/academics/models/exam';
import {ExamResult} from '@/academics/models/exam-result';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ToastrService} from 'ngx-toastr';

@Component({
    selector: 'app-exam-results-table',
    templateUrl: './exam-results-table.component.html',
    styleUrl: './exam-results-table.component.scss'
})
export class ExamResultsTableComponent {
    @Input() tableTitle: string = 'Examination results';
    @Input() examResults: ExamResult[] = [];
    @Input() pageSize: number = 10;
    // @Input() exams: Exam[] = [];
    @Input() isReport: boolean = false;

    @Output() deleteItemEvent = new EventEmitter<ExamResult>();

    page = 1;

    constructor(private toarst: ToastrService) {}

    validateNumber(event: Event, index: number, examMark: number) {
        const input = event.target as HTMLInputElement;
        if (input.value === null || input.value.trim() === '') {
            input.value = '';
            return;
        }
        if (isNaN(Number(input.value))) {
            this.toarst.error(`Entered mark ${input.value} is not a number!`);
            input.value = '';
            return;
        }

        if (parseFloat(input.value) > examMark) {
            this.toarst.error(
                `Entered mark ${input.value} is greater than exam mark ${examMark}!`
            );
            input.value = '';
            return;
        }
    }

    deleteItem = (examRes: ExamResult) => {
        this.deleteItemEvent.emit(examRes);
    };

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    tableHeaders: any[] = [
        {name: 'Class', show: this.isReport},
        {name: 'Subject', show: this.isReport},
        {name: 'Adm #', show: true},
        {name: 'Student name', show: true},
        {name: 'Subject name', show: !this.isReport},
        {name: 'Exam Type', show: true},
        {name: 'Exam Name', show: true},
        {name: 'Score', show: true},
        {name: 'Out of', show: true},
        {name: 'Contributing', show: true},
        {name: 'Action', show: !this.isReport}
    ];
}
