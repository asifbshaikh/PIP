import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SharePipComponent } from './share-pip.component';

describe('SharePipComponent', () => {
  let component: SharePipComponent;
  let fixture: ComponentFixture<SharePipComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SharePipComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SharePipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
