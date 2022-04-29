import { IRoleAndAccount } from './../../shared-module/domain/IRoleAndAccount';
export interface IUser {
  email: string;
  userId: number;
  firstName: string;
  lastName: string;
  uid: string;
  isActive: boolean;
  roleAndAccountDTO: IRoleAndAccount[];
}
