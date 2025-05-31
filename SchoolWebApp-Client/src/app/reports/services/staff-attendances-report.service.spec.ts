import { TestBed } from '@angular/core/testing';

import { StaffAttendancesReportService } from './staff-attendances-report.service';

describe('StaffAttendancesReportService', () => {
  let service: StaffAttendancesReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StaffAttendancesReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
