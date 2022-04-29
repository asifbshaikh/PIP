import { TestBed, getTestBed } from '@angular/core/testing';
import { ProjectService } from './project.service';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ProjectServiceMocks } from '../services/project.service.spec.mocks';

describe('ProjectService', () => {

let injector: TestBed;
let projectService: ProjectService;
let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, HttpClientModule],
      providers: [HttpClient, ProjectService]
    });
    injector = getTestBed();
    projectService = injector.get(ProjectService);
    httpMock = injector.get(HttpTestingController);
    afterEach(() => {
      httpMock.verify();
    });
  });
  describe('getProjects', () => {
    it('should check for correct project and validate count of projects', () => {
  projectService.getProjects().subscribe(project => {
      expect(project.length).toBe(1);
      expect(project).toEqual(ProjectServiceMocks.mockGetProjects);
    });
});

  it('should check when no projects found', () => {
    projectService.getProjects().subscribe(project => {
      expect(project).not.toEqual(null);
    });
   });
  });

describe('getServicePortfolios', () => {
  it('should check for correct Service Portfolio and validate count', () => {
projectService.getServicePortfolios().subscribe(servicePortfolio => {
    expect(servicePortfolio.length).toBe(1);
    expect(servicePortfolio).toEqual(ProjectServiceMocks.mockGetServicePortfolios);
  });
});

it('should be null when no Service Portfolios found', () => {
  projectService.getServicePortfolios().subscribe(servicePortfolio => {
    expect(servicePortfolio).toEqual(null);
  });
 });
});


describe('getServiceLines', () => {

it('should check for correct Service Line and validate the count', () => {
  projectService.getServiceLines().subscribe(serviceLine => {
    expect(serviceLine.length).toBe(1);
    expect(serviceLine).toEqual(ProjectServiceMocks.mockGetServiceLines);
  });
});

it('should be null when no Service Lines are found', () => {
  projectService.getServiceLines().subscribe(serviceLine => {
    expect(serviceLine).toEqual(null);
  });
 });
});

describe('getContractingEntities', () => {
  it('should check for correct Contracting Entities and validate the count', () => {

projectService.getContractingEntities().subscribe(contractingEntity => {
    expect(contractingEntity.length).toBe(1);
    expect(contractingEntity).toEqual(ProjectServiceMocks.mockGetContractingEntities);
  });
});

it('should check when no Contracting Entities are found', () => {
  projectService.getContractingEntities().subscribe(contractingEntity => {
    expect(contractingEntity).toEqual(null);
  });
 });
});


describe('getDeliveryTypes', () => {
  it('should check for correct Delivery Types and validate the count', () => {

projectService.getDeliveryTypes().subscribe(deliveryType => {
    expect(deliveryType.length).toBe(1);
    expect(deliveryType).toEqual(ProjectServiceMocks.mockGetDeliveryTypes);
  });
});

it('should return null when no delivery types are found', () => {
  projectService.getDeliveryTypes().subscribe(deliveryType => {
    expect(deliveryType).toEqual(null);
  });
 });
});

describe('getBillingTypes', () => {
  it('should check for correct billing types and validate the count', () => {

projectService.getBillingTypes().subscribe(billingType => {
    expect(billingType.length).toBe(1);
    expect(billingType).toEqual(ProjectServiceMocks.mockGetBillingTypes);
  });
});

it('should return null when no billing types are found', () => {
  projectService.getBillingTypes().subscribe(billingType => {
    expect(billingType).toEqual(null);
  });
 });
});

});
