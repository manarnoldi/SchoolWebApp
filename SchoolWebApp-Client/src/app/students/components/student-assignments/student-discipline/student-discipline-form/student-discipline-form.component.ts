import { DateLessThanOrEqualsValidator } from '@/core/validators.ts/DateValidators';
import { OccurenceType } from '@/settings/models/occurence-type';
import { Outcome } from '@/settings/models/outcome';
import { StudentDetails } from '@/students/models/student-details';
import { StudentDiscipline } from '@/students/models/student-discipline';
import { formatDate } from '@angular/common';
import { Component, ElementRef, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-student-discipline-form',
  templateUrl: './student-discipline-form.component.html',
  styleUrl: './student-discipline-form.component.scss'
})
export class StudentDisciplineFormComponent implements OnInit {
  @ViewChild('closeButton') closeButton: ElementRef;
  @Input() studentDiscipline: StudentDiscipline;
  @Input() statuses;
  @Input() student: StudentDetails;

  @Input() outcomes: Outcome[] = [];
  @Input() occurenceTypes: OccurenceType[] = [];
  action: string = 'add';

  @Output() addItemEvent = new EventEmitter<StudentDiscipline>();
  @Output() errorEvent = new EventEmitter<string>();

  studentDisciplineForm: FormGroup;

  constructor(private formBuilder: FormBuilder) {}

  ngOnInit(): void {
      this.refreshItems();
  }

  refreshItems = () => {
      this.studentDisciplineForm = this.formBuilder.group({
          studentId: [this.student?.id, [Validators.required]],
          occurenceStartDate: [
              formatDate(new Date(), 'yyyy-MM-dd', 'en'),
              [
                  Validators.required,
                  DateLessThanOrEqualsValidator('occurenceEndDate', 'greater')
              ]
          ],
          occurenceEndDate: [
              formatDate(new Date(), 'yyyy-MM-dd', 'en'),
              [Validators.required]
          ],
          occurenceDetails: [''],
          outcomeDetails: [''],
          outcomeId: [null],
          occurenceTypeId: [null]
      });
  };

  setFormControls = (studentDiscipline: StudentDiscipline) => {
      this.studentDisciplineForm.setValue({
          occurenceStartDate: formatDate(
              new Date(studentDiscipline.occurenceStartDate),
              'yyyy-MM-dd',
              'en'
          ),
          occurenceEndDate: formatDate(
              new Date(studentDiscipline.occurenceEndDate),
              'yyyy-MM-dd',
              'en'
          ),
          occurenceDetails: studentDiscipline.occurenceDetails,
          outcomeDetails: studentDiscipline.outcomeDetails,
          studentId: studentDiscipline.studentId ?? null,
          outcomeId: studentDiscipline.outcomeId ?? null,
          occurenceTypeId: studentDiscipline.occurenceTypeId ?? null
      });
  };

  get f() {
      return this.studentDisciplineForm.controls;
  }

  closeStudentDisciplineForm = () => {
      this.closeButton.nativeElement.click();
      this.resetFormControls();
      this.refreshItems();        
  };

  resetFormControls() {
      this.action = 'add';
      this.studentDisciplineForm.reset();
  }

  onSubmit = () => {
      if (this.action == 'edit') {
          let studentDisciplineId = this.studentDiscipline.id;
          this.studentDiscipline = new StudentDiscipline(
              this.studentDisciplineForm.value
          );
          this.studentDiscipline.id = studentDisciplineId;
      } else {
          this.studentDiscipline = new StudentDiscipline(
              this.studentDisciplineForm.value
          );
      }
      this.addItemEvent.emit(this.studentDiscipline);
  };

}
