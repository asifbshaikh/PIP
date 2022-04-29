import { IBase } from './base';

export interface IBillingType extends IBase {

    projectBillingTypeId: number;
    billingTypeName: string;
    masterVersionId: number;
}
