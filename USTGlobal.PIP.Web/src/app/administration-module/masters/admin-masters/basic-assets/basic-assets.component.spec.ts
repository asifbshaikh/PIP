import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BasicAssetsComponent } from './basic-assets.component';

describe('BasicAssetsComponent', () => {
  let component: BasicAssetsComponent;
  let fixture: ComponentFixture<BasicAssetsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BasicAssetsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BasicAssetsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
