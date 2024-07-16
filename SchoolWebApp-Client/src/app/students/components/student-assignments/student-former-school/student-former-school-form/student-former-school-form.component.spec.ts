import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentFormerSchoolFormComponent } from './student-former-school-form.component';

describe('StudentFormerSchoolFormComponent', () => {
  let component: StudentFormerSchoolFormComponent;
  let fixture: ComponentFixture<StudentFormerSchoolFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentFormerSchoolFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentFormerSchoolFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
