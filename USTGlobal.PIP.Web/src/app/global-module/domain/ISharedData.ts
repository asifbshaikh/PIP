import { IPipSheetWorkflowStatus } from './../../shared-module/domain/IPipSheetWorkflowStatus';
import {
  IServicePortfolio, IServiceLine, IDeliveryType, IBillingType, IAccountNameEntity, IContractingEntity, ILocation, IResourceRoleGroup,
  IResourceUSTRole, IResourceMarkup, IProjectMilestone, IHoliday, IProjectMilestoneGroup, IMilestone, ICurrency,
  ICountry, ICorpBillingRate
} from '@shared';
import { IUser } from '@global';
import { IBasicAsset } from '@shared/domain/basicasset';
import { IDefaultLabel } from '@shared/domain/defaultlabel';
import { IDeliveryBillingType } from '@shared/domain/IDeliveryBillingType';
import { IYear } from '@shared/domain/IYear';
import { INonBillableCategory } from '@shared/domain/INonBillableCategory';
import { IRole } from '@shared/domain/IRole';
import { IResourceServiceLine } from './IResourceServiceLine';

export interface ISharedData {
  currencyId: number;
  corpBillingRateDTO: ICorpBillingRate[];
  servicePortfolioDTO: IServicePortfolio[];
  serviceLineDTO: IServiceLine[];
  projectDeliveryTypeDTO: IDeliveryType[];
  projectBillingTypeDTO: IBillingType[];
  contractingEntityDTO: IContractingEntity[];
  locationDTO: ILocation[];
  userRoleAccountDTO: IUser;
  resourceGroupDTO: IResourceRoleGroup[];
  resourceDTO: IResourceUSTRole[];
  resourceServiceLineDTO: IResourceServiceLine[];
  markupDTO: IResourceMarkup[];
  holidayDTO: IHoliday[];
  milestoneGroupDTO: IProjectMilestoneGroup[];
  milestoneDTO: IMilestone[];
  currencyDTO: ICurrency[];
  countryDTO: ICountry[];
  basicAssetDTO: IBasicAsset[];
  defaultLabelDTO: IDefaultLabel[];
  projectDeliveryBillingTypeDTO: IDeliveryBillingType[];
  billingYearDTO: IYear[];
  nonBillableCategoryDTO: INonBillableCategory[];
  accountDTO: IAccountNameEntity[];
  roleDTO: IRole[];
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  hasAccountLevelAccess: boolean;
  hasSharePipAccess: boolean;
  hasDummyPipAccess: boolean;
  hasAccountLevelEditorAccess: boolean;
  hasFinanceApproverAccess: boolean;
}
