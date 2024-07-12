import { TestBed } from '@angular/core/testing';

import { SchoolClassesService } from './school-classes.service';

describe('SchoolClassesService', () => {
  let service: SchoolClassesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SchoolClassesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
