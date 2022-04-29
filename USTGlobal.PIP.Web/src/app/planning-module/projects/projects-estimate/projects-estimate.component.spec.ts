import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectsEstimateComponent } from './projects-estimate.component';

describe('ProjectsEstimateComponent', () => {
  let component: ProjectsEstimateComponent;
  let fixture: ComponentFixture<ProjectsEstimateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProjectsEstimateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProjectsEstimateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
