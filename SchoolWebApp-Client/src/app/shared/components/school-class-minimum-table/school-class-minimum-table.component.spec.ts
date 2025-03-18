import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SchoolClassMinimumTableComponent } from './school-class-minimum-table.component';

describe('SchoolClassMinimumTableComponent', () => {
  let component: SchoolClassMinimumTableComponent;
  let fixture: ComponentFixture<SchoolClassMinimumTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SchoolClassMinimumTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SchoolClassMinimumTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
