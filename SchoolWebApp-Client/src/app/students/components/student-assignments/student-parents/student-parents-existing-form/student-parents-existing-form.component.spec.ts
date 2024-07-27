import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentParentsExistingFormComponent } from './student-parents-existing-form.component';

describe('StudentParentsExistingFormComponent', () => {
  let component: StudentParentsExistingFormComponent;
  let fixture: ComponentFixture<StudentParentsExistingFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentParentsExistingFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentParentsExistingFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
