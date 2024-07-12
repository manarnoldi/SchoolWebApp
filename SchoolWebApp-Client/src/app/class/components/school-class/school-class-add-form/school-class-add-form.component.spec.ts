import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SchoolClassAddFormComponent } from './school-class-add-form.component';

describe('SchoolClassAddFormComponent', () => {
  let component: SchoolClassAddFormComponent;
  let fixture: ComponentFixture<SchoolClassAddFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SchoolClassAddFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SchoolClassAddFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
