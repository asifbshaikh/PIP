import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DefineFinancePocComponent } from './define-finance-poc.component';

describe('DefineFinancePocComponent', () => {
  let component: DefineFinancePocComponent;
  let fixture: ComponentFixture<DefineFinancePocComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DefineFinancePocComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DefineFinancePocComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
