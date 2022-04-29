export interface ProjectPeriod {
    projectPeriodId: number;
    pipSheetId: number;
    billingPeriodId: number;
    month: number;
    year: number;
    cappedCost: number;
    inflation: number;
}
