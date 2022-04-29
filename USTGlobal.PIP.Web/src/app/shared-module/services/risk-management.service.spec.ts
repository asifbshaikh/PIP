import { TestBed } from '@angular/core/testing';

import { RiskManagementService } from './risk-management.service';

describe('RiskManagementService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: RiskManagementService = TestBed.get(RiskManagementService);
    expect(service).toBeTruthy();
  });
});
