import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentSubjectsTableComponent } from './student-subjects-table.component';

describe('StudentSubjectsTableComponent', () => {
  let component: StudentSubjectsTableComponent;
  let fixture: ComponentFixture<StudentSubjectsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentSubjectsTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentSubjectsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
