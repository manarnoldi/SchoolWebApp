import { TestBed } from '@angular/core/testing';

import { MissingMarksReportService } from './missing-marks-report.service';

describe('MissingMarksReportService', () => {
  let service: MissingMarksReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MissingMarksReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
