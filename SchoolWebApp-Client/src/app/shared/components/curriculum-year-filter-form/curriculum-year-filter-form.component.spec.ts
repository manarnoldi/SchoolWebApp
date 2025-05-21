import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CurriculumYearFilterFormComponent } from './curriculum-year-filter-form.component';

describe('CurriculumYearFilterFormComponent', () => {
  let component: CurriculumYearFilterFormComponent;
  let fixture: ComponentFixture<CurriculumYearFilterFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CurriculumYearFilterFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CurriculumYearFilterFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
