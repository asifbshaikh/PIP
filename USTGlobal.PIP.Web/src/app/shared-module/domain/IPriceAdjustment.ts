import { IPriceAdjustmentLocation, IProjectDuration, IPriceAdjustmentYoy } from '@shared';
export interface IPriceAdjustment {

    priceAdjustmentYoyDTO: IPriceAdjustmentYoy;
    colaDTO: IPriceAdjustmentLocation[];
    projectDurationDTO: IProjectDuration;
}
