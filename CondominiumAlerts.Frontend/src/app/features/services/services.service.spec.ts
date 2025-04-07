import { TestBed } from '@angular/core/testing';

import { priorityLevelService } from './services.service';

describe('ServicesService', () => {
  let service: priorityLevelService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(priorityLevelService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
