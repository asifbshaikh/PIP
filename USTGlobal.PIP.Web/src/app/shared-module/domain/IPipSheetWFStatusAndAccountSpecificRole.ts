import { IRoleAndAccount } from '@shared/domain/IRoleAndAccount';
import { IPipSheetWorkflowStatus } from './IPipSheetWorkflowStatus';

export interface IPipSheetWFStatusAndAccountSpecificRole {
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  roleAndAccountDTO: IRoleAndAccount[];
  hasAccountLevelAccess: boolean;
  canNavigate: boolean;
  isDummy: boolean;
}
