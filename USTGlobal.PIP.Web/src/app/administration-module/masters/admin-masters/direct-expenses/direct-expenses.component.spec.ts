import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DirectExpensesComponent } from './direct-expenses.component';

describe('DefaultLabelsComponent', () => {
  let component: DirectExpensesComponent;
  let fixture: ComponentFixture<DirectExpensesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DirectExpensesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DirectExpensesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
