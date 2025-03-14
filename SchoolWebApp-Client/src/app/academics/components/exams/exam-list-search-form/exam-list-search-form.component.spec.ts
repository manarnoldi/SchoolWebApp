import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExamListSearchFormComponent } from './exam-list-search-form.component';

describe('ExamListSearchFormComponent', () => {
  let component: ExamListSearchFormComponent;
  let fixture: ComponentFixture<ExamListSearchFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ExamListSearchFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExamListSearchFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
