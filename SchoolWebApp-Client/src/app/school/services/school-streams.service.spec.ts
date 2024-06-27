import { TestBed } from '@angular/core/testing';

import { SchoolStreamsService } from './school-streams.service';

describe('SchoolStreamsService', () => {
  let service: SchoolStreamsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SchoolStreamsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
