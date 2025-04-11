import { TestBed } from '@angular/core/testing';

import { PriorityLevelService } from './priorityLevel.service';

describe('ServicesService', () => {
  let service: PriorityLevelService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PriorityLevelService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
