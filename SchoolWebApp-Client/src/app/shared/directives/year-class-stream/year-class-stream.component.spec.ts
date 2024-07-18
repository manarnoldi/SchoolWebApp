import { ComponentFixture, TestBed } from '@angular/core/testing';

import { YearClassStreamComponent } from './year-class-stream.component';

describe('YearClassStreamComponent', () => {
  let component: YearClassStreamComponent;
  let fixture: ComponentFixture<YearClassStreamComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [YearClassStreamComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(YearClassStreamComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
