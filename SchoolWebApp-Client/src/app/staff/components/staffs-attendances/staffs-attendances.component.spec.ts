import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffsAttendancesComponent } from './staffs-attendances.component';

describe('StaffsAttendancesComponent', () => {
  let component: StaffsAttendancesComponent;
  let fixture: ComponentFixture<StaffsAttendancesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffsAttendancesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffsAttendancesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
