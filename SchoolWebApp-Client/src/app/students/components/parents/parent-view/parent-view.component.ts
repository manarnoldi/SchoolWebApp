import {Parent} from '@/students/models/parent';
import {Component, Input} from '@angular/core';

@Component({
    selector: 'app-parent-view',
    templateUrl: './parent-view.component.html',
    styleUrl: './parent-view.component.scss'
})
export class ParentViewComponent {
    @Input() parent: Parent;
    @Input() statuses;
    @Input() alignment: string = 'reduced';
}
