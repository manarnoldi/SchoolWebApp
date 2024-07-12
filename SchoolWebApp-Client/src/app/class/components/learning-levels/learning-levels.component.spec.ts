import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LearningLevelsComponent } from './learning-levels.component';

describe('LearningLevelsComponent', () => {
  let component: LearningLevelsComponent;
  let fixture: ComponentFixture<LearningLevelsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LearningLevelsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LearningLevelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
