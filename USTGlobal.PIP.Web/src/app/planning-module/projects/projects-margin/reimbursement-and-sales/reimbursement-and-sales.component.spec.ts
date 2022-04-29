import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReimbursementAndSalesComponent } from './reimbursement-and-sales.component';

describe('ReimbursementAndSalesComponent', () => {
  let component: ReimbursementAndSalesComponent;
  let fixture: ComponentFixture<ReimbursementAndSalesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReimbursementAndSalesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReimbursementAndSalesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
