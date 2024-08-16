import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffSubjectFormComponent } from './staff-subject-form.component';

describe('StaffSubjectFormComponent', () => {
  let component: StaffSubjectFormComponent;
  let fixture: ComponentFixture<StaffSubjectFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StaffSubjectFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffSubjectFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
