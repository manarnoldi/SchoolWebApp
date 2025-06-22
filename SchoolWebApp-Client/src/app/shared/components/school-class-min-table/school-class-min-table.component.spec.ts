import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SchoolClassMinTableComponent } from './school-class-min-table.component';

describe('SchoolClassMinTableComponent', () => {
  let component: SchoolClassMinTableComponent;
  let fixture: ComponentFixture<SchoolClassMinTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SchoolClassMinTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SchoolClassMinTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
