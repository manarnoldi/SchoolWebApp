import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClassListsReportComponent } from './class-lists-report.component';

describe('ClassListsReportComponent', () => {
  let component: ClassListsReportComponent;
  let fixture: ComponentFixture<ClassListsReportComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ClassListsReportComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClassListsReportComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
