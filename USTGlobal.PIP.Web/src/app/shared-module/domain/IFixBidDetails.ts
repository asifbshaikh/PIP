import { IFixBidPeriods } from './IFixBidPeriods';

export interface IFixBidDetails {
    costMarginId: number;
    descriptionId: number;
    totalCost: number;
    periodDetails: IFixBidPeriods[];
}
