import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EducationLevelFormComponent } from './education-level-form.component';

describe('EducationLevelFormComponent', () => {
  let component: EducationLevelFormComponent;
  let fixture: ComponentFixture<EducationLevelFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EducationLevelFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EducationLevelFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
