import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SchoolStreamsComponent } from './school-streams.component';

describe('SchoolStreamsComponent', () => {
  let component: SchoolStreamsComponent;
  let fixture: ComponentFixture<SchoolStreamsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SchoolStreamsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SchoolStreamsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
