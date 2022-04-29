import { IBase } from './base';

export interface IEbitda extends IBase {

    locationId: number;
    pipSheetId: number;
    locationName: string;
    refUSD: number;
    ebitdaSeatCost: number;
    overheadAmount: number;
    sharedSeatsUsePercent: number;
    isStdOverheadOverriden: boolean;
    createdOn?: string;
    updatedOn?: string;
}
