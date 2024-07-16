import { TestBed } from '@angular/core/testing';

import { StudentFormerSchoolsService } from './student-former-schools.service';

describe('StudentFormerSchoolsService', () => {
  let service: StudentFormerSchoolsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StudentFormerSchoolsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
