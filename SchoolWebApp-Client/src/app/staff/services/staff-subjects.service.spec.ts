import { TestBed } from '@angular/core/testing';

import { StaffSubjectsService } from './staff-subjects.service';

describe('StaffSubjectsService', () => {
  let service: StaffSubjectsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StaffSubjectsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
