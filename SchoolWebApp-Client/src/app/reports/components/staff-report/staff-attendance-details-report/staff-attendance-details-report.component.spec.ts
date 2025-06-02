import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffAttendanceDetailsReportComponent } from './staff-attendance-details-report.component';

describe('StaffAttendanceDetailsReportComponent', () => {
  let component: StaffAttendanceDetailsReportComponent;
  let fixture: ComponentFixture<StaffAttendanceDetailsReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffAttendanceDetailsReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffAttendanceDetailsReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
