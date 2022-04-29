import { IBase } from './base';

export interface ICapitalCharge extends IBase {
    capitalChargeId: number;
    pipSheetId: number;
    paymentLag: number;
    capitalChargeDailyRate: number;
    totalCostBeforeCap: number;
    capitalCharge: number;
    isTargetMarginPrice: boolean;
}
