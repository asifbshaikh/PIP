import { ProjectPeriod } from './projectperiod';
import { ITotalClientPrice } from './ITotalClientPrice';
import { IplForcast } from './IplForcast';

export interface TotalClientPrice {
    projectPeriodDTO: ProjectPeriod[];
    clientPriceDTO: ITotalClientPrice[];
    plForecastDTO: IplForcast[];
    feesAtRisk: number;
    netEstimatedRevenue: number;
    currencyId: number;
}
