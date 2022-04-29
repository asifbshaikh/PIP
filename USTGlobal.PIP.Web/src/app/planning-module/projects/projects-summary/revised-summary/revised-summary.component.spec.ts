import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RevisedSummaryComponent } from './revised-summary.component';

describe('RevisedSummaryComponent', () => {
  let component: RevisedSummaryComponent;
  let fixture: ComponentFixture<RevisedSummaryComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RevisedSummaryComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RevisedSummaryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
