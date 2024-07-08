import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StudentsAddFormComponent } from './students-add-form.component';

describe('StudentsAddFormComponent', () => {
  let component: StudentsAddFormComponent;
  let fixture: ComponentFixture<StudentsAddFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StudentsAddFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StudentsAddFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
