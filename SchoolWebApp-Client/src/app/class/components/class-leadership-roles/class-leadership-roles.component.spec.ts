import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ClassLeadershipRolesComponent } from './class-leadership-roles.component';

describe('ClassLeadershipRolesComponent', () => {
  let component: ClassLeadershipRolesComponent;
  let fixture: ComponentFixture<ClassLeadershipRolesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ClassLeadershipRolesComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ClassLeadershipRolesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
