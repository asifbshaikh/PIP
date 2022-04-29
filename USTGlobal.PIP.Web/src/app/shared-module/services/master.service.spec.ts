import { TestBed } from '@angular/core/testing';

import { MasterService } from './master.service';
import { HttpClient} from '@angular/common/http';
import { HttpClientModule } from '@angular/common/http';

describe('MasterService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [
      HttpClientModule
    ],
    providers: [
      HttpClient
    ],
  }));

  it('should be created', () => {
    const service: MasterService = TestBed.get(MasterService);
    expect(service).toBeTruthy();
  });
});
