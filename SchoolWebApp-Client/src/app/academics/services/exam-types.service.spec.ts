import { TestBed } from '@angular/core/testing';

import { ExamTypesService } from './exam-types.service';

describe('ExamTypesService', () => {
  let service: ExamTypesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ExamTypesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
