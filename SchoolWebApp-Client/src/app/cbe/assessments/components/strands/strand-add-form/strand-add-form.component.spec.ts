import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StrandAddFormComponent } from './strand-add-form.component';

describe('StrandAddFormComponent', () => {
  let component: StrandAddFormComponent;
  let fixture: ComponentFixture<StrandAddFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [StrandAddFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StrandAddFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
