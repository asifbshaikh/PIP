import { IPeriodPartnerCost } from './IPeriodPartnerCost';

export interface IPartnerCost {
    partnerCostId: number;
    pipSheetId: number;
    milestoneId: number;
    description: string;
    paidAmount: number;
    isDeleted: boolean;
    setMargin: boolean;
    marginPercent: number;
    partnerCostPeriodDetail: IPeriodPartnerCost[];
}
