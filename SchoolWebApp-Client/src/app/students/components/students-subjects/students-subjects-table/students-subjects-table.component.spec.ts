import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentsSubjectsTableComponent } from './students-subjects-table.component';

describe('StudentsSubjectsTableComponent', () => {
  let component: StudentsSubjectsTableComponent;
  let fixture: ComponentFixture<StudentsSubjectsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentsSubjectsTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentsSubjectsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
