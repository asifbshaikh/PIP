import { ICapitalCharge } from './ICapitalCharge';
import { IProjectPeriodClientPrice } from './IProjectPeriodClientPrice';

export interface ICapitalChargeMain {
    capitalChargeDTO: ICapitalCharge;
    projectPeriodTotalDTO: IProjectPeriodClientPrice[];
}
