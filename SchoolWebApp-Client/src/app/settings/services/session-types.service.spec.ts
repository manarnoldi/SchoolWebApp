import { TestBed } from '@angular/core/testing';

import { SessionTypesService } from './session-types.service';

describe('SessionTypesService', () => {
  let service: SessionTypesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SessionTypesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
