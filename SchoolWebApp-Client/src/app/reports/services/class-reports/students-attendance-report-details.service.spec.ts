import { TestBed } from '@angular/core/testing';

import { StudentsAttendanceReportDetailsService } from './students-attendance-report-details.service';

describe('StudentsAttendanceReportDetailsService', () => {
  let service: StudentsAttendanceReportDetailsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StudentsAttendanceReportDetailsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
