import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserRolesComponent } from './userroles.component';

describe('UserrolesComponent', () => {
  let component: UserRolesComponent;
  let fixture: ComponentFixture<UserRolesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserRolesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserRolesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
