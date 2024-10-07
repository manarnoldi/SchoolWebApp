import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentSubjectsLoadFormComponent } from './student-subjects-load-form.component';

describe('StudentSubjectsLoadFormComponent', () => {
  let component: StudentSubjectsLoadFormComponent;
  let fixture: ComponentFixture<StudentSubjectsLoadFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentSubjectsLoadFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentSubjectsLoadFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
