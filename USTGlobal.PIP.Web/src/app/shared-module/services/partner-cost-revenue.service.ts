import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Constants } from '@shared/infrastructure';
import { SharedDataService } from '@global';
import { IPartnerCost, ProjectPeriod, IPartnerRevenue } from '@shared/domain';
import { IPeriodPartnerRevenue } from '@shared/domain/IPeriodPartnerRevenue';
import { IPeriodPartnerCost } from '@shared/domain/IPeriodPartnerCost';
import { IHeaderInfo } from '@shared/domain/IHeaderInfo';

@Injectable({
    providedIn: 'root'
})

export class PartnerCostRevenueService {
    constructor(private httpClient: HttpClient, private sharedDataService: SharedDataService) {
    }

    getPartnerCostRevenue(pipSheetId: any): Observable<any> {
        return this.httpClient.get(Constants.webApis.getPartnerCostRevenue.replace('{pipSheetId}', pipSheetId));
    }

    getHeader1Data(projectId: any, pipSheetId: any): Observable<IHeaderInfo> {
        return this.httpClient.get<IHeaderInfo>(Constants.webApis.getHeader1Data.replace('{projectId}', projectId)
            .replace('{pipSheetId}', pipSheetId));
    }

    getDefaultPartnerCost(periods: ProjectPeriod[], pipSheetId: number): IPartnerCost[] {
        const partnerCost: IPartnerCost[] = [];
        for (let i = 0; i < 2; i++) {
            partnerCost.push({
                partnerCostId: 0,
                milestoneId: -1,
                description: '',
                paidAmount: 0,
                pipSheetId: pipSheetId,
                isDeleted: false,
                setMargin: false,
                marginPercent: null,
                partnerCostPeriodDetail: this.computePartnerCostPeriods(periods)
            });
        }
        return partnerCost;
    }

    addPartnerCostRow(pipSheetId: number, periods: ProjectPeriod[]): IPartnerCost {
        return {
            partnerCostId: 0,
            milestoneId: -1,
            description: '',
            paidAmount: 0,
            pipSheetId: pipSheetId,
            isDeleted: false,
            setMargin: false,
            marginPercent: null,
            partnerCostPeriodDetail: this.computePartnerCostPeriods(periods)
        };
    }

    addPartnerRevenueRow(pipSheetId: number, periods: ProjectPeriod[]): IPartnerRevenue {
        return {
            partnerRevenueId: 0,
            milestoneId: -1,
            description: '',
            revenueAmount: 0,
            pipSheetId: pipSheetId,
            isDeleted: false,
            partnerCostUId: null,
            setMargin: false,
            marginPercent: null,
            partnerRevenuePeriodDetail: this.computePartnerRevenuePeriods(periods)
        };
    }

    private computePartnerCostPeriods(periods: ProjectPeriod[]): IPeriodPartnerCost[] {
        const periodWiseCost: IPeriodPartnerCost[] = [];
        periods.forEach(period => {
            periodWiseCost.push({
                partnerCostId: 0,
                billingPeriodId: period.billingPeriodId,
                cost: 0
            });
        });
        return periodWiseCost;
    }

    getDefaultPartnerRevenue(periods: ProjectPeriod[], pipSheetId: number): IPartnerRevenue[] {
        const partnerRevenue: IPartnerRevenue[] = [];
        for (let i = 0; i < 2; i++) {
            partnerRevenue.push({
                partnerRevenueId: 0,
                milestoneId: -1,
                description: '',
                revenueAmount: 0,
                pipSheetId: pipSheetId,
                isDeleted: false,
                setMargin: false,
                partnerCostUId: null,
                marginPercent: null,
                partnerRevenuePeriodDetail: this.computePartnerRevenuePeriods(periods)
            });
        }
        return partnerRevenue;
    }
    private computePartnerRevenuePeriods(periods: ProjectPeriod[]): IPeriodPartnerRevenue[] {
        const periodWiseCost: IPeriodPartnerRevenue[] = [];
        periods.forEach(period => {
            periodWiseCost.push({
                partnerRevenueId: 0,
                billingPeriodId: period.billingPeriodId,
                revenue: 0
            });
        });
        return periodWiseCost;
    }

    savePartnerCostRevenue(data: any[]): Observable<any> {
        return this.httpClient.post(Constants.webApis.savePartnerCostRevenue, data);
    }

    costCalculations(partnerCostData: IPartnerCost[], projectPeriod: ProjectPeriod[]) {
        const amountPaidTotals = new Array(partnerCostData.length);
        const periodTotals = new Array(projectPeriod.length);
        periodTotals.fill(0);
        amountPaidTotals.fill(0);
        partnerCostData.forEach((costData, costIndex) => {
            costData.partnerCostPeriodDetail.forEach((periodCost, periodIndex) => {
                amountPaidTotals[costIndex] += +periodCost.cost;
                periodTotals[periodIndex] += +periodCost.cost;
            });
        });
        return { amountPaidTotals, periodTotals };
    }

    revenueCalculations(partnerRevenueData: IPartnerRevenue[], projectPeriod: ProjectPeriod[]) {
        const revenueAmountTotals = new Array(partnerRevenueData.length);
        const periodTotals = new Array(projectPeriod.length);
        periodTotals.fill(0);
        revenueAmountTotals.fill(0);
        partnerRevenueData.forEach((revenueData, costIndex) => {
            revenueData.partnerRevenuePeriodDetail.forEach((periodRevenue, periodIndex) => {
                revenueAmountTotals[costIndex] += +periodRevenue.revenue;
                periodTotals[periodIndex] += +periodRevenue.revenue;
            });
        });
        return { revenueAmountTotals, periodTotals };
    }
}
