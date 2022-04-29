import { TestBed } from '@angular/core/testing';

import { ReimbursementSalesService } from './reimbursement-sales.service';

describe('ReimbursementSalesService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ReimbursementSalesService = TestBed.get(ReimbursementSalesService);
    expect(service).toBeTruthy();
  });
});
