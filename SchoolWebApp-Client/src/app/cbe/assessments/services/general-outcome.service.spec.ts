import { TestBed } from '@angular/core/testing';

import { GeneralOutcomeService } from './general-outcome.service';

describe('GeneralOutcomeService', () => {
  let service: GeneralOutcomeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GeneralOutcomeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
