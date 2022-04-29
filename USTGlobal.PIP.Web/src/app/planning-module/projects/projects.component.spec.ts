import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ProjectsComponent } from './projects.component';
import { from } from 'rxjs';
import { ButtonModule } from 'primeng/button';
import { ProjectsStaffComponent } from './projects-staff/projects-staff.component';
import { TableModule } from 'primeng/table';
import { ProjectsHeaderComponent } from './projects-staff/projects-header/projects-header.component';
import { StepsModule } from 'primeng/steps';
import { PanelModule } from 'primeng/panel';
import { DropdownModule } from 'primeng/dropdown';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClient } from '@angular/common/http';
import { HttpModule } from '@angular/http';
import { HttpClientModule } from '@angular/common/http';
import { RouterTestingModule } from '@angular/router/testing';
import { SharedDataService } from '@global';
import { Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { TranslateService } from '@ngx-translate/core';


describe('ProjectsComponent', () => {
  let component: ProjectsComponent;
  let fixture: ComponentFixture<ProjectsComponent>;

  const projectsService = {
    getProjects: (x: any) => {
      return [
      {
        'label': 'projects',
        'value': {
          'id': 1,
          'name': 'projects',
          'code': 'ProjectsGroup'
        }
      },
      {
        'label': 'projects',
        'value': {
          'id': 2,
          'name': 'projects',
          'code': 'Projects'
        }
      }];
    },
    getContractingEntities: (x: any) => {
      return [
        {
          'label': 'Contracting Entities',
          'value': {
            'id': 1,
            'name': 'Contracting Entities',
            'code': 'ContractingEntities'
          }
        },
        {
          'label': 'Contracting Entities',
          'value': {
            'id': 2,
            'name': 'Contracting Entities',
            'code': 'ContractingEntities'
          }
        }];
    },
    getServicePortfolios: (x: any) => {
      return [
        {
          'label': 'Service Portfolio Group',
          'value': {
            'id': 1,
            'name': 'Service Portfolio Group',
            'code': 'ServicePortfolioGroup'
          }
        },
        {
          'label': 'Service Portfolio Group',
          'value': {
            'id': 2,
            'name': 'Service Portfolio Group',
            'code': 'ServicePortfolioGroup'
          }
        }];
    },
    getServiceLines: (x: any) => {
      return [
        {
          'label': 'Service Line',
          'value': {
            'id': 1,
            'name': 'Service Line',
            'code': 'ServiceLine'
          }
        },
        {
          'label': 'Service Line',
          'value': {
            'id': 2,
            'name': 'Service Line',
            'code': 'ServiceLine'
          }
        }];
    },
    getDeliveryTypes: (x: any) => {
      return [
        {
          'label': 'Delivery Types',
          'value': {
            'id': 1,
            'name': 'Delivery Types',
            'code': 'DeliveryTypes'
          }
        },
        {
          'label': 'Delivery Types',
          'value': {
            'id': 2,
            'name': 'Delivery Types',
            'code': 'DeliveryTypes'
          }
        }];
    },
    getBillingTypes: (x: any) => {
      return [
        {
          'label': 'Billing Types',
          'value': {
            'id': 1,
            'name': 'Billing Types',
            'code': 'BillingTypes'
          }
        },
        {
          'label': 'Billing Types',
          'value': {
            'id': 2,
            'name': 'Billing Types',
            'code': 'BillingTypes'
          }
        }];
    }
  };

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ProjectsComponent, ProjectsStaffComponent, ProjectsHeaderComponent],
      imports: [
        ButtonModule,
        TableModule,
        StepsModule,
        PanelModule,
        DropdownModule,
        FormsModule,
        BrowserModule,
        HttpClientModule,
        RouterTestingModule

      ],
      providers: [
        HttpClient
      ],

    })
      .compileComponents();
  }));

  beforeEach(() => {
    // component = new ProjectsComponent(projectsService as any);
    fixture = TestBed.createComponent(ProjectsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create Project component', () => {
    expect(component).toBeTruthy();
  });

  it('should  get projects', () => {
    expect(projectsService.getProjects).toBeDefined();
  });
  it('should  get contracting entities', () => {
    expect(projectsService.getContractingEntities).toBeDefined();
  });

  it('should  get service portfolios', () => {
    expect(projectsService.getServicePortfolios).toBeDefined();
  });

  it('should  get service lines', () => {
    expect(projectsService.getServiceLines).toBeDefined();
  });

  it('should  get delivery types', () => {
    expect(projectsService.getDeliveryTypes).toBeDefined();
  });

  it('should  get billing types', () => {
    expect(projectsService.getBillingTypes).toBeDefined();
  });

  it('should create Project component', () => {
    expect(component).not.toBeTruthy();
  });

  it('should not get projects', () => {
    expect(projectsService.getProjects).toBe(null);
  });
  it('should not get contracting entities', () => {
    expect(projectsService.getContractingEntities).not.toBeDefined();
  });

  it('should not get service portfolios', () => {
    expect(projectsService.getServicePortfolios).not.toBeDefined();
  });

  it('should not get service lines', () => {
    expect(projectsService.getServiceLines).not.toBeDefined();
  });

  it('should not get delivery types', () => {
    expect(projectsService.getDeliveryTypes).not.toBeDefined();
  });

  it('should not get billing types', () => {
    expect(projectsService.getBillingTypes).not.toBeDefined();
  });

});
