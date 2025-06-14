import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffMinTableComponent } from './staff-min-table.component';

describe('StaffMinTableComponent', () => {
  let component: StaffMinTableComponent;
  let fixture: ComponentFixture<StaffMinTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffMinTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffMinTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
