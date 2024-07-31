import { TestBed } from '@angular/core/testing';

import { StudentDisciplinesService } from './student-disciplines.service';

describe('StudentDisciplinesService', () => {
  let service: StudentDisciplinesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StudentDisciplinesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
