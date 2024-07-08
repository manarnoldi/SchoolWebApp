import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubjectGroupsComponent } from './subject-groups.component';

describe('SubjectGroupsComponent', () => {
  let component: SubjectGroupsComponent;
  let fixture: ComponentFixture<SubjectGroupsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SubjectGroupsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubjectGroupsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
