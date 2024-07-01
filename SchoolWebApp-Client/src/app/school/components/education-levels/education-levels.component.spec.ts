import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EducationLevelsComponent } from './education-levels.component';

describe('EducationLevelsComponent', () => {
  let component: EducationLevelsComponent;
  let fixture: ComponentFixture<EducationLevelsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EducationLevelsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EducationLevelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
