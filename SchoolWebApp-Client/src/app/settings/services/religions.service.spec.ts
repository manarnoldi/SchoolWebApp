import { TestBed } from '@angular/core/testing';

import { ReligionsService } from './religions.service';

describe('ReligionsService', () => {
  let service: ReligionsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ReligionsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
