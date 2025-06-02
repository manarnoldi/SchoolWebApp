import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffAttendanceReportTableComponent } from './staff-attendance-report-table.component';

describe('StaffAttendanceReportTableComponent', () => {
  let component: StaffAttendanceReportTableComponent;
  let fixture: ComponentFixture<StaffAttendanceReportTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffAttendanceReportTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffAttendanceReportTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
