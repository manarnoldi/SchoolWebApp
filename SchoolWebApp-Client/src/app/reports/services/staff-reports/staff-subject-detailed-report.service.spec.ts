import { TestBed } from '@angular/core/testing';

import { StaffSubjectDetailedReportService } from './staff-subject-detailed-report.service';

describe('StaffSubjectDetailedReportService', () => {
  let service: StaffSubjectDetailedReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StaffSubjectDetailedReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
