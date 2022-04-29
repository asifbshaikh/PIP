import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PerformanceIndicatorsComponent } from './performance-indicators.component';

describe('PerformanceIndicatorsComponent', () => {
  let component: PerformanceIndicatorsComponent;
  let fixture: ComponentFixture<PerformanceIndicatorsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PerformanceIndicatorsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PerformanceIndicatorsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
