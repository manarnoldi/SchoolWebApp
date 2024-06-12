
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
import { Setting } from '@/core/models/setting';

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

    @Input() collectionSize = 0;

    @Output() editItemEvent = new EventEmitter<number>();
    @Output() deleteItemEvent = new EventEmitter<number>();
    // eslint-disable-next-line @angular-eslint/no-output-on-prefix
    @Output() onInitEvent = new EventEmitter<void>();

    editItem(id: number) {
        this.editItemEvent.emit(id);
    }

    deleteItem(id: number) {
        this.deleteItemEvent.emit(id);
    }

    onInit() {
        this.onInitEvent.emit();
    }

    onButtonClick() {
        this.tableButton.onClick();
    }

    pageChanged(): void {
        this.onInitEvent.emit();
    }
}
