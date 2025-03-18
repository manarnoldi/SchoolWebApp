import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EducationLevelSubjectsTableComponent } from './education-level-subjects-table.component';

describe('EducationLevelSubjectsTableComponent', () => {
  let component: EducationLevelSubjectsTableComponent;
  let fixture: ComponentFixture<EducationLevelSubjectsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EducationLevelSubjectsTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EducationLevelSubjectsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
