import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EventsAddFormComponent } from './events-add-form.component';

describe('EventsAddFormComponent', () => {
  let component: EventsAddFormComponent;
  let fixture: ComponentFixture<EventsAddFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EventsAddFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EventsAddFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
