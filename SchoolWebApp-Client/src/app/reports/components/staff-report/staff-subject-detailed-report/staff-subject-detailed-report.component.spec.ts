import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffSubjectDetailedReportComponent } from './staff-subject-detailed-report.component';

describe('StaffSubjectDetailedReportComponent', () => {
  let component: StaffSubjectDetailedReportComponent;
  let fixture: ComponentFixture<StaffSubjectDetailedReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffSubjectDetailedReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffSubjectDetailedReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
