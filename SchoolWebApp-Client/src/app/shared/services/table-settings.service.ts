import {Injectable} from '@angular/core';
import {BehaviorSubject} from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class TableSettingsService {
    private pageBS = new BehaviorSubject(1);
    private pageSizeBS = new BehaviorSubject(10);
    page = this.pageBS.asObservable();
    pageSize = this.pageSizeBS.asObservable();

    constructor() {}

    changePage(page: number) {
        this.pageBS.next(page);
    }

    changePageSize(pageSize: number) {
        this.pageSizeBS.next(pageSize);
    }
}