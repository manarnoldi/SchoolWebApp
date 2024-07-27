import { Component, ElementRef, ViewChild } from '@angular/core';

@Component({
  selector: 'app-student-parents-new-form',
  templateUrl: './student-parents-new-form.component.html',
  styleUrl: './student-parents-new-form.component.scss'
})
export class StudentParentsNewFormComponent {
  @ViewChild('closeButton') closeButton: ElementRef;
  action: string = 'add';
}
