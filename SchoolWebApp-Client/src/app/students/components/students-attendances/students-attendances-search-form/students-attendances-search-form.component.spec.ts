import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentsAttendancesSearchFormComponent } from './students-attendances-search-form.component';

describe('StudentsAttendancesSearchFormComponent', () => {
  let component: StudentsAttendancesSearchFormComponent;
  let fixture: ComponentFixture<StudentsAttendancesSearchFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentsAttendancesSearchFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentsAttendancesSearchFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
