import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
    selector: 'app-table-paging',
    templateUrl: './table-paging.component.html',
    styleUrls: ['./table-paging.component.scss']
})
export class TablePagingComponent  {
    constructor() {}
    // Lets the list send the pager back to page 1 when its filters change.
    @Input() page: number = 1;
    @Input() pageSize: number = 10;
    @Input() collectionSize = 0;
    @Output() pageChangedEvent = new EventEmitter<number>();
    @Output() pageSizeChangedEvent = new EventEmitter<number>();

    pageChanged() {
        this.pageChangedEvent.emit(this.page);
    }

    changePageSize = () => {
        this.pageSizeChangedEvent.emit(this.pageSize);
    };
}
