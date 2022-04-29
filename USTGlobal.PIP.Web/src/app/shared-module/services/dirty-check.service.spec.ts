import { TestBed } from '@angular/core/testing';

import { DirtyCheckService } from './dirty-check.service';

describe('DirtyCheckService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: DirtyCheckService = TestBed.get(DirtyCheckService);
    expect(service).toBeTruthy();
  });
});
