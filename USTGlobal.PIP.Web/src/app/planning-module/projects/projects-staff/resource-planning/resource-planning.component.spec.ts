import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourcePlanningComponent } from './resource-planning.component';

describe('ResourcePlanningComponent', () => {
  let component: ResourcePlanningComponent;
  let fixture: ComponentFixture<ResourcePlanningComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResourcePlanningComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResourcePlanningComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
