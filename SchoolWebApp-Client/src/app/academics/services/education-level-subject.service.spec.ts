import { TestBed } from '@angular/core/testing';

import { EducationLevelSubjectService } from './education-level-subject.service';

describe('EducationLevelSubjectService', () => {
  let service: EducationLevelSubjectService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(EducationLevelSubjectService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
