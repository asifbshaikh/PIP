import { Component, OnInit } from '@angular/core';
import { SharedDataService } from '@global';
import { TranslateService } from '@ngx-translate/core';
import { SummaryService } from '@shared/services/summary.service';
import { ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { ResourceMapper } from '@shared';
import { DateService } from '@core';
import { ITotalDealFinancials } from '@shared/domain/ITotalDealFinancails';
import { ITotalDealFinancialsYearList } from '@shared/domain/ITotalDealFinancialsYearList';

@Component({
  selector: 'app-total-deal-financials',
  templateUrl: './total-deal-financials.component.html',
})
export class TotalDealFinancialsComponent implements OnInit {
  totalDealFinancialsForm: FormGroup;
  currencySymbol: string;
  projectPeriod: ITotalDealFinancialsYearList[];
  periodCols: any = [];
  pipSheetId: number;
  translationData: [];
  totalDealFinancialsData: ITotalDealFinancials[];
  collapsed = false;
  collapsible: number;
  totalClientPrice = false;
  netRevenue = false;
  totalProjectCost = false;
  selectedRow: any;
  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private sharedData: SharedDataService,
    private translateService: TranslateService,
    private summaryService: SummaryService,
    private dateService: DateService,
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
    });
  }

  ngOnInit() {
    this.currencySymbol =
      this.sharedData.sharedData.currencyDTO.find(sym => sym.currencyId === this.sharedData.sharedData.currencyId).symbol;
    this.initializeForm();
  }

  initializeForm() {
    this.totalDealFinancialsForm = this.fb.group({
      totalDealFinancialDTO: this.fb.array([])
    });
    this.getTotalDealFinancialsData();
  }
  onExpandCollapse(index) {
    switch (!index.undefined) {
      case index === 0:
        this.totalClientPrice = !this.totalClientPrice;
        break;
      case index === 5:
        this.netRevenue = !this.netRevenue;
        break;
      case index === 9:
        this.totalProjectCost = !this.totalProjectCost;
        break;
    }
  }
  public highlightRow(rowindex) {
    this.selectedRow = rowindex;
  }
  getTotalDealFinancialsData() {
    this.summaryService.getTotalDealFinancialData(this.pipSheetId).subscribe(totalFinancialData => {
      this.getPeriods(totalFinancialData[0].totalDealFinancialsYearList);
      this.translateService.get('ProjectSummary').subscribe((label) => {
        totalFinancialData.forEach((totalDealRow, index) => {
          this.translationData = label.totalDealFinancialsLabels[index].label;
          totalDealRow.totalFinLabel = this.translationData;
        });
        this.totalDealFinancialsData = totalFinancialData;
      });
      this.bindFormData();
    });
  }

  bindFormData() {
    this.totalDealFinancialsData.forEach((price) => {
      this.totalDealFinFrom.push(this.totalDealFinData(price));
    });
  }
  get totalDealFinFrom() {
    return this.totalDealFinancialsForm.get('totalDealFinancialDTO') as FormArray;
  }

  private totalDealFinData(totalDealFin: ITotalDealFinancials): FormGroup {
    const totalDealForm = this.fb.group({
      descriptionId: [totalDealFin.descriptionId],
      pipSheetId: this.pipSheetId,
      totalUSD: [totalDealFin.totalUSD],
      totalLocal: [totalDealFin.totalLocal],
      totalFinLabel: [totalDealFin.totalFinLabel],
      totalDealFinancialsYearList: this.totalDealFinancialsPeriodData(totalDealFin.totalDealFinancialsYearList),
    });
    return totalDealForm;
  }
  private totalDealFinancialsPeriodData(periodPrice: ITotalDealFinancialsYearList[]): FormArray {
    const totalDealData = this.fb.array([]);

    periodPrice.forEach((period) => {
      totalDealData.push(this.formulateTotalDealFinPeriodForm(period));
    });

    return totalDealData;
  }

  private formulateTotalDealFinPeriodForm(period: ITotalDealFinancialsYearList) {
    const periodForm = this.fb.group({
      yearId: [period.yearId],
      year: [period.year],
      amount: [period.amount]
    });
    return periodForm;
  }

  private getPeriods(yearPeriods: ITotalDealFinancialsYearList[]) {
    this.projectPeriod = yearPeriods;
    if (yearPeriods.length > 0) {
      this.periodCols = new ResourceMapper(null).computeDyanmicYearColumns(yearPeriods);
    }
  }
}
