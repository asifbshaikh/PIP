import { IBase } from './base';

export interface IDeliveryType extends IBase {
    projectDeliveryTypeId: number;
    deliveryType: string;
    masterVersionId: number;
}
