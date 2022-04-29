import { IBase } from './base';
import { IPeriodLostRevenue } from './IPeriodLostRevenue';

export interface IVacationAbsence extends IBase {
    pipSheetId: number;
    lostRevenue: number;
    totalRevenue: number;
    totalLostRevenue: number;
    isPercent: boolean;
    amount: number;
    isOverride: boolean;
    isMarginSet: Boolean;
    periodLostRevenue: IPeriodLostRevenue[];
}
