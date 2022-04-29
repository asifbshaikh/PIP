import { ResourcePeriod } from './resourceperiod';

export interface ILabor {
    projectResourceId: number;
    pipSheetId: number;
    alias: string;
    locationId: number;
    locationName: string;
    resourceId: number;
    name: string;
    utilizationType: boolean;
    percent: number;
    rate?: number;
    cost?: number;
    ratePerHour: number;
    yr1PerHour: number;
    margin: number;
    cappedCost: number;
    totalRevenue: number;
    totalHoursPerResource: number;
    costHrsPerResource: number;
    standardCostRate: number;
    totalInflation: number;
    projectResourcePeriodDTO: ResourcePeriod[];
    nonBillableCategoryId: number;
    milestoneName: string;
    resourceServiceLineId: number;
    gradeClientRole: string;
}
