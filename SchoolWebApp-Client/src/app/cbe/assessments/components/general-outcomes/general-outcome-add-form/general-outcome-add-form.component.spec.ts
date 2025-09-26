import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GeneralOutcomeAddFormComponent } from './general-outcome-add-form.component';

describe('GeneralOutcomeAddFormComponent', () => {
  let component: GeneralOutcomeAddFormComponent;
  let fixture: ComponentFixture<GeneralOutcomeAddFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GeneralOutcomeAddFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GeneralOutcomeAddFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
