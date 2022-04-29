import { TestBed } from '@angular/core/testing';

import { UserWorkflowService } from './user-workflow.service';

describe('UserWorkflowService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: UserWorkflowService = TestBed.get(UserWorkflowService);
    expect(service).toBeTruthy();
  });
});
