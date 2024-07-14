import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffDisciplineComponent } from './staff-discipline.component';

describe('StaffDisciplineComponent', () => {
  let component: StaffDisciplineComponent;
  let fixture: ComponentFixture<StaffDisciplineComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffDisciplineComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffDisciplineComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
