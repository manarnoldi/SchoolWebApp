import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
    selector: 'app-table-paging',
    templateUrl: './table-paging.component.html',
    styleUrls: ['./table-paging.component.scss']
})
export class TablePagingComponent  {
    constructor() {}
    page: number = 1;
    pageSize: number = 10;

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
