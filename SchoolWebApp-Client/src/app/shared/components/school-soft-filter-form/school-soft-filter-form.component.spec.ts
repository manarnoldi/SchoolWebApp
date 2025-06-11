import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SchoolSoftFilterFormComponent } from './school-soft-filter-form.component';

describe('SchoolSoftFilterFormComponent', () => {
  let component: SchoolSoftFilterFormComponent;
  let fixture: ComponentFixture<SchoolSoftFilterFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SchoolSoftFilterFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SchoolSoftFilterFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
