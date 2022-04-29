import { IcashFlowDTO } from './IcashFlowDTO';

export interface IcashFlowParentDTO {
    cashFlowDTO: IcashFlowDTO[];
    clientPriceId: number;
    descriptionId: number;
    isOverrideUpdated: boolean;
    pipSheetId: number;
    totalPrice: number;
    uId: number;
}
