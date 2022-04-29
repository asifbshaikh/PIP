import { IplForecastPeriod } from './IplForecastPeriod';
import { IplForcast } from './IplForcast';
import { ProjectPeriod } from './projectperiod';

export interface IplForecastSummary {
    plForecastDTO: IplForcast[];
    projectPeriodDTO: ProjectPeriod[];
}
