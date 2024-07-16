import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentFormerSchoolTableComponent } from './student-former-school-table.component';

describe('StudentFormerSchoolTableComponent', () => {
  let component: StudentFormerSchoolTableComponent;
  let fixture: ComponentFixture<StudentFormerSchoolTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentFormerSchoolTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentFormerSchoolTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
