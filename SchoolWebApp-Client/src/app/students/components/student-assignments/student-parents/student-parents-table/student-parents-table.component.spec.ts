import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentParentsTableComponent } from './student-parents-table.component';

describe('StudentParentsTableComponent', () => {
  let component: StudentParentsTableComponent;
  let fixture: ComponentFixture<StudentParentsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentParentsTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentParentsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
