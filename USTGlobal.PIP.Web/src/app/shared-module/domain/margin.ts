import { IBase } from './base';

export interface IMargin extends IBase {
    marginId: number;
    pipSheetId: number;
    isMarginSet: number;
    marginPercent: number;
    which: number;
}
