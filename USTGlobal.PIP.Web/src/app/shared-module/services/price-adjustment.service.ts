import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Constants } from '@shared';
import { IPriceAdjustment } from '@shared/domain/IPriceAdjustment';
import { IHeaderInfo } from '@shared/domain/IHeaderInfo';

@Injectable({
    providedIn: 'root'
})

export class PriceAdjustmentService {
    constructor(private httpClient: HttpClient) {
    }

    getPriceAdjustmentData(pipSheetId: number): Observable<IPriceAdjustment> {
        return this.httpClient.get<IPriceAdjustment>(Constants.webApis.getPriceAdjustmentData + pipSheetId);
    }

    savePriceAdjustmentData(priceAdjustmentList: IPriceAdjustment): Observable<number> {
        return this.httpClient.post<number>(Constants.webApis.savePriceAdjustmentData, priceAdjustmentList);
    }

    getHeader1Data(projectId: any, pipSheetId: any): Observable<IHeaderInfo> {
        return this.httpClient.get<IHeaderInfo>(Constants.webApis.getHeader1Data.replace('{projectId}', projectId)
            .replace('{pipSheetId}', pipSheetId));
    }

    getEffectiveDate(endDate: Date, triggerDate: Date): any {
        const projectEndDate: Date = new Date(endDate);
        let effectiveDate: Date;
        const monthDiff = ((projectEndDate.getMonth() - triggerDate.getMonth()) +
            (12 * (projectEndDate.getFullYear() - triggerDate.getFullYear())));
        if (triggerDate <= projectEndDate && monthDiff < 72) {
            effectiveDate = triggerDate;
            if (triggerDate.getDate() !== 1 && triggerDate.getMonth() + 1 === 12) {
                effectiveDate = new Date('01' + '/' + '01' + '/' + (triggerDate.getFullYear() + 1));
            }
            else if (triggerDate.getDate() !== 1) {
                effectiveDate = new Date((triggerDate.getMonth() + 2) + '/' + '01' + '/' + triggerDate.getFullYear());
            }
        }
        return effectiveDate;
    }
}
