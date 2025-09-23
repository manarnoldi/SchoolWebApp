import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BroadsheetComponent } from './broadsheet.component';

describe('BroadsheetComponent', () => {
  let component: BroadsheetComponent;
  let fixture: ComponentFixture<BroadsheetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BroadsheetComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BroadsheetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
