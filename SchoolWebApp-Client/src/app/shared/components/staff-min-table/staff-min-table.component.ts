import {Status} from '@/core/enums/status';
import {StaffDetails} from '@/staff/models/staff-details';
import {
    Component,
    ElementRef,
    EventEmitter,
    Input,
    OnInit,
    Output,
    ViewChild
} from '@angular/core';

@Component({
    selector: 'app-staff-min-table',
    templateUrl: './staff-min-table.component.html',
    styleUrl: './staff-min-table.component.scss'
})
export class StaffMinTableComponent implements OnInit {
    @Input() showOthers: Boolean = false;
    @Input() staff: StaffDetails[] = [];
    @Input() tableTitle: string = 'Staff list';
    @Input() showCheckBoxes: boolean = false;
    @Input() checkBoxDisabled: Boolean = false;
    // Optional map of staffId -> subject count for the current report year.
    // When provided, a clickable badge is rendered next to each staff name
    // that emits staffClickedEvent on click (same as clicking the name).
    @Input() subjectCounts: Record<number, number> | null = null;
    // Highlights the row whose staff.id matches this value. Parent typically
    // updates it to the last-clicked staff so the selection is visually obvious
    // while the right-hand subjects pane is loaded.
    @Input() selectedId: number | string | null = null;

    @Output() staffClickedEvent = new EventEmitter<number>();

    @ViewChild('checkAllStaff', {static: false}) checkAll: ElementRef;

    tableHeaders = [];

    page = 1;
    pageSize = 10;

    statusVals = Status;
    statusValues;

    ngOnInit(): void {
        this.statusValues = Object.keys(this.statusVals).filter((k) =>
            isNaN(Number(k))
        );

        this.tableHeaders = [
            {name: 'Staff no', showColumn: true},
            {name: 'Full name', showColumn: true},
            {name: 'Staff category', showColumn: this.showOthers},
            {name: 'Status', showColumn: this.showOthers}
        ];
    }

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    checkAllClicked = (inputSelectAll: any) => {
        if (inputSelectAll?.target?.checked) {
            this.staff.forEach((c) => {
                c.isSelected = true;
            });
        } else {
            this.staff.forEach((c) => {
                c.isSelected = false;
            });
        }
    };

    staffClicked = (staffId: number) => {
        this.staffClickedEvent.emit(staffId);
    };

    itemClicked = (inputCheckItem: any) => {
        if (this.staff.some((s) => !s.isSelected)) {
            this.checkAll.nativeElement.checked = false;
        } else {
            this.checkAll.nativeElement.checked = true;
        }
    };
}
