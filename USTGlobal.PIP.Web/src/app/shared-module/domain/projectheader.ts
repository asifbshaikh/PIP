import { IBase } from './base';

export interface IProjectHeader extends IBase {
  projectId: number;
  sfProjectId: string;
  projectName: string;
  accountId?: number;
  contractingEntityId?: number;
  beatTax?: boolean;
  deliveryOwner: string;
  servicePortfolioId?: number;
  serviceLineId?: number;
  projectDeliveryTypeId?: number;
  submittedBy: number;
  projectBillingTypeId?: number;
  pipSheetId: number;
  versionNumber?: number;
  errorCode: number;
  currencyId: number;
  lastUpdatedBy: string;
  pipsheetCreatedBy: string;
  approverComments: string;
  approverStatusId: number;
  approvedBy: string;
  approvedOn: Date;
  resendComments: string;
  resendBy: string;
  resendOn: Date;
  isDummy: boolean;
}
