import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Constants } from '@shared/infrastructure/constants';
import { IReimbursementAndSales } from '@shared/domain/IReimbursementAndSales';
import { IReimbursement } from '@shared/domain/IReimbursement';
import { ProjectPeriod } from '@shared';
import { IReimbursementPeriod } from '@shared/domain/IReimbursementPeriod';
import { ISalesDiscount } from '@shared/domain/ISalesDiscount';
import { ISalesDiscountPeriod } from '@shared/domain/ISalesDiscountPeriod';
import { Observable } from 'rxjs';
import { IHeaderInfo } from '@shared/domain/IHeaderInfo';

@Injectable({
  providedIn: 'root'
})
export class ReimbursementSalesService {

  constructor(private httpClient: HttpClient) {
  }

  getReimbursementAndSalesDetails(pipSheetId: any) {
    return this.httpClient.get<IReimbursementAndSales>(Constants.webApis.getReimbursementAndSales.replace('{pipSheetId}', pipSheetId));
  }

  getDefaultReimbursements(reimbursements: IReimbursement[], periods: ProjectPeriod[], pipSheetId: number,
    length: number): IReimbursement[] {

    // adding 2 default rows
    for (let i = 0; i < length; i++) {
      reimbursements.push({
        uId: 0,
        reimbursementId: 0,
        milestoneId: -1,
        description: '',
        createdBy: 1,
        pipSheetId: pipSheetId,
        reimbursedExpense: 0,
        isDeleted: false,
        updatedBy: 1,
        isDirectExpenseReimbursable: false,
        directExpensePercent: 0,
        isDirectExpenseMilestone: false,
        reimbursementPeriods: this.computeReimbursementPeriods(periods)
      });
    }

    return reimbursements;
  }


  private computeReimbursementPeriods(periods: ProjectPeriod[]): IReimbursementPeriod[] {
    const periodWiseReimbursements: IReimbursementPeriod[] = [];

    periods.forEach(period => {
      periodWiseReimbursements.push({
        uId: 0,
        billingPeriodId: period.billingPeriodId,
        reimbursementId: 0,
        expense: 0
      });
    });
    return periodWiseReimbursements;
  }

  getDefaultSalesDiscount(salesDiscounts: ISalesDiscount[], periods: ProjectPeriod[], pipSheetId: number): ISalesDiscount[] {

    // adding 5 default rows.
    for (let i = 0; i < 2; i++) {
      salesDiscounts.push({
        uId: 0,
        salesDiscountId: 0,
        milestoneId: -1,
        description: '',
        createdBy: 1,
        pipSheetId: pipSheetId,
        discount: 0,
        isDeleted: false,
        updatedBy: 1,
        salesDiscountPeriods: this.computeSalesDiscountPeriods(periods)
      });
    }
    return salesDiscounts;
  }


  private computeSalesDiscountPeriods(periods: ProjectPeriod[]): ISalesDiscountPeriod[] {
    const periodWiseDiscount: ISalesDiscountPeriod[] = [];

    periods.forEach(period => {
      periodWiseDiscount.push({
        uId: 0,
        billingPeriodId: period.billingPeriodId,
        discount: 0,
        salesDiscountId: 0
      });
    });
    return periodWiseDiscount;
  }

  addReimbursementrow(pipSheetId: number, periods: ProjectPeriod[]): IReimbursement {
    return {
      uId: 0,
      reimbursementId: 0,
      description: '',
      milestoneId: -1,
      isDeleted: false,
      createdBy: 1,
      updatedBy: 1,
      pipSheetId: pipSheetId,
      reimbursedExpense: 0,
      isDirectExpenseReimbursable: false,
      directExpensePercent: 0,
      isDirectExpenseMilestone: false,
      reimbursementPeriods: this.computeReimbursementPeriods(periods)
    };
  }


  addSalesDiscountRow(pipSheetId: number, periods: ProjectPeriod[]): ISalesDiscount {
    return {
      uId: 0,
      salesDiscountId: 0,
      description: '',
      discount: 0,
      isDeleted: false,
      createdBy: 1,
      milestoneId: -1,
      pipSheetId: pipSheetId,
      updatedBy: 1,
      salesDiscountPeriods: this.computeSalesDiscountPeriods(periods)
    };
  }


  saveReimbursementAndSalesDetails(data: any): Observable<any> {
    return this.httpClient.post(Constants.webApis.saveReimbursementAndSales, data);
  }

  getHeader1Data(projectId: any, pipSheetId: any): Observable<IHeaderInfo> {
    return this.httpClient.get<IHeaderInfo>(Constants.webApis.getHeader1Data.replace('{projectId}', projectId)
      .replace('{pipSheetId}', pipSheetId));
  }

  calculateReimbursedExpense(reimbursementData: IReimbursement[], projectPeriod: ProjectPeriod[]): any {
    const expenseTotals = new Array(reimbursementData.length);
    const periodTotals = new Array(projectPeriod.length);
    periodTotals.fill(0);
    expenseTotals.fill(0);
    reimbursementData.forEach((row, expenseIndex) => {
      row['reimbursementPeriods'].forEach((period, periodIndex) => {
        expenseTotals[expenseIndex] += +period.expense;
        periodTotals[periodIndex] += +period.expense;
      });
    });
    return { expenseTotals, periodTotals };
  }

  calculateDiscount(discountData: ISalesDiscount[], projectPeriod: ProjectPeriod[]): any {
    const discountTotals = new Array(discountData.length);
    const periodTotals = new Array(projectPeriod.length);
    periodTotals.fill(0);
    discountTotals.fill(0);
    discountData.forEach((row, expenseIndex) => {
      row['salesDiscountPeriods'].forEach((period, periodIndex) => {
        discountTotals[expenseIndex] += +period.discount;
        periodTotals[periodIndex] += +period.discount;
      });
    });
    return { discountTotals, periodTotals };
  }
}
