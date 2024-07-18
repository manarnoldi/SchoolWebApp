import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonSelectComponent } from './person-select.component';

describe('PersonSelectComponent', () => {
  let component: PersonSelectComponent;
  let fixture: ComponentFixture<PersonSelectComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PersonSelectComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PersonSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
