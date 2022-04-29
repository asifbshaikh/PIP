import { IProjectMilestone } from './IProjectMilestone';
import { IOtherPriceAdjustment } from './IOtherPriceAdjustment';
import { from } from 'rxjs';
import { IPeriodOtherPriceAdjustment } from './IPeriodOtherPriceAdjustment';
import { ProjectPeriod } from './projectperiod';
export interface OtherPriceAdjustment {
  otherPriceAdjustmentParent: IOtherPriceAdjustment[];
  projectMilestone: IProjectMilestone[];
  projectPeriod: ProjectPeriod[];
  isMonthlyFeeAdjustment: boolean;
}

