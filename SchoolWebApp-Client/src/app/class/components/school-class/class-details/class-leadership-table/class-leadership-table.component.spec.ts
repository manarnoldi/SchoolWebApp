import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClassLeadershipTableComponent } from './class-leadership-table.component';

describe('ClassLeadershipTableComponent', () => {
  let component: ClassLeadershipTableComponent;
  let fixture: ComponentFixture<ClassLeadershipTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClassLeadershipTableComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClassLeadershipTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
