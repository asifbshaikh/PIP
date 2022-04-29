import { ITotalClientPrice, ProjectPeriod } from '@shared';
import { IcashFlowParentDTO } from './IcashFlowParentDTO';
export interface IBillingSchedule {
    pipSheetId: number;
    blendedLaborCostPerHr: number;
    blendedBillRate: number;
    projectPeriodDTO: ProjectPeriod[];
    clientPriceDTO: ITotalClientPrice[];
    cashFlowParentDTO: IcashFlowParentDTO[];
}
