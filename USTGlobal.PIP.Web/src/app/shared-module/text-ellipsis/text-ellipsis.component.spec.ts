/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { TextEllipsisComponent } from './text-ellipsis.component';

describe('TextEllipsisComponent', () => {
  let component: TextEllipsisComponent;
  let fixture: ComponentFixture<TextEllipsisComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TextEllipsisComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TextEllipsisComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
