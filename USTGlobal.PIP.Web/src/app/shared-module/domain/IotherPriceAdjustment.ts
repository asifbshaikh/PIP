import { IBase } from './base';
import { IPeriodOtherPriceAdjustment } from './IPeriodOtherPriceAdjustment';
export interface IOtherPriceAdjustment extends IBase {
        uId: number;
        otherPriceAdjustmentId: number;
        pipSheetId: number;
        milestoneId: number;
        description: string;
        totalRevenue: number;
        isDeleted: boolean;
        rowType: number;
        otherPriceAdjustmentPeriodDetail: IPeriodOtherPriceAdjustment[];
}
