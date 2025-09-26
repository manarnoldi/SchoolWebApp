import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CbeComponent } from './cbe.component';

describe('CbeComponent', () => {
  let component: CbeComponent;
  let fixture: ComponentFixture<CbeComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CbeComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CbeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
