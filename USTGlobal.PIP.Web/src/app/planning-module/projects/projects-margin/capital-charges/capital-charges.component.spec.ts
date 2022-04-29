import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CapitalChargesComponent } from './capital-charges.component';

describe('CapitalChargesComponent', () => {
  let component: CapitalChargesComponent;
  let fixture: ComponentFixture<CapitalChargesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CapitalChargesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CapitalChargesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
