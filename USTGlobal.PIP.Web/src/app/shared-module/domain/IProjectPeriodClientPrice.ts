export interface IProjectPeriodClientPrice {
    projectPeriodId: number;
    billingPeriodId: number;
    pipSheetId: number;
    clientPrice: number;
    feesAtRisk: number;
    capitalCharge: number;
    netEstimatedRevenue: number;
}
