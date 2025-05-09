import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentsAttendancesTableComponent } from './students-attendances-table.component';

describe('StudentsAttendancesTableComponent', () => {
  let component: StudentsAttendancesTableComponent;
  let fixture: ComponentFixture<StudentsAttendancesTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentsAttendancesTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentsAttendancesTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
