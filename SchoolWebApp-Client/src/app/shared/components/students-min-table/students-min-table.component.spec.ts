import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentsMinTableComponent } from './students-min-table.component';

describe('StudentsMinTableComponent', () => {
  let component: StudentsMinTableComponent;
  let fixture: ComponentFixture<StudentsMinTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentsMinTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentsMinTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
