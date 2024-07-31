import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentDisciplineTableComponent } from './student-discipline-table.component';

describe('StudentDisciplineTableComponent', () => {
  let component: StudentDisciplineTableComponent;
  let fixture: ComponentFixture<StudentDisciplineTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentDisciplineTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentDisciplineTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
