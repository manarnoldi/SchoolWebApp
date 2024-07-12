import { TestBed } from '@angular/core/testing';

import { ClassLeadershipRolesService } from './class-leadership-roles.service';

describe('ClassLeadershipRolesService', () => {
  let service: ClassLeadershipRolesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ClassLeadershipRolesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
