import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SubjectsMinTableComponent } from './subjects-min-table.component';

describe('SubjectsMinTableComponent', () => {
  let component: SubjectsMinTableComponent;
  let fixture: ComponentFixture<SubjectsMinTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SubjectsMinTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SubjectsMinTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
