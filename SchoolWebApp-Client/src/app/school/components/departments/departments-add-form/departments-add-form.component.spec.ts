import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DepartmentsAddFormComponent } from './departments-add-form.component';

describe('DepartmentsAddFormComponent', () => {
  let component: DepartmentsAddFormComponent;
  let fixture: ComponentFixture<DepartmentsAddFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DepartmentsAddFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DepartmentsAddFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
