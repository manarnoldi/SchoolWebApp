import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentsSubjectsSearchFormComponent } from './students-subjects-search-form.component';

describe('StudentsSubjectsSearchFormComponent', () => {
  let component: StudentsSubjectsSearchFormComponent;
  let fixture: ComponentFixture<StudentsSubjectsSearchFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentsSubjectsSearchFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentsSubjectsSearchFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
