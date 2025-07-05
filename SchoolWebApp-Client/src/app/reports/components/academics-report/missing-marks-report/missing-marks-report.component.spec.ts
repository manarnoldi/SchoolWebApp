import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MissingMarksReportComponent } from './missing-marks-report.component';

describe('MissingMarksReportComponent', () => {
  let component: MissingMarksReportComponent;
  let fixture: ComponentFixture<MissingMarksReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MissingMarksReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MissingMarksReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
