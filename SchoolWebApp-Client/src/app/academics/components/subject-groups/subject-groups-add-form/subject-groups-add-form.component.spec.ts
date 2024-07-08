import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubjectGroupsAddFormComponent } from './subject-groups-add-form.component';

describe('SubjectGroupsAddFormComponent', () => {
  let component: SubjectGroupsAddFormComponent;
  let fixture: ComponentFixture<SubjectGroupsAddFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SubjectGroupsAddFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubjectGroupsAddFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
