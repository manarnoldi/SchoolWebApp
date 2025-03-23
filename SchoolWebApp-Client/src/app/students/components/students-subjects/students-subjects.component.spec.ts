import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentsSubjectsComponent } from './students-subjects.component';

describe('StudentsSubjectsComponent', () => {
  let component: StudentsSubjectsComponent;
  let fixture: ComponentFixture<StudentsSubjectsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentsSubjectsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentsSubjectsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
