import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FixedBidAndMarginCalcComponent } from './fixed-bid-and-margin-calc.component';

describe('FixedBidAndMarginCalcComponent', () => {
  let component: FixedBidAndMarginCalcComponent;
  let fixture: ComponentFixture<FixedBidAndMarginCalcComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FixedBidAndMarginCalcComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FixedBidAndMarginCalcComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
