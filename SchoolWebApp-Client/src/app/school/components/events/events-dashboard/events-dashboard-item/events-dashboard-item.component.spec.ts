import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EventsDashboardItemComponent } from './events-dashboard-item.component';

describe('EventsDashboardItemComponent', () => {
  let component: EventsDashboardItemComponent;
  let fixture: ComponentFixture<EventsDashboardItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EventsDashboardItemComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EventsDashboardItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
