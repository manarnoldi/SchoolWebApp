import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffDetailsTableComponent } from './staff-details-table.component';

describe('StaffDetailsTableComponent', () => {
  let component: StaffDetailsTableComponent;
  let fixture: ComponentFixture<StaffDetailsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffDetailsTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffDetailsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
