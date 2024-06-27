import { TestBed } from '@angular/core/testing';

import { EducationLevelTypesService } from './education-level-types.service';

describe('EducationLevelTypesService', () => {
  let service: EducationLevelTypesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EducationLevelTypesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
