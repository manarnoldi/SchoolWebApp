import { TestBed } from '@angular/core/testing';

import { ExamNamesService } from './exam-names.service';

describe('ExamNamesService', () => {
  let service: ExamNamesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ExamNamesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
