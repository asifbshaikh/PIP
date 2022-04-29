/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';
import {StepsModule} from 'primeng/steps';
import {PanelModule} from 'primeng/panel';
import { ProjectsMarginComponent } from './projects-margin.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('ProjectsMarginComponent', () => {
  let component: ProjectsMarginComponent;
  let fixture: ComponentFixture<ProjectsMarginComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProjectsMarginComponent ],
      imports: [
        StepsModule,
        PanelModule,
        BrowserAnimationsModule
      ],
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProjectsMarginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
