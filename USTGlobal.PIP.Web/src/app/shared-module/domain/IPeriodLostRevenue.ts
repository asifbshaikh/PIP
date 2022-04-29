export interface IPeriodLostRevenue {
    projectPeriodId: number;
    billingPeriodId: number;
    month: number;
    year: number;
    lostRevenue: number;
    revenue: number;
}
