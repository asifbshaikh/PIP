import { IProject } from '@shared/domain/project';
import { IServicePortfolio } from '@shared/domain/serviceportfolio';
import { IServiceLine } from '@shared/domain/serviceline';
import { IContractingEntity } from '@shared/domain/contractingEntity';
import { IDeliveryType } from '@shared/domain/deliveryType';
import { IBillingType } from '@shared/domain/billingType';
export class ProjectServiceMocks {
  static mockGetProjects: Array<IProject> = [{
    'projectId': 1,
    'sfProjectId': 'CBDA-1234-12-45',
    'projectName': 'PIP Sheets',
    'accountId': 19,
    'accountName': 'UST Global Group',
    'pipSheetStatus': 'Not Submitted',
    'serviceLine': 'Business: BI/DW Solutions',
    'deliveryType': 'Staff Augmentation',
    'billingType': 'Time and Expense',
    'pipSheetId': 0,
    'isDummy': true
  }];

  static mockGetServicePortfolios: Array<IServicePortfolio> = [{
    'servicePortfolioId': 1,
    'portfolioName': 'Business Services',
    'masterVersionId': 1,
    'createdBy': 1,
    'updatedBy': 1
  }];

  static mockGetServiceLines: Array<IServiceLine> = [{
    'serviceLineId': 1,
    'serviceLineName': 'Business: BI/DW Solutions',
    'servicePortfolioId': 1,
    'masterVersionId': 1,
    'portfolioName': 'Business Services',
    'createdBy': 1,
    'updatedBy': 1
  }];

  static mockGetContractingEntities: Array<IContractingEntity> = [{
    'contractingEntityId': 1,
    'code': 'INI11',
    'name': 'India',
    'masterVersionId': 1,
    'createdBy': 1,
    'updatedBy': 1
  }];

  static mockGetDeliveryTypes: Array<IDeliveryType> = [{
    'projectDeliveryTypeId': 1,
    'deliveryType': 'Managed Services / SLA Based',
    'masterVersionId': 1,
    'createdBy': 1,
    'updatedBy': 1
  }];

  static mockGetBillingTypes: Array<IBillingType> = [{
    'projectBillingTypeId': 1,
    'billingTypeName': 'Flat Fee Monthly',
    'masterVersionId': 1,
    'createdBy': 1,
    'updatedBy': 1
  }];
}
