import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OtherPriceAdjustmentComponent } from './other-price-adjustment.component';

describe('OtherPriceAdjustmentComponent', () => {
  let component: OtherPriceAdjustmentComponent;
  let fixture: ComponentFixture<OtherPriceAdjustmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OtherPriceAdjustmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OtherPriceAdjustmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
