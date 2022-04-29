import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractorMarkupComponent } from './contractor-markup.component';

describe('ContractorMarkupComponent', () => {
  let component: ContractorMarkupComponent;
  let fixture: ComponentFixture<ContractorMarkupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContractorMarkupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractorMarkupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
