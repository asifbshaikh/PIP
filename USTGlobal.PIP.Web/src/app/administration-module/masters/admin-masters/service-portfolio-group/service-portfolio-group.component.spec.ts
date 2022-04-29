import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServicePortfolioGroupComponent } from './service-portfolio-group.component';

describe('ServicePortfolioGroupComponent', () => {
  let component: ServicePortfolioGroupComponent;
  let fixture: ComponentFixture<ServicePortfolioGroupComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServicePortfolioGroupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServicePortfolioGroupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
