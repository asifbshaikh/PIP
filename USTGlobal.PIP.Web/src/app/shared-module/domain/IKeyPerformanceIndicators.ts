import { IServiceLineRevenueList } from './IServiceLineRevenueList';
import { IServiceLineEbitdaPercentList } from './IServiceLineEbitdaPercentList';

export interface IKeyPerformanceIndicators {
    onShoreFTEPercent: number;
    grossMarginPercent: number;
    ebitdaPercent: number;
    serviceLineBlendedTargetEbitda: number;
    variancePercent: number;
    costContingencyPercent: number;
    firstMonthPositiveCashFlow: string;
    serviceLineRevenueList: IServiceLineRevenueList[];
    serviceLineEbitdaPercentList: IServiceLineEbitdaPercentList[];
}
