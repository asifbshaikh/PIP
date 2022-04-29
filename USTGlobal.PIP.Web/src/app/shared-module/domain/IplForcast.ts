import { IplForecastPeriod } from './IplForecastPeriod';

export interface IplForcast {
    descriptionId: number;
    total: number;
    plForecastLabels?: string;
    plForecastPeriodDTO: IplForecastPeriod[];
}
