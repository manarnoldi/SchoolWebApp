import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AcademicsReportComponent } from './academics-report.component';

describe('AcademicsReportComponent', () => {
  let component: AcademicsReportComponent;
  let fixture: ComponentFixture<AcademicsReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AcademicsReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AcademicsReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
