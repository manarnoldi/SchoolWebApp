import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParentAddFormComponent } from './parent-add-form.component';

describe('ParentAddFormComponent', () => {
  let component: ParentAddFormComponent;
  let fixture: ComponentFixture<ParentAddFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ParentAddFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ParentAddFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
