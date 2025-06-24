import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClassAttendanceDetailsReportComponent } from './class-attendance-details-report.component';

describe('ClassAttendanceDetailsReportComponent', () => {
  let component: ClassAttendanceDetailsReportComponent;
  let fixture: ComponentFixture<ClassAttendanceDetailsReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ClassAttendanceDetailsReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClassAttendanceDetailsReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
