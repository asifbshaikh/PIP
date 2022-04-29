import { ProjectPeriod } from '.';
import { ICalulcatedValue } from './ICalculatedValue';
import { IRisk } from './IRisk';

export interface IRiskManagement {
    calculatedValue: ICalulcatedValue;
    riskManagement: IRisk;
    projectPeriod: ProjectPeriod[];
}
