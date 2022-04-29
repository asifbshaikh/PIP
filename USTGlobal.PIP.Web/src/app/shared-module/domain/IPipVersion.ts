import { IBase } from './base';

export interface IPipVersion extends IBase {
  projectId: number;
  pipSheetId: number;
  accountId: number;
  versionNumber: number;
  status: string;
  modifiedBy: string;
  modifiedOn: Date;
  userComments: string;
  approverComments: string;
  resendBy: string;
  resendComments: string;
  resendOn: Date;
  approvedBy: string;
  approvedOn: Date;
  isCheckedOut: boolean;
  roleName: string[];
  checkedInOutBy: number;
  checkedInOutByName: string;
  approverStatusId: number;
  hasAccountLevelAccess: boolean;
  sfProjectId: string;
}
