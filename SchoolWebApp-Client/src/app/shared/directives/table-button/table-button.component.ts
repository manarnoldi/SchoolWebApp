import {
    Component,
    ElementRef,
    EventEmitter,
    Input,
    OnInit,
    Output,
    ViewChild
} from '@angular/core';

@Component({
    selector: 'app-table-button',
    templateUrl: './table-button.component.html',
    styleUrls: ['./table-button.component.scss']
})
export class TableButtonComponent implements OnInit {
    @Output() buttonClickEvent = new EventEmitter<void>();
    @ViewChild('modalOpenButton') buttonToBeClicked: ElementRef;
    @Input() modelName: string = '';
    @Input() buttonTitle: string = '';
    @Input() btnClasses: string =
        'btn btn-sm btn-flat btn-success float-right my-1';
    @Input() btnIcon = 'fa-plus-circle';

    constructor() {}

    ngOnInit(): void {}

    onClick() {
        this.buttonToBeClicked.nativeElement.click();
    }

    clickEvent = () => {
        this.buttonClickEvent.emit();
    };
}
