
import { TableSettingsService } from '@/shared/services/table-settings.service';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
    selector: 'app-table-paging',
    templateUrl: './table-paging.component.html',
    styleUrls: ['./table-paging.component.scss']
})
export class TablePagingComponent implements OnInit {
    constructor(private tableSettingsSvc: TableSettingsService) {}
    page:number;
    pageSize:number;
    subscriptionPage: Subscription;
    subscriptionPageSize: Subscription;
    
    ngOnInit(): void {
        this.subscriptionPage = this.tableSettingsSvc.page.subscribe(
            (page) => (this.page = page)
        );

        this.subscriptionPageSize = this.tableSettingsSvc.pageSize.subscribe(
            (pageSize) => (this.pageSize = pageSize)
        );
    }
    @Input() collectionSize = 0;
    // eslint-disable-next-line @angular-eslint/no-output-on-prefix
    @Output() onInitEvent = new EventEmitter<void>();

    pageChanged(): void {
        this.tableSettingsSvc.changePage(this.page);
        this.tableSettingsSvc.changePageSize(this.pageSize);
        this.onInitEvent.emit();
    }
}
