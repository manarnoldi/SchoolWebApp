import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExamResultsTableComponent } from './exam-results-table.component';

describe('ExamResultsTableComponent', () => {
  let component: ExamResultsTableComponent;
  let fixture: ComponentFixture<ExamResultsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ExamResultsTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExamResultsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
