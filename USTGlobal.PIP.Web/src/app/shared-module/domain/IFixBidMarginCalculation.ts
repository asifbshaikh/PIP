import { ProjectPeriod } from './projectperiod';
import { IMarginDetails } from './IMarginDetails';
import { IFixBidDetails } from './IFixBidDetails';

export interface IFixBidMarginCalculation {
    marginDTO: IMarginDetails;
    fixBidMarginDTO: IFixBidDetails[];
    projectPeriodDTO: ProjectPeriod[];
    marginBeforeAdjustment: number;
}
