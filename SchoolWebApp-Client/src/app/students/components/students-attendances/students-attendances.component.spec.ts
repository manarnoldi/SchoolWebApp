import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentsAttendancesComponent } from './students-attendances.component';

describe('StudentsAttendancesComponent', () => {
  let component: StudentsAttendancesComponent;
  let fixture: ComponentFixture<StudentsAttendancesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentsAttendancesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentsAttendancesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
