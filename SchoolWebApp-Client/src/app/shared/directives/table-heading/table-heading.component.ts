import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

@Component({
    selector: 'app-table-heading',
    templateUrl: './table-heading.component.html',
    styleUrls: ['./table-heading.component.scss']
})
export class TableHeadingComponent implements OnInit {
    @Input() tableTitle: string = '';
    @Input() editControls: boolean = true;

    @Output() searchItemEvent = new EventEmitter<string>();

    searchItem(searchText: string) {
        this.searchItemEvent.emit(searchText);
    }
    constructor() {}

    ngOnInit(): void {}
}
