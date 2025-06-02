import { TestBed } from '@angular/core/testing';

import { StaffAttendancesReportDetailsService } from './staff-attendances-report-details.service';

describe('StaffAttendancesReportDetailsService', () => {
  let service: StaffAttendancesReportDetailsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StaffAttendancesReportDetailsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
