import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GeneralOutcomesComponent } from './general-outcomes.component';

describe('GeneralOutcomesComponent', () => {
  let component: GeneralOutcomesComponent;
  let fixture: ComponentFixture<GeneralOutcomesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GeneralOutcomesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GeneralOutcomesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
