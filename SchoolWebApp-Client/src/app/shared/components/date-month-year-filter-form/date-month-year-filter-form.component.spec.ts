import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DateMonthYearFilterFormComponent } from './date-month-year-filter-form.component';

describe('DateMonthYearFilterFormComponent', () => {
  let component: DateMonthYearFilterFormComponent;
  let fixture: ComponentFixture<DateMonthYearFilterFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DateMonthYearFilterFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DateMonthYearFilterFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
