import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentClassTableComponent } from './student-class-table.component';

describe('StudentClassTableComponent', () => {
  let component: StudentClassTableComponent;
  let fixture: ComponentFixture<StudentClassTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentClassTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentClassTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
