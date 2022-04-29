import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LocationWiseDetailsComponent } from './location-wise-details.component';

describe('LocationWiseDetailsComponent', () => {
  let component: LocationWiseDetailsComponent;
  let fixture: ComponentFixture<LocationWiseDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LocationWiseDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LocationWiseDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
