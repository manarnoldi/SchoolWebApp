import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentsSubjectsAddFormComponent } from './students-subjects-add-form.component';

describe('StudentsSubjectsAddFormComponent', () => {
  let component: StudentsSubjectsAddFormComponent;
  let fixture: ComponentFixture<StudentsSubjectsAddFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentsSubjectsAddFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentsSubjectsAddFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
