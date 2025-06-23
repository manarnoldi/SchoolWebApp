import { TestBed } from '@angular/core/testing';

import { StudentsAttendanceReportService } from './students-attendance-report.service';

describe('StudentsAttendanceReportService', () => {
  let service: StudentsAttendanceReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StudentsAttendanceReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
