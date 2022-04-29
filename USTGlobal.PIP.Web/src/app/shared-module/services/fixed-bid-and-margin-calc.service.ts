import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Constants } from '@shared';
import { IFixBidMarginCalculation } from '@shared/domain/IFixBidMarginCalculation';

@Injectable({
    providedIn: 'root'
})

export class FixedBidAndMarginCalcService {
    constructor(
        private httpClient: HttpClient,
    ) {
    }


    getFixBidData(pipSheetId: number): Observable<IFixBidMarginCalculation> {
        return this.httpClient.get<IFixBidMarginCalculation>(Constants.webApis.getFixedBidData + pipSheetId);
    }
}
