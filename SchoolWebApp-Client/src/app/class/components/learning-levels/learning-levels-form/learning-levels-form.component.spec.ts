import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LearningLevelsFormComponent } from './learning-levels-form.component';

describe('LearningLevelsFormComponent', () => {
  let component: LearningLevelsFormComponent;
  let fixture: ComponentFixture<LearningLevelsFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LearningLevelsFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LearningLevelsFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
