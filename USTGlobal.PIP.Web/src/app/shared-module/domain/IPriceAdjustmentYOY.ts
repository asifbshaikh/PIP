import { IBase } from './base';
export interface IPriceAdjustmentYoy extends IBase {

    pipSheetId: number;
    triggerDate: String;
    effectiveDate: String;
}
