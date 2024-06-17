import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LearningModesComponent } from './learning-modes.component';

describe('LearningModesComponent', () => {
  let component: LearningModesComponent;
  let fixture: ComponentFixture<LearningModesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LearningModesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LearningModesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
