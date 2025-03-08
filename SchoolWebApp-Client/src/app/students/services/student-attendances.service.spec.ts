import { TestBed } from '@angular/core/testing';

import { StudentAttendancesService } from './student-attendances.service';

describe('StudentAttendancesService', () => {
  let service: StudentAttendancesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StudentAttendancesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
