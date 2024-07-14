import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffAttendanceFormComponent } from './staff-attendance-form.component';

describe('StaffAttendanceFormComponent', () => {
  let component: StaffAttendanceFormComponent;
  let fixture: ComponentFixture<StaffAttendanceFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffAttendanceFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffAttendanceFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
