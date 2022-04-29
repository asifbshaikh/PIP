import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Constants } from '@shared/infrastructure';
import { ExpensesAndAssetsMapper } from '@shared/mapper/master/expenseandassetmapper';
import { SharedDataService } from '@global';
import { IAsset } from '@shared/domain/asset';
import { IDirectExpense } from '@shared/domain/directexpense';
import { IDirectExpensesPeriod } from '@shared/domain/directexpensesperiod';
import { ProjectPeriod } from '@shared/domain/projectperiod';
import { IHeaderInfo } from '@shared/domain/IHeaderInfo';
import { map } from 'rxjs/internal/operators';

@Injectable({
  providedIn: 'root'
})
export class DirectexpensesService {

  constructor(private httpClient: HttpClient, private sharedDataService: SharedDataService) { }

  getDirectExpensesAndAssetsDetails(pipSheetId: any): Observable<ExpensesAndAssetsMapper> {
    return this.httpClient.get(Constants.webApis.getExpenseAndAsset.replace('{pipSheetId}', pipSheetId))
      .pipe(map(data => new ExpensesAndAssetsMapper().mapper(data)));
  }

  saveDirectExpenses(data: any): Observable<any> {
    return this.httpClient.post(Constants.webApis.saveExpenseAndAsset, data);
  }

  getHeader1Data(projectId: any, pipSheetId: any): Observable<IHeaderInfo> {
    return this.httpClient.get<IHeaderInfo>(Constants.webApis.getHeader1Data.replace('{projectId}', projectId)
      .replace('{pipSheetId}', pipSheetId));
  }

  getDefaultBasicAssets(pipSheetId: number, savedAssetDTO?: IAsset[]): IAsset[] {
    const basicAssets = this.sharedDataService.sharedData.basicAssetDTO;
    const assetDTO: IAsset[] = [];

    // for basic assets
    basicAssets.forEach(asset => {
      const savedBasicAsset = savedAssetDTO ? savedAssetDTO.find(sad => sad.basicAssetId === asset.basicAssetId) : null;
      assetDTO.push({
        pipSheetId: pipSheetId,
        basicAssetId: asset.basicAssetId,
        amount: savedBasicAsset ? savedBasicAsset.count * savedBasicAsset.amount : 0,
        costToProject: asset.costToProject,
        count: savedBasicAsset ? savedBasicAsset.count : 0,
        description: asset.description,
        projectAssetId: savedBasicAsset ? savedBasicAsset.projectAssetId : 0,
        createdBy: 1,
        createdOn: null,
        isDeleted: false,
        updatedBy: 1,
        updatedOn: null
      });
    });

    // for additional hardware / software
    let savedHwAsset: IAsset[];
    savedHwAsset = savedAssetDTO ? savedAssetDTO.filter(sad => sad.basicAssetId === null) : null;

    if (savedHwAsset) {
      if (savedHwAsset.length === 1) {
        assetDTO.push(this.addSavedAdditionalHWObject(pipSheetId, savedHwAsset[0]));
        this.addEmptyAdditionalHWObject(pipSheetId);
      }
      else if (savedHwAsset.length >= 2) {
        for (let i = 0; i < savedHwAsset.length; i++) {
          assetDTO.push(this.addSavedAdditionalHWObject(pipSheetId, savedHwAsset[i]));
        }
      }
    }
    else {
      for (let i = 0; i < 2; i++) {
        assetDTO.push(this.addEmptyAdditionalHWObject(pipSheetId));
      }
    }
    return assetDTO;
  }

  addEmptyAdditionalHWObject(pipSheetId: number): IAsset {
    return {
      pipSheetId: pipSheetId,
      basicAssetId: null,
      amount: 0,
      costToProject: 0,
      count: 0,
      description: '',
      projectAssetId: 0,
      createdBy: 1,
      createdOn: null,
      isDeleted: false,
      updatedBy: 1,
      updatedOn: null
    };
  }

  addSavedAdditionalHWObject(pipSheetId: number, savedHwAsset: IAsset) {
    return {
      pipSheetId: pipSheetId,
      basicAssetId: null,
      amount: savedHwAsset.amount,
      costToProject: savedHwAsset.costToProject,
      count: savedHwAsset.count,
      description: savedHwAsset.description,
      projectAssetId: savedHwAsset.projectAssetId,
      createdBy: 1,
      createdOn: null,
      isDeleted: false,
      updatedBy: 1,
      updatedOn: null
    };
  }

