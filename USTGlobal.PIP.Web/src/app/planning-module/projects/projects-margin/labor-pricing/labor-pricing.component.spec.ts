import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaborPricingComponent } from './labor-pricing.component';

describe('LaborPricingComponent', () => {
  let component: LaborPricingComponent;
  let fixture: ComponentFixture<LaborPricingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaborPricingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaborPricingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
