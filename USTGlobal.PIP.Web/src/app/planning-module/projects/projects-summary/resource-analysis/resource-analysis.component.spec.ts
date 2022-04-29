import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceAnalysisComponent } from './resource-analysis.component';

describe('ResourceAnalysisComponent', () => {
  let component: ResourceAnalysisComponent;
  let fixture: ComponentFixture<ResourceAnalysisComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResourceAnalysisComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourceAnalysisComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
