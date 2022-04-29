import { TestBed } from '@angular/core/testing';

import { OtherpriceadjustmentService } from './other-price-adjustment.service';

describe('OtherpriceadjustmentService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: OtherpriceadjustmentService = TestBed.get(OtherpriceadjustmentService);
    expect(service).toBeTruthy();
  });
});
