import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffsAttendancesTableComponent } from './staffs-attendances-table.component';

describe('StaffsAttendancesTableComponent', () => {
  let component: StaffsAttendancesTableComponent;
  let fixture: ComponentFixture<StaffsAttendancesTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffsAttendancesTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffsAttendancesTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
