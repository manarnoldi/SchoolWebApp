import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffSubjectTableComponent } from './staff-subject-table.component';

describe('StaffSubjectTableComponent', () => {
  let component: StaffSubjectTableComponent;
  let fixture: ComponentFixture<StaffSubjectTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StaffSubjectTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffSubjectTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
