import { TestBed } from '@angular/core/testing';

import { PipsheetCommentsService } from './pipsheet-comments.service';

describe('PipsheetCommentsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PipsheetCommentsService = TestBed.get(PipsheetCommentsService);
    expect(service).toBeTruthy();
  });
});
