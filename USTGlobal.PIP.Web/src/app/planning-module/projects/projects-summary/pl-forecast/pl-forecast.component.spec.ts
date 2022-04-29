import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PlForecastComponent } from './pl-forecast.component';

describe('PlForecastComponent', () => {
  let component: PlForecastComponent;
  let fixture: ComponentFixture<PlForecastComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PlForecastComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PlForecastComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
