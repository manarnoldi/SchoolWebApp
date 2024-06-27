import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EducationLevelTypesComponent } from './education-level-types.component';

describe('EducationLevelTypesComponent', () => {
  let component: EducationLevelTypesComponent;
  let fixture: ComponentFixture<EducationLevelTypesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EducationLevelTypesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EducationLevelTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
