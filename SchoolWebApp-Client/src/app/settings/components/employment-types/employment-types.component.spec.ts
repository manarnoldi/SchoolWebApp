import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmploymentTypesComponent } from './employment-types.component';

describe('EmploymentTypesComponent', () => {
  let component: EmploymentTypesComponent;
  let fixture: ComponentFixture<EmploymentTypesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmploymentTypesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmploymentTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
