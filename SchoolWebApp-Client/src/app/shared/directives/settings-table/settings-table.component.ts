import {
    Component,
    Input,
    OnInit,
    Output,
    EventEmitter,
    ViewChild,
    ElementRef
} from '@angular/core';
import {TableButtonComponent} from '../table-button/table-button.component';
import {Setting} from '@/core/models/setting';

@Component({
    selector: 'app-settings-table',
    templateUrl: './settings-table.component.html',
    styleUrls: ['./settings-table.component.scss']
})
export class SettingsTableComponent {
    constructor() {}
    @ViewChild(TableButtonComponent) tableButton: TableButtonComponent;
    @Input() buttonTitle: string = '';
    @Input() tblName: string = '';
    @Input() tblModel: string = '';
    @Input() tblHeaders: string[] = [];
    @Input() tblItems: Setting[] = [];
    @Input() tblShowView: boolean = false;

    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();
    @Output() pageChangedEvent = new EventEmitter<number>();
    @Output() pageSizeChangedEvent = new EventEmitter<number>();

    page: number = 1;
    pageSize: number = 10;

    editItem(id: number) {
        this.editItemEvent.emit(id);
    }

    deleteItem(id: number) {
        this.deleteItemEvent.emit(id);
    }

    onButtonClick() {
        this.tableButton.onClick();
    }

    pageChanged(page: number) {
        this.pageChangedEvent.emit(page);
    }

    pageSizeChanged = (pageSize: number) => {
        this.pageSizeChangedEvent.emit(pageSize);
    };
}
