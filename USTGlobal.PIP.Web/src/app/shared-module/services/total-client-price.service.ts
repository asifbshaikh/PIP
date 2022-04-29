import { IHeaderInfo } from '@shared/domain/IHeaderInfo';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Constants, ProjectPeriod, ITotalClientPrice, ITotalClientPricePeriods } from '@shared';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TotalClientPriceService {
  constructor(private httpClient: HttpClient) { }

  getTotalClientPrice(pipSheetId: any): Observable<any> {
    return this.httpClient.get(Constants.webApis.getTotalClientPrice.replace('{pipSheetId}', pipSheetId));
  }

  getDefaultTotalClientPrice(periods: ProjectPeriod[], pipSheetId: number, totalClientPrice: ITotalClientPrice[]): ITotalClientPrice[] {
    // const totalClientPrice: ITotalClientPrice[] = [];
    for (let i = 1; i < 6; i++) {
      totalClientPrice.push({
        uId: 0,
        clientPriceId: 0,
        pipSheetId: pipSheetId,
        descriptionId: 0,
        totalPrice: 0,
        clientPricePeriodDTO: this.computeTotalClientPricePeriods(periods),
      });
    }
    return totalClientPrice;
  }

  private computeTotalClientPricePeriods(periods: ProjectPeriod[]): ITotalClientPricePeriods[] {
    const periodWiseRevenue: ITotalClientPricePeriods[] = [];
    periods.forEach(period => {
      periodWiseRevenue.push({
        uId: 0,
        clientPriceId: 0,
        billingPeriodId: period.billingPeriodId,
        price: 0
      });
    });
    return periodWiseRevenue;
  }

  saveTotalClientPrice(data: any): Observable<any> {
    return this.httpClient.post(Constants.webApis.saveTotalClientPrice, data);
  }

  priceCalculations(clientPriceData: ITotalClientPrice[]) {
    const totalPrice = new Array(clientPriceData.length);
    totalPrice.fill(0);
    clientPriceData.forEach((costData, costIndex) => {
      costData.clientPricePeriodDTO.forEach((periodCost) => {
        totalPrice[costIndex] += +periodCost.price;
      });
    });
    return { totalPrice: totalPrice };
  }

  // Calculate  Total Invoiced Periods
  computeTotalInvoicedPeriods(client, index) {
    const totalClientPricePeriods = client[0].value.clientPricePeriodDTO;
    const invoicePlanPeriods = client[1].value.clientPricePeriodDTO;
    if (client[1].value.totalPrice > 0) {
      return (invoicePlanPeriods[index].price);
    } else {
      return (totalClientPricePeriods[index].price);
    }
  }

  // Calculate Project Cost
  computeProjectCost(plForcast, index: number) {
    // Project Cost = Cost of Staffing + Inflation Only + Expenses and Overhead + Contingency Cost + Partner Cost + Capital Charges
    const costOfStaffing = plForcast[0].value.plForecastPeriodDTO;
    const inflationRate = plForcast[1].value.plForecastPeriodDTO;
    const expenseOH = plForcast[2].value.plForecastPeriodDTO;
    const costContingency = plForcast[3].value.plForecastPeriodDTO;
    const partnerCost = plForcast[4].value.plForecastPeriodDTO;
    const capitalCharges = plForcast[5].value.plForecastPeriodDTO;
    return (costOfStaffing[index].price + (inflationRate[index].price) + (expenseOH[index].price)
      + (costContingency[index].price) + (partnerCost[index].price) + (capitalCharges[index].price));
  }

  // Calculate Net Cash Flow
  computeNetCashFlow(client, index: number) {
    const periodWiseTotalInvoice = client[2].value.clientPricePeriodDTO;
    const periodWiseProjectCost = client[3].value.clientPricePeriodDTO;
    return ((periodWiseTotalInvoice[index].price) - (periodWiseProjectCost[index].price));
  }

  // Calculate Cumulative Cash Flow
  computeCumulativeCashFlow(client, index: number, periodWiseCumulativeCashFlow) {
    const periodWiseNetCashFlow = client[4].value.clientPricePeriodDTO;
    const totalPrice = client[5].value.totalPrice;
    if (index === 0) {
      return ((totalPrice) + (periodWiseNetCashFlow[index].price));
    }
    else {
      return (periodWiseCumulativeCashFlow.value[index - 1].price +
        periodWiseNetCashFlow[index].price);
    }
  }

  getHeader1Data(projectId: any, pipSheetId: any): Observable<IHeaderInfo> {
    return this.httpClient.get<IHeaderInfo>(Constants.webApis.getHeader1Data.replace('{projectId}', projectId)
      .replace('{pipSheetId}', pipSheetId));
  }
}