  getDefaultDirectExpenses(directExpenses: IDirectExpense[], periods: ProjectPeriod[], pipSheetId: number): IDirectExpense[] {
    const defaultLabels = this.sharedDataService.sharedData.defaultLabelDTO;
    defaultLabels.forEach((label, index) => {
      directExpenses.push({
        uId: 0,
        directExpenseId: 0,
        label: label.name,
        milestoneId: -1,
        description: '',
        createdBy: 1,
        pipSheetId: pipSheetId,
        totalExpense: 0,
        isReimbursable: false,
        percentRevenue: 0,
        isDeleted: false,
        updatedBy: 1,
        directExpensePeriodDTO: this.computeDirectExpensePeriods(periods)
      });
    });

    return directExpenses;
  }

  addDirectExpenseRow(pipSheetId: number, periods: ProjectPeriod[]): IDirectExpense {
    return {
      uId: 0,
      directExpenseId: 0,
      label: '',
      milestoneId: -1,
      description: '',
      createdBy: 1,
      pipSheetId: pipSheetId,
      isDeleted: false,
      totalExpense: 0,
      isReimbursable: false,
      percentRevenue: 0,
      updatedBy: 1,
      directExpensePeriodDTO: this.computeDirectExpensePeriods(periods)
    };
  }

  addAdditionalAssetRow(pipSheetId: number): IAsset {
    return {
      pipSheetId: pipSheetId,
      basicAssetId: null,
      amount: 0,
      costToProject: 0,
      count: 0,
      description: '',
      projectAssetId: 0,
      createdBy: 1,
      createdOn: null,
      updatedBy: 1,
      updatedOn: null,
      isDeleted: false
    };

  }

  private computeDirectExpensePeriods(periods: ProjectPeriod[]): IDirectExpensesPeriod[] {
    const periodWiseExpense: IDirectExpensesPeriod[] = [];

    periods.forEach(period => {
      periodWiseExpense.push({
        uId: 0,
        billingPeriodId: period.billingPeriodId,
        directExpenseId: 0,
        directExpensePeriodDetailId: 0,
        expense: 0
      });
    });
    return periodWiseExpense;
  }

  calculateAssetAmount(asset: IAsset): number {
    return asset.count * asset.costToProject;
  }

  calculateTotalAssetCost(allAssets: IAsset[]): number {
    let totalAssetCost = 0;
    allAssets.forEach(asset => {
      totalAssetCost += parseFloat(asset.amount.toString());
    });
    return totalAssetCost;
  }

  calculateTotalExpense(allExpenses: IDirectExpense): number {
    let totalExpense = 0;
    allExpenses.directExpensePeriodDTO.forEach(period => {
      totalExpense += + period.expense;
    });
    return totalExpense;
  }

  calculatePerMonthAssetCost(allExpenses: IDirectExpense[], totalAssetCost: number, assetMaxDayCharge: number,
    currencyFactor: number): number[] {
    const noOfPeriods = allExpenses[0].directExpensePeriodDTO.length;
    const perMonthAssetCost: number[] = [];

    if (totalAssetCost > (assetMaxDayCharge * currencyFactor)) {
      const assetCostPerPeriod = totalAssetCost / noOfPeriods;

      for (let i = 0; i < noOfPeriods; i++) {
        perMonthAssetCost[i] = assetCostPerPeriod;
      }
    }
    else {
      for (let i = 0; i < noOfPeriods; i++) {
        perMonthAssetCost[i] = 0;
      }
      perMonthAssetCost[0] = totalAssetCost;
    }

    return perMonthAssetCost;
  }

  calculatePerMonthAssetAndExpense(allExpenses: IDirectExpense[], monthlyAssetCost: number[]): number[] {
    const perMonthAssetAndExpense = new Array(allExpenses[0].directExpensePeriodDTO.length);
    perMonthAssetAndExpense.fill(0);

    // First row intentionally ignored. Only Overhead amount to be considered in final period totals
    // allExpenses.shift();

    allExpenses.forEach((expenseData, expenseNumber) => {
      if (expenseNumber > 0) {
        expenseData.directExpensePeriodDTO.forEach((periodData, periodNo) => {
          perMonthAssetAndExpense[periodNo] += + periodData.expense;
        });
      }
    });

    perMonthAssetAndExpense.forEach((periodData, periodNo) => {
      perMonthAssetAndExpense[periodNo] += + monthlyAssetCost[periodNo];
    });

    return perMonthAssetAndExpense;
  }

  calculateTotalAssetAndExpense(perMonthAssetAndExpense: number[]): number {
    let totalAssetAndExpense = 0;

    perMonthAssetAndExpense.forEach(monthlyTotal => {
      totalAssetAndExpense += + monthlyTotal;
    });

    return totalAssetAndExpense;
  }

}
