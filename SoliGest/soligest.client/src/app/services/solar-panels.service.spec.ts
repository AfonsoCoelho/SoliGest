import { TestBed } from '@angular/core/testing';

import { SolarPanelsService } from './solar-panels.service';

describe('SolarPanelsService', () => {
  let service: SolarPanelsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SolarPanelsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
