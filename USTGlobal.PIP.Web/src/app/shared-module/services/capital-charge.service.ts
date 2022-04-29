import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Constants } from '@shared/infrastructure';
import { SharedDataService } from '@global';
import { ICapitalCharge } from '@shared/domain/ICapitalCharge';
import { ICapitalChargeMain } from '@shared/domain/ICapitalChargeMain';
import { IProjectPeriodClientPrice } from '@shared/domain';
import { IHeaderInfo } from '@shared/domain/IHeaderInfo';

@Injectable({
    providedIn: 'root'
})

export class CapitalChargeService {
    constructor(private httpClient: HttpClient, private sharedDataService: SharedDataService) {
    }

    getCapitalChargeData(pipSheetId: any): Observable<ICapitalChargeMain> {
        return this.httpClient.get<ICapitalChargeMain>(Constants.webApis.getCapitalCharge.replace('{pipSheetId}', pipSheetId));
    }

    calculations(formData: any, capitalChargeData: ICapitalChargeMain): any {
        let totalCapitalCharge = 0, periodCapitalCharge = 0, netEstimatedRevenue = 0, totalPaymentLag = 0;
        if (capitalChargeData.projectPeriodTotalDTO.length > 0) {
            // calculate month wise capital charge and total capital charge
            formData.paymentLag > 30 ? totalPaymentLag = (+formData.paymentLag - 30) *
                (capitalChargeData.capitalChargeDTO.capitalChargeDailyRate / 100) : totalPaymentLag = 0;
            capitalChargeData.projectPeriodTotalDTO.forEach(period => {
                netEstimatedRevenue = period.clientPrice - period.feesAtRisk;
                if (capitalChargeData.capitalChargeDTO.isTargetMarginPrice) {
                    periodCapitalCharge = totalPaymentLag * period.clientPrice;
                } else {
                    periodCapitalCharge = totalPaymentLag * netEstimatedRevenue;
                }
                period.capitalCharge = periodCapitalCharge;
                period.netEstimatedRevenue = netEstimatedRevenue;
                totalCapitalCharge += periodCapitalCharge;
            });
        } else {
            capitalChargeData.projectPeriodTotalDTO.forEach(period => {
                period.capitalCharge = 0;
                period.netEstimatedRevenue = 0;
            });
        }
        return { totalCapitalCharge, calculatedPeriodData: capitalChargeData.projectPeriodTotalDTO };
    }

    saveCapitalCharge(data: any): Observable<any> {
        return this.httpClient.post(Constants.webApis.saveCapitalCharge, data);
    }

    getHeader1Data(projectId: any, pipSheetId: any): Observable<IHeaderInfo> {
        return this.httpClient.get<IHeaderInfo>(Constants.webApis.getHeader1Data.replace('{projectId}', projectId)
            .replace('{pipSheetId}', pipSheetId));
    }
}
