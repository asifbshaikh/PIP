import { IRole } from './IRole';

export interface IUserRole {
    userId: number;
    email: string;
    name: string;
    firstName: string;
    lastName: string;
    uid: string;
    roleId: number;
    roles:  string;
    account:  string;
    accountId: number;
    isAdmin: boolean;
    isFinanceApprover: boolean;
    isEditor: boolean;
    isReviewer: boolean;
    isReadOnly: boolean;
    isAllAccountReadOnly: boolean;
    isDisabled: boolean;
    isDataToBeSaved: boolean;
}
