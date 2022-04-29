import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { YOYComparisonComponent } from './yoy-comparison.component';

describe('YearCoparisonComponent', () => {
  let component: YOYComparisonComponent;
  let fixture: ComponentFixture<YOYComparisonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [YOYComparisonComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(YOYComparisonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
