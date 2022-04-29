import { IBase } from './base';

export interface IDeliveryBillingType extends IBase {

    projectBillingTypeId: number;
    billingTypeName: string;
    masterVersionId: number;
    projectDeliveryTypeId: number;
    deliveryTypeName: string;
}
