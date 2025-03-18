import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EducationLevelSubjectsComponent } from './education-level-subjects.component';

describe('EducationLevelSubjectsComponent', () => {
  let component: EducationLevelSubjectsComponent;
  let fixture: ComponentFixture<EducationLevelSubjectsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EducationLevelSubjectsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EducationLevelSubjectsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
