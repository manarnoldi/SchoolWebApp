import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentDisciplineComponent } from './student-discipline.component';

describe('StudentDisciplineComponent', () => {
  let component: StudentDisciplineComponent;
  let fixture: ComponentFixture<StudentDisciplineComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentDisciplineComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentDisciplineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
