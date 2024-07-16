import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentFormerSchoolComponent } from './student-former-school.component';

describe('StudentFormerSchoolComponent', () => {
  let component: StudentFormerSchoolComponent;
  let fixture: ComponentFixture<StudentFormerSchoolComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentFormerSchoolComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentFormerSchoolComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
