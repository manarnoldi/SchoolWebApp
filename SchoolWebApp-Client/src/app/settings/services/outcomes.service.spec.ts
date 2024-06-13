import { TestBed } from '@angular/core/testing';

import { OutcomesService } from './outcomes.service';

describe('OutcomesService', () => {
  let service: OutcomesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OutcomesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
