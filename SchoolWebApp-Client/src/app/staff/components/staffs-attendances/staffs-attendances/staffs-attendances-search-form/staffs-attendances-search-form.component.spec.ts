import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffsAttendancesSearchFormComponent } from './staffs-attendances-search-form.component';

describe('StaffsAttendancesSearchFormComponent', () => {
  let component: StaffsAttendancesSearchFormComponent;
  let fixture: ComponentFixture<StaffsAttendancesSearchFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffsAttendancesSearchFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffsAttendancesSearchFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
