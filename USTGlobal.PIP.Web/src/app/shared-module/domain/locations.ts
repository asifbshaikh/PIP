import { IBase } from './base';

export interface ILocation extends IBase {
    locationId: number;
    locationName: string;
    hoursPerDay: number;
    hoursPerMonth: number;
    countryID: number;
    refUSD: number;
    comments: string;
    ebitdaSeatCost: number;
    inflationRate: number;
    startDate: string;
    endDate: Date;
    isActive: boolean;
    status: number;
    isDeleted: boolean;
}
