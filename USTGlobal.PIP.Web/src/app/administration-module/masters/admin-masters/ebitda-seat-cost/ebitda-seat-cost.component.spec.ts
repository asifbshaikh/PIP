import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EbitdaSeatCostComponent } from './ebitda-seat-cost.component';

describe('EbitdaSeatCostComponent', () => {
  let component: EbitdaSeatCostComponent;
  let fixture: ComponentFixture<EbitdaSeatCostComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EbitdaSeatCostComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EbitdaSeatCostComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
