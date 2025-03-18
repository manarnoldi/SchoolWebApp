import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AcademicYearsSelectorFormComponent } from './academic-years-selector-form.component';

describe('AcademicYearsSelectorFormComponent', () => {
  let component: AcademicYearsSelectorFormComponent;
  let fixture: ComponentFixture<AcademicYearsSelectorFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AcademicYearsSelectorFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AcademicYearsSelectorFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
