import { TestBed } from '@angular/core/testing';

import { StaffDetailsService } from './staff-details.service';

describe('StaffDetailsService', () => {
  let service: StaffDetailsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StaffDetailsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
