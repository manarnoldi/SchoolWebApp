import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffDisciplineFormComponent } from './staff-discipline-form.component';

describe('StaffDisciplineFormComponent', () => {
  let component: StaffDisciplineFormComponent;
  let fixture: ComponentFixture<StaffDisciplineFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffDisciplineFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffDisciplineFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
