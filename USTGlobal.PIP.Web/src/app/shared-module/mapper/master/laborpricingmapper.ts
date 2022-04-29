import { ILaborPricing, IMargin, ILabor, ProjectPeriod, IPriceAdjustmentLocation } from '@shared';

export class LaborPricingMapper implements ILaborPricing {
    projectPeriodDTO: ProjectPeriod[];
    marginDTO: IMargin;
    resourceLaborPricingDTOs: ILabor[];
    isDeliveryTypeRestricted: boolean;
    createdBy: number;
    updatedBy: number;

    mapper(input: any): this {
        Object.assign(this, input);
        return this;
    }
}
