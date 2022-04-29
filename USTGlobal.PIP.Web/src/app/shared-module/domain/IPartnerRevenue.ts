import { IPeriodPartnerRevenue } from './IPeriodPartnerRevenue';

export interface IPartnerRevenue {
    partnerRevenueId: number;
    uId?: number;
    pipSheetId: number;
    milestoneId: number;
    description: string;
    revenueAmount: number;
    isDeleted: boolean;
    setMargin: boolean;
    marginPercent: number;
    partnerCostUId: number;
    partnerRevenuePeriodDetail: IPeriodPartnerRevenue[];
}
