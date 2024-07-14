import { TestBed } from '@angular/core/testing';

import { StaffAttendancesService } from './staff-attendances.service';

describe('StaffAttendancesService', () => {
  let service: StaffAttendancesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StaffAttendancesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
