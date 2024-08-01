import { SchoolClass } from '@/class/models/school-class';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-class-details-view',
  templateUrl: './class-details-view.component.html',
  styleUrl: './class-details-view.component.scss'
})
export class ClassDetailsViewComponent {
  @Input() schoolClass: SchoolClass;
  @Input() alignment: string = 'extended';
}
