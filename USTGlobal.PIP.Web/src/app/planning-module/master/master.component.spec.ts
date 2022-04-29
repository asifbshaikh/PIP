import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MasterComponent } from './master.component';
import { MultiSelectModule } from 'primeng/multiselect';
import { TableModule } from 'primeng/table';
import { DropdownModule } from 'primeng/dropdown';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { HttpClient } from '@angular/common/http';
import { BrowserModule } from '@angular/platform-browser';
import {HttpClientModule} from '@angular/common/http';


describe('MasterComponent', () => {
  let component: MasterComponent;
  let fixture: ComponentFixture<MasterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MasterComponent ],
      imports: [
        MultiSelectModule,
        TableModule,
        DropdownModule,
        FormsModule,
        BrowserModule,
        HttpClientModule
      ],
      providers: [
        HttpClient
      ],
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
