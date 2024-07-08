import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GradesAddFormComponent } from './grades-add-form.component';

describe('GradesAddFormComponent', () => {
  let component: GradesAddFormComponent;
  let fixture: ComponentFixture<GradesAddFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GradesAddFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GradesAddFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
