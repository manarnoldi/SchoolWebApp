import { TestBed } from '@angular/core/testing';

import { ClassListReportService } from './class-list-report.service';

describe('ClassListReportService', () => {
  let service: ClassListReportService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ClassListReportService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
