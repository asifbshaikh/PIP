import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ExpensesAndAssetsComponent } from './expenses-and-assets.component';

describe('ExpensesAndAssetsComponent', () => {
  let component: ExpensesAndAssetsComponent;
  let fixture: ComponentFixture<ExpensesAndAssetsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ExpensesAndAssetsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ExpensesAndAssetsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
