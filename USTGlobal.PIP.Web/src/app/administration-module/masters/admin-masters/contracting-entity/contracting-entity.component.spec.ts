import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractingEntityComponent } from './contracting-entity.component';

describe('ContractingEntityComponent', () => {
  let component: ContractingEntityComponent;
  let fixture: ComponentFixture<ContractingEntityComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ContractingEntityComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractingEntityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
