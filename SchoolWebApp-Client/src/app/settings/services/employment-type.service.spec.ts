import { TestBed } from '@angular/core/testing';

import { EmploymentTypeService } from './employment-type.service';

describe('EmploymentTypeService', () => {
  let service: EmploymentTypeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EmploymentTypeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
