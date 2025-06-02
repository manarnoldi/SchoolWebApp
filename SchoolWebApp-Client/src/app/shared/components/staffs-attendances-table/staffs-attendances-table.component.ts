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
    selector: 'app-staffs-attendances-table',
    templateUrl: './staffs-attendances-table.component.html',
    styleUrl: './staffs-attendances-table.component.scss'
})
export class StaffsAttendancesTableComponent implements OnInit {
    @Input() tableTitle: string = 'Staff attendance list';
    @Input() staffs: StaffDetails[] = [];
    @Input() currentDate: Date = new Date();
    @Input() disabled: Boolean = false;
    @Input() showActions: Boolean = true;

    @Output() deleteItemEvent = new EventEmitter<number>();

    @ViewChild('checkAllStaffs', {static: false}) checkAll: ElementRef;

    tableHeaders: string[] = ['Staff No', 'Staff Full Name', 'Date'];

    page = 1;
    pageSize = 20;

    constructor() {}

    updateCheckAll = () => {
        if (this.checkAll) {
            if (this.staffs.length <= 0) {
                this.checkAll.nativeElement.checked = false;
            } else if (this.staffs.some((s) => !s.isSelected)) {
                this.checkAll.nativeElement.checked = false;
            } else {
                this.checkAll.nativeElement.checked = true;
            }
        }
    };

    ngOnInit(): void {
        this.updateCheckAll();
    }

    checkAllClicked = (inputSelectAll: any) => {
        if (inputSelectAll?.target?.checked) {
            this.staffs.forEach((c) => {
                c.isSelected = true;
            });
        } else {
            this.staffs.forEach((c) => {
                c.isSelected = false;
            });
        }
    };

    itemClicked = (inputCheckItem: any) => {
        if (this.staffs.some((s) => !s.isSelected)) {
            this.checkAll.nativeElement.checked = false;
        } else {
            this.checkAll.nativeElement.checked = true;
        }
    };
    
    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    deleteItem = (id: number) => {
        this.deleteItemEvent.emit(id);
    };
}
