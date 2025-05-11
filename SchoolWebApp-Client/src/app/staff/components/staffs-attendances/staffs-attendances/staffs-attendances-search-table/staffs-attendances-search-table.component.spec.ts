import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffsAttendancesSearchTableComponent } from './staffs-attendances-search-table.component';

describe('StaffsAttendancesSearchTableComponent', () => {
  let component: StaffsAttendancesSearchTableComponent;
  let fixture: ComponentFixture<StaffsAttendancesSearchTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffsAttendancesSearchTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffsAttendancesSearchTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
