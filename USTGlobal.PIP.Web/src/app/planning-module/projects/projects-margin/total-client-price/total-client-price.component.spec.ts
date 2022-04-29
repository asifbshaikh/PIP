import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TotalClientPriceComponent } from './total-client-price.component';

describe('TotalClientPriceComponent', () => {
  let component: TotalClientPriceComponent;
  let fixture: ComponentFixture<TotalClientPriceComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TotalClientPriceComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TotalClientPriceComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
