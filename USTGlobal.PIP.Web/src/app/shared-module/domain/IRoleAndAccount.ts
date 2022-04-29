import { IBase } from '.';

export interface IRoleAndAccount extends IBase {
  roleId: number;
  roleName: string;
  accountId: number;
  accountName: string;
}
