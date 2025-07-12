import {Status} from '@/core/enums/status';
import {StudentDetails} from '@/students/models/student-details';
import {
    AfterViewInit,
    Component,
    EventEmitter,
    Input,
    OnDestroy,
    Output
} from '@angular/core';
import {Config} from 'protractor';

import {Subject} from 'rxjs';

@Component({
    selector: 'app-students-table',
    templateUrl: './students-table.component.html',
    styleUrl: './students-table.component.scss'
})
export class StudentsTableComponent implements AfterViewInit, OnDestroy {
    @Input() tableTitle: string = 'Students list';
    @Input() students: StudentDetails[] = [];
    @Input() showLoginControls: Boolean = false;
    @Input() showType: string = 'details';
    @Input() querySource: string = '';
    @Input() status: Status = Status.Active;

    @Output() deleteItemEvent = new EventEmitter<number>();
 statusT = Status;
    dtOptions: Config = {};
    dtTrigger: Subject<any> = new Subject();

    tableHeaders: string[] = [
        'Ref#',
        'Adm#',
        'Full name',
        'Admission date',
        'Learning mode',
        'Nationality',
        'Gender',
        'Status',
        'Action'
    ];

    statusVals = Status;
    statusValues;

    constructor() {
        this.statusValues = Object.keys(this.statusVals).filter((k) =>
            isNaN(Number(k))
        );
        this.dtOptions = {
            scrollX: true,
            columnDefs: [
                {width: 250, targets: 2},
                {width: 70, targets: 1},
                {width: 170, targets: 8},
                {width: 50, targets: 0}
            ]
        };
    }

    deleteStudent = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

    ngAfterViewInit(): void {
        this.dtTrigger.next(null);
    }

    ngOnDestroy(): void {
        this.dtTrigger.unsubscribe();
    }
}
