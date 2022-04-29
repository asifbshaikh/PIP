import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EffortSummaryComponent } from './effort-summary.component';

describe('EffortSummaryComponent', () => {
  let component: EffortSummaryComponent;
  let fixture: ComponentFixture<EffortSummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EffortSummaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EffortSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
