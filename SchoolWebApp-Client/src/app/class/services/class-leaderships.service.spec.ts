import { TestBed } from '@angular/core/testing';

import { ClassLeadershipsService } from './class-leaderships.service';

describe('ClassLeadershipsService', () => {
  let service: ClassLeadershipsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ClassLeadershipsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
