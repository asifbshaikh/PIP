import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DefineAdminComponent } from './define-admin.component';

describe('DefineAdminComponent', () => {
  let component: DefineAdminComponent;
  let fixture: ComponentFixture<DefineAdminComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DefineAdminComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DefineAdminComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
