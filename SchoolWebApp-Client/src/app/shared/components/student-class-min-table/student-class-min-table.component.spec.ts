import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentClassMinTableComponent } from './student-class-min-table.component';

describe('StudentClassMinTableComponent', () => {
  let component: StudentClassMinTableComponent;
  let fixture: ComponentFixture<StudentClassMinTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentClassMinTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentClassMinTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
