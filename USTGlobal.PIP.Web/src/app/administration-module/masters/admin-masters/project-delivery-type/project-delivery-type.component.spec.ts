import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProjectDeliveryTypeComponent } from './project-delivery-type.component';

describe('ProjectDeliveryTypeComponent', () => {
  let component: ProjectDeliveryTypeComponent;
  let fixture: ComponentFixture<ProjectDeliveryTypeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProjectDeliveryTypeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProjectDeliveryTypeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
