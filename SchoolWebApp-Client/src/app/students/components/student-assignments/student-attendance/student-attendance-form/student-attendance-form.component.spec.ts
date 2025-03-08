import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentAttendanceFormComponent } from './student-attendance-form.component';

describe('StudentAttendanceFormComponent', () => {
  let component: StudentAttendanceFormComponent;
  let fixture: ComponentFixture<StudentAttendanceFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentAttendanceFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentAttendanceFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
