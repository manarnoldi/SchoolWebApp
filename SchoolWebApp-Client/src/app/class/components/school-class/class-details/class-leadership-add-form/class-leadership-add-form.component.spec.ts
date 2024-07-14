import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClassLeadershipAddFormComponent } from './class-leadership-add-form.component';

describe('ClassLeadershipAddFormComponent', () => {
  let component: ClassLeadershipAddFormComponent;
  let fixture: ComponentFixture<ClassLeadershipAddFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ClassLeadershipAddFormComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClassLeadershipAddFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
