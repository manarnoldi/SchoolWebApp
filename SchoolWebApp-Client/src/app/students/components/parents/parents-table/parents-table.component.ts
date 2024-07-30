import { Status } from '@/core/enums/status';
import { Parent } from '@/students/models/parent';
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
    selector: 'app-parents-table',
    templateUrl: './parents-table.component.html',
    styleUrl: './parents-table.component.scss'
})
export class ParentsTableComponent implements AfterViewInit, OnDestroy {
    @Input() tableTitle: string = 'Parents list';
    @Input() parents: Parent[] = [];
    @Input() showLoginControls: Boolean = false;
    @Input() showType: string = 'details';

    @Output() deleteItemEvent = new EventEmitter<number>();

    dtOptions: Config = {};
    dtTrigger: Subject<any> = new Subject();

    tableHeaders: string[] = [
        'Full name',
        'Occupation',
        'Nationality',
        'Gender',
        'Notifiable',
        'Payer',
        'Picker',
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
                {width: 250, targets: 0},
                {width: 170, targets: 8}
            ]
        };
    }

    deleteParent = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

    ngAfterViewInit(): void {
        this.dtTrigger.next(null);
    }

    ngOnDestroy(): void {
        this.dtTrigger.unsubscribe();
    }
}
