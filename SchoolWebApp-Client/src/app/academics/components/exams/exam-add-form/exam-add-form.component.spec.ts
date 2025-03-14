import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExamAddFormComponent } from './exam-add-form.component';

describe('ExamAddFormComponent', () => {
  let component: ExamAddFormComponent;
  let fixture: ComponentFixture<ExamAddFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ExamAddFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExamAddFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
