import { TestBed } from '@angular/core/testing';

import { SubjectGroupsService } from './subject-groups.service';

describe('SubjectGroupsService', () => {
  let service: SubjectGroupsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SubjectGroupsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
