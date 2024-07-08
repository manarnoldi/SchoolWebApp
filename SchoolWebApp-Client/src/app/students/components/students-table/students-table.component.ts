import {Status} from '@/core/enums/status';
import {StudentDetails} from '@/students/models/student-details';
import {
    AfterViewInit,
    Component,
    EventEmitter,
    Input,
    OnDestroy,
    OnInit,
    Output
} from '@angular/core';
import {Config} from 'protractor';

import {Subject} from 'rxjs';

@Component({
    selector: 'app-students-table',
    templateUrl: './students-table.component.html',
    styleUrl: './students-table.component.scss'
})
export class StudentsTableComponent
    implements AfterViewInit, OnDestroy, OnInit
{
    @Input() tableTitle: string = 'Students list';
    @Input() students: StudentDetails[] = [];
    @Input() showActionControls: Boolean = true;
    @Input() showLoginControls: Boolean = false;

    @Output() deleteItemEvent = new EventEmitter<number>();

    tableHeaders: string[] = [
        'Admission no',
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

    dtOptions: Config = {};
    dtTrigger: Subject<any> = new Subject();

    constructor() {
        this.statusValues = Object.keys(this.statusVals).filter((k) =>
            isNaN(Number(k))
        );
        this.dtOptions = {
            scrollX: true,
            columnDefs: [
                {width: 250, targets: 1},
                {width: 150, targets: 0},
                {width: 120, targets: 7}
            ]
        };
    }

    ngOnInit(): void {}

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
