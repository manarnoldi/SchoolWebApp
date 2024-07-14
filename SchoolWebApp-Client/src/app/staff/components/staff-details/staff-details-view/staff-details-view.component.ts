import {StaffDetails} from '@/staff/models/staff-details';
import {Component, Input} from '@angular/core';

@Component({
    selector: 'app-staff-details-view',
    templateUrl: './staff-details-view.component.html',
    styleUrl: './staff-details-view.component.scss'
})
export class StaffDetailsViewComponent {
    @Input() staffDetails: StaffDetails;
    @Input() statuses;
    @Input() alignment: string = 'extended';
}
