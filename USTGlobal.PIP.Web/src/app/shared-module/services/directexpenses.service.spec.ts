import { TestBed } from '@angular/core/testing';

import { DirectexpensesService } from './directexpenses.service';

describe('DirectexpensesService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: DirectexpensesService = TestBed.get(DirectexpensesService);
    expect(service).toBeTruthy();
  });
});
