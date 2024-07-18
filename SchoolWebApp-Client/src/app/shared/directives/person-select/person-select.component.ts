import {Component, Input} from '@angular/core';

@Component({
    selector: 'app-person-select',
    templateUrl: './person-select.component.html',
    styleUrl: './person-select.component.scss'
})
export class PersonSelectComponent {
    @Input() personId: string;
    @Input() selectFormLabel: string;
    @Input() persons: any[];
    @Input() bindLabel: string;
    @Input() requiredTitle: string;
    @Input() placeholderText: string;
}
