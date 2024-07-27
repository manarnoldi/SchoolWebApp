import { TestBed } from '@angular/core/testing';

import { StudentParentsService } from './student-parents.service';

describe('StudentParentsService', () => {
  let service: StudentParentsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StudentParentsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
