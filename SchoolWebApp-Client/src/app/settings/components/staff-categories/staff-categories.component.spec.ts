import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffCategoriesComponent } from './staff-categories.component';

describe('StaffCategoriesComponent', () => {
  let component: StaffCategoriesComponent;
  let fixture: ComponentFixture<StaffCategoriesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffCategoriesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffCategoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
