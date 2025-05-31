import {TableSettingsService} from '@/shared/services/table-settings.service';
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
import {Subscription} from 'rxjs';

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
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    constructor(private tableSettingsSvc: TableSettingsService) {}

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
        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );
        this.tableSettingsSvc.changePageSize(20);
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

    deleteItem = (id: number) => {
        this.deleteItemEvent.emit(id);
    };
}
