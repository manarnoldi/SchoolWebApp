import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OccurenceTypesComponent } from './occurence-types.component';

describe('OccurenceTypesComponent', () => {
  let component: OccurenceTypesComponent;
  let fixture: ComponentFixture<OccurenceTypesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [OccurenceTypesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(OccurenceTypesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
