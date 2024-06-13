import { TestBed } from '@angular/core/testing';

import { OccurenceTypeService } from './occurence-type.service';

describe('OccurenceTypeService', () => {
  let service: OccurenceTypeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(OccurenceTypeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
