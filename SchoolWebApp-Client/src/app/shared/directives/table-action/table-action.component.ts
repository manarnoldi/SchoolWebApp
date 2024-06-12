import {Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild} from '@angular/core';

@Component({
    selector: 'app-table-action',
    templateUrl: './table-action.component.html',
    styleUrls: ['./table-action.component.scss']
})
export class TableActionComponent implements OnInit {
    @Input() tblShowViewButton = false;
    @Input() tblShowEditButton = true;
    @Input() tblShowDeleteButton = true;
    @Input() tblClass = "";
    
    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();
    @Output() viewItemEvent = new EventEmitter<number>();

    @ViewChild('modalOpenButton') buttonToBeClicked: ElementRef;

    editItem(id: number) {
        this.editItemEvent.emit(id);
    }

    deleteItem(id: number) {
        this.deleteItemEvent.emit(id);
    }

    viewItem(id: number) {
        this.viewItemEvent.emit(id);
    }

    clickButton() {
        this.buttonToBeClicked.nativeElement.click();
    }
    
    constructor() {}

    ngOnInit(): void {}
}
