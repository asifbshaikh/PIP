import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PipCheckInComponent } from './pip-check-in.component';

describe('PipCheckInComponent', () => {
  let component: PipCheckInComponent;
  let fixture: ComponentFixture<PipCheckInComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PipCheckInComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PipCheckInComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
