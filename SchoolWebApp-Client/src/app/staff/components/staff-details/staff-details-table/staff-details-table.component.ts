import {StaffDetails} from '@/staff/models/staff-details';
import {
    AfterViewInit,
    ChangeDetectorRef,
    Component,
    EventEmitter,
    Input,
    OnDestroy,
    Output
} from '@angular/core';
import {Config} from 'protractor';
import {Subject} from 'rxjs';

@Component({
    selector: 'app-staff-details-table',
    templateUrl: './staff-details-table.component.html',
    styleUrl: './staff-details-table.component.scss'
})
export class StaffDetailsTableComponent implements AfterViewInit, OnDestroy {
    @Input() tableTitle: string = 'Staff list';
    @Input() staffs: StaffDetails[] = [];
    @Input() showLoginControls: Boolean = false;
    @Input() showType: string = "details";

    @Output() deleteItemEvent = new EventEmitter<number>();

    dtOptions: Config = {};
    dtTrigger: Subject<any> = new Subject();

    tableHeaders: string[] = [        
        'Full name',
        'Staff category',
        'Designation',
        'Employment type',
        'Phone number',
        'Employment date',
        'Nationality',
        'Action'
    ];

    constructor() {
        this.dtOptions = {
            scrollX: true,
            columnDefs: [
                {width: 150, targets: 7},
                {width: 250, targets: 0}
            ]
        };
    }

    deleteStaff = (id: number) => {
        this.deleteItemEvent.emit(id);
    };

    ngAfterViewInit(): void {
        this.dtTrigger.next(null);
    }

    ngOnDestroy(): void {
        this.dtTrigger.unsubscribe();
    }
}
