import { StudentDetails } from '@/students/models/student-details';
import {Component, Input} from '@angular/core';

@Component({
    selector: 'app-student-details-view',
    templateUrl: './student-details-view.component.html',
    styleUrl: './student-details-view.component.scss'
})
export class StudentDetailsViewComponent {
    @Input() studentDetails: StudentDetails;
    @Input() statuses;
    @Input() alignment: string = 'extended';
}
