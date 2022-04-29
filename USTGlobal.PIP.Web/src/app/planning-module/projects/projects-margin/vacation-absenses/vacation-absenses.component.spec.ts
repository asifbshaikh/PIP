import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VacationAbsensesComponent } from './vacation-absenses.component';

describe('VacationAbsensesComponent', () => {
  let component: VacationAbsensesComponent;
  let fixture: ComponentFixture<VacationAbsensesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VacationAbsensesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VacationAbsensesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
