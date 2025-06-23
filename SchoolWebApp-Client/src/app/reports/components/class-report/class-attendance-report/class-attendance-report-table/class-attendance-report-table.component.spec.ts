import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClassAttendanceReportTableComponent } from './class-attendance-report-table.component';

describe('ClassAttendanceReportTableComponent', () => {
  let component: ClassAttendanceReportTableComponent;
  let fixture: ComponentFixture<ClassAttendanceReportTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ClassAttendanceReportTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClassAttendanceReportTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
