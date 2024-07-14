import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StaffDisciplineTableComponent } from './staff-discipline-table.component';

describe('StaffDisciplineTableComponent', () => {
  let component: StaffDisciplineTableComponent;
  let fixture: ComponentFixture<StaffDisciplineTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StaffDisciplineTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StaffDisciplineTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
