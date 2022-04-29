import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NavmenuComponent } from './navmenu.component';
import { PanelMenuModule } from 'primeng/panelmenu';
import { RouterTestingModule } from '@angular/router/testing';

describe('NavmenuComponent', () => {
  let component: NavmenuComponent;
  let fixture: ComponentFixture<NavmenuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NavmenuComponent ],
      imports: [
        PanelMenuModule,
        RouterTestingModule
      ],
      providers: []
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NavmenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
