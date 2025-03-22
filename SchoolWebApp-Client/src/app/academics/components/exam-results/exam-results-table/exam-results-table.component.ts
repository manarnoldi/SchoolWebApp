import {Exam} from '@/academics/models/exam';
import {ExamResult} from '@/academics/models/exam-result';
import {ExamType} from '@/academics/models/exam-type';
import {Subject} from '@/academics/models/subject';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {StudentDetails} from '@/students/models/student-details';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-exam-results-table',
    templateUrl: './exam-results-table.component.html',
    styleUrl: './exam-results-table.component.scss'
})
export class ExamResultsTableComponent implements OnInit {
    @Input() tableTitle: string = 'Examination results';
    @Input() examResults: ExamResult[] = [];
    @Input() exams: Exam[] = [];

    @Output() deleteItemEvent = new EventEmitter<ExamResult>();

    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    constructor(
        private tableSettingsSvc: TableSettingsService,
        private toarst: ToastrService
    ) {}

    ngOnInit(): void {
        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );
    }

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

    tableHeaders: string[] = [
        'Adm #',
        'Student name',
        'Subject name',
        'Exam Type',
        'Exam Name',
        'Score',
        'Out of',
        'Contributing',
        'Action'
    ];
}
