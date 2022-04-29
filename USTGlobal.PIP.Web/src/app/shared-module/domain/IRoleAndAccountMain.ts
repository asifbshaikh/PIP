import { IAccountId } from './IAccountId';
import { IRoleAndAccount } from './IRoleAndAccount';
import { IBase } from '.';

export interface IRoleAndAccountMain extends IBase {
  roleAndAccountDTO: IRoleAndAccount[];
  sharedAccountRoles: IRoleAndAccount[];
  accountLevelAccessIds: IAccountId[];
}
