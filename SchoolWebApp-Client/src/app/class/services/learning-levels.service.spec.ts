import { TestBed } from '@angular/core/testing';

import { LearningLevelsService } from './learning-levels.service';

describe('LearningLevelsService', () => {
  let service: LearningLevelsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LearningLevelsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
