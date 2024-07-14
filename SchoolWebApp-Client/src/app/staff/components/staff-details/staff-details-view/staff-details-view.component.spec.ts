import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffDetailsViewComponent } from './staff-details-view.component';

describe('StaffDetailsViewComponent', () => {
  let component: StaffDetailsViewComponent;
  let fixture: ComponentFixture<StaffDetailsViewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffDetailsViewComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffDetailsViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
