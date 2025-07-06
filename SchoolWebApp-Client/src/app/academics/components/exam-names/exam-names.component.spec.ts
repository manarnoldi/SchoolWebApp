import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExamNamesComponent } from './exam-names.component';

describe('ExamNamesComponent', () => {
  let component: ExamNamesComponent;
  let fixture: ComponentFixture<ExamNamesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ExamNamesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ExamNamesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
