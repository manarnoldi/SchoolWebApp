import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffAssignmentsComponent } from './staff-assignments.component';

describe('StaffAssignmentsComponent', () => {
  let component: StaffAssignmentsComponent;
  let fixture: ComponentFixture<StaffAssignmentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffAssignmentsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffAssignmentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
