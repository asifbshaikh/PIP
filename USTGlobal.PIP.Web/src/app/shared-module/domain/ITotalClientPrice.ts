import { ITotalClientPricePeriods } from './ITotalClientPricePeriods';

export interface ITotalClientPrice {
    uId: number;
    clientPriceId: number;
    pipSheetId: number;
    descriptionId: number;
    totalPrice: number;
    clientPricePeriodDTO: ITotalClientPricePeriods[];
}
