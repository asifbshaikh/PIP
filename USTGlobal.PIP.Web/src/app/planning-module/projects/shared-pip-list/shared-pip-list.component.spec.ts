import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedPipListComponent } from './shared-pip-list.component';

describe('SharedPipListComponent', () => {
  let component: SharedPipListComponent;
  let fixture: ComponentFixture<SharedPipListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SharedPipListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SharedPipListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
