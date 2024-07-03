import {TableSettingsService} from '@/shared/services/table-settings.service';
import {StaffDetails} from '@/staff/models/staff-details';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {Subscription} from 'rxjs';

@Component({
    selector: 'app-staff-details-table',
    templateUrl: './staff-details-table.component.html',
    styleUrl: './staff-details-table.component.scss'
})
export class StaffDetailsTableComponent implements OnInit {
    @Input() tableTitle: string = 'School staff list';
    @Input() staffs: StaffDetails[] = [];
    @Input() searchedStaffs: StaffDetails[] = [];
    @Input() showActionControls: Boolean = true;
    @Input() showLoginControls: Boolean = false;

    @Output() deleteItemEvent = new EventEmitter<number>();

    tableHeaders: string[] = [
        'No',
        'Full name',
        'Staff category',
        'Designation',
        'Employment type',
        'Email',
        'Phone number',
        'Employment date',
        'Nationality',
        'Action'
    ];

    page = 1;
    pageSize = 10;
    collectionSize = 0;
    pageSubscription: Subscription;
    pageSizeSubscription: Subscription;

    constructor(private tableSettingsSvc: TableSettingsService) {}

    ngOnInit(): void {
        this.pageSubscription = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );
        this.pageSizeSubscription = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );
    }

    searchStaff = (searchVal: string) => {
        if (this.staffs.length > 1 && searchVal.length > 1) {
            this.searchedStaffs = this.staffs.filter(
                (s) =>
                    s.fullName?.toLowerCase()
                        .includes(searchVal.toLowerCase()) ||
                    s.upi?.toLowerCase().includes(searchVal.toLowerCase()) ||
                    s.idNumber?.toLowerCase().includes(searchVal.toLowerCase())
            );
        } else {
            this.searchedStaffs = this.staffs;
        }
    };

    refreshItems = () => {
        this.collectionSize = this.staffs.length;
        this.searchedStaffs = this.staffs;
    };

    deleteStaff = (id: number) => {
        this.deleteItemEvent.emit(id);
    };
}
