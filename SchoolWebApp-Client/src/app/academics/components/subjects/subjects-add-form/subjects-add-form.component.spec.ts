import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubjectsAddFormComponent } from './subjects-add-form.component';

describe('SubjectsAddFormComponent', () => {
  let component: SubjectsAddFormComponent;
  let fixture: ComponentFixture<SubjectsAddFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SubjectsAddFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubjectsAddFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
