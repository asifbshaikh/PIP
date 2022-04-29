import { TestBed } from '@angular/core/testing';

import { AddNewLocationService } from './add-new-location.service';

describe('AddNewLocationService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AddNewLocationService = TestBed.get(AddNewLocationService);
    expect(service).toBeTruthy();
  });
});
