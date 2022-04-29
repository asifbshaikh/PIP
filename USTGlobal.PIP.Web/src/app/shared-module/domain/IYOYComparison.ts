import { IYOYComparisonPeriod } from '@shared';

export interface IYOYComparison {
    descriptionId: number;
    total: number;
    summaryYoyPeriodList: IYOYComparisonPeriod[];
}
