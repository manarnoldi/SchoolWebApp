import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentParentsComponent } from './student-parents.component';

describe('StudentParentsComponent', () => {
  let component: StudentParentsComponent;
  let fixture: ComponentFixture<StudentParentsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentParentsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentParentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
