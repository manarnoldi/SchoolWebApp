import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentParentsNewFormComponent } from './student-parents-new-form.component';

describe('StudentParentsNewFormComponent', () => {
  let component: StudentParentsNewFormComponent;
  let fixture: ComponentFixture<StudentParentsNewFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentParentsNewFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentParentsNewFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
