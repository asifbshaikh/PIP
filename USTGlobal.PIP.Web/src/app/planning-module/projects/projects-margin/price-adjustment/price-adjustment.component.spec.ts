import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PriceAdjustmentComponent } from './price-adjustment.component';

describe('PriceAdjustmentComponent', () => {
  let component: PriceAdjustmentComponent;
  let fixture: ComponentFixture<PriceAdjustmentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PriceAdjustmentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PriceAdjustmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
