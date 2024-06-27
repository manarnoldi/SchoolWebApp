import { TestBed } from '@angular/core/testing';

import { SchoolDetailsService } from './school-details.service';

describe('SchoolDetailsService', () => {
  let service: SchoolDetailsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SchoolDetailsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
