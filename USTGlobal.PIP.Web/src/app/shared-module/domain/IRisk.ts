import { IRiskManagementPeriod } from './IRiskManagementPeriod';

export interface IRisk {
    riskManagementId: number;
    pipSheetId: number;
    isContingencyPercent: boolean;
    costContingencyRisk: number;
    feesAtRisk: number;
    isFixedBid: boolean;
    fixBidRiskAmount: number;
    totalAssesedRiskOverrun: number;
    isOverride: boolean;
    riskManagementPeriodDetail: IRiskManagementPeriod[];
    projectDeliveryTypeID: number;
    isMarginSet: boolean;
    costContingencyPercent: number;
    riskCostSubTotal: number;
}

