import { IMargin } from './margin';
import { ILabor } from './labor';
import { IBase } from './base';
import { ProjectPeriod } from './projectperiod';


export interface ILaborPricing extends IBase {
    marginDTO: IMargin;
    resourceLaborPricingDTOs: ILabor[];
    projectPeriodDTO: ProjectPeriod[];
    isDeliveryTypeRestricted: boolean;
}
