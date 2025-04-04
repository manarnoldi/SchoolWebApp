import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClassDetailsComponent } from './class-details.component';

describe('ClassDetailsComponent', () => {
  let component: ClassDetailsComponent;
  let fixture: ComponentFixture<ClassDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClassDetailsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClassDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
