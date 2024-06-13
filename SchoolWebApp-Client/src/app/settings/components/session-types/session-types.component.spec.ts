import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SessionTypesComponent } from './session-types.component';

describe('SessionTypesComponent', () => {
  let component: SessionTypesComponent;
  let fixture: ComponentFixture<SessionTypesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SessionTypesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SessionTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
