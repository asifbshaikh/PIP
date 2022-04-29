import { IBase } from './base';

export interface IAccountNameEntity extends IBase {
    accountId: number;
    accountName: string;
    accountCode: string;
    paymentLag: number;
}
