import { TestBed } from '@angular/core/testing';

import { LabourPricingService } from './labour-pricing.service';

describe('LabourPricingService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: LabourPricingService = TestBed.get(LabourPricingService);
    expect(service).toBeTruthy();
  });
});
