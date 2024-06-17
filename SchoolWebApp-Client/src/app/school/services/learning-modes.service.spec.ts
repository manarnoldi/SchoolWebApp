import { TestBed } from '@angular/core/testing';

import { LearningModesService } from './learning-modes.service';

describe('LearningModesService', () => {
  let service: LearningModesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LearningModesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
