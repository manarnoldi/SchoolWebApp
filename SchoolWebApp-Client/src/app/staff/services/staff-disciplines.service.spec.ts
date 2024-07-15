import { TestBed } from '@angular/core/testing';

import { StaffDisciplinesService } from './staff-disciplines.service';

describe('StaffDisciplinesService', () => {
  let service: StaffDisciplinesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StaffDisciplinesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
