import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentDisciplineFormComponent } from './student-discipline-form.component';

describe('StudentDisciplineFormComponent', () => {
  let component: StudentDisciplineFormComponent;
  let fixture: ComponentFixture<StudentDisciplineFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentDisciplineFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentDisciplineFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
