import { IclientPricePeriodDTO } from './IclientPricePeriodDTO';

export interface IcashFlowDTO {
    clientPriceId: number;
    clientPricePeriodDTO: IclientPricePeriodDTO[];
    sumOfYearPrice: number;
    year: number;
}
