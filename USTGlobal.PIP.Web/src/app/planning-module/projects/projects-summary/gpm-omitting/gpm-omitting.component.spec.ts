import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GpmOmittingComponent } from './gpm-omitting.component';

describe('GpmOmittingComponent', () => {
  let component: GpmOmittingComponent;
  let fixture: ComponentFixture<GpmOmittingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GpmOmittingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GpmOmittingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
