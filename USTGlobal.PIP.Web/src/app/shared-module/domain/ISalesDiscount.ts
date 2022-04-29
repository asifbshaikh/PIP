import { IBase } from '.';
import { ISalesDiscountPeriod } from './ISalesDiscountPeriod';

export interface ISalesDiscount extends IBase {
    uId: number;
    salesDiscountId: number;
    pipSheetId: number;
    milestoneId: number;
    description: string;
    discount: number;
    isDeleted: boolean;
    salesDiscountPeriods: ISalesDiscountPeriod[];
}
