import { IBase } from './base';

export interface IPeriodLabourRevenue extends IBase {
  billingPeriodId: number;
  pipSheetId: number;
  revenue: number;
}
