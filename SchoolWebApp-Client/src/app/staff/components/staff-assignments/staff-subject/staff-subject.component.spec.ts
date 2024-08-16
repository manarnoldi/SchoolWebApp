import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffSubjectComponent } from './staff-subject.component';

describe('StaffSubjectComponent', () => {
  let component: StaffSubjectComponent;
  let fixture: ComponentFixture<StaffSubjectComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StaffSubjectComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffSubjectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
