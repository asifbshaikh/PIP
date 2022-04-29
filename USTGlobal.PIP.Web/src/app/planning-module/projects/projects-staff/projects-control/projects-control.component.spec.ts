import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectsControlComponent } from './projects-control.component';

describe('ProjectsControlComponent', () => {
  let component: ProjectsControlComponent;
  let fixture: ComponentFixture<ProjectsControlComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProjectsControlComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProjectsControlComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
