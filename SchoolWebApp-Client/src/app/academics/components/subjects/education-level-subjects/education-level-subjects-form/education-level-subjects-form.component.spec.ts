import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EducationLevelSubjectsFormComponent } from './education-level-subjects-form.component';

describe('EducationLevelSubjectsFormComponent', () => {
  let component: EducationLevelSubjectsFormComponent;
  let fixture: ComponentFixture<EducationLevelSubjectsFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EducationLevelSubjectsFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EducationLevelSubjectsFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
