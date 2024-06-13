import { TestBed } from '@angular/core/testing';

import { StaffCategoriesService } from './staff-categories.service';

describe('StaffCategoriesService', () => {
  let service: StaffCategoriesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StaffCategoriesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
