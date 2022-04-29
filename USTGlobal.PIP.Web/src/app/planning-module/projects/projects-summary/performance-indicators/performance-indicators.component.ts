import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { PerformanceIndicatiorsService } from '@shared/services/performance-indicatiors.service';
import { ActivatedRoute } from '@angular/router';
import { IKeyPerformanceIndicators } from '@shared/domain/IKeyPerformanceIndicators';
import { FormGroup, FormBuilder } from '@angular/forms';
import { SharedDataService } from '@global';

@Component({
  selector: 'app-performance-indicators',
  templateUrl: './performance-indicators.component.html'
})
export class PerformanceIndicatorsComponent implements OnInit {
  keyPerformanceIndicatorForm: FormGroup;
  cols: any;
  rows: any;
  pipSheetId: number;
  collapsedRevenue = false;
  collapsedEBITDA = false;
  currencySymbol: string;
  selectedRow: any;
  keyPerformanceIndicatorsData: IKeyPerformanceIndicators;
  constructor(
    private translateService: TranslateService,
    private sharedData: SharedDataService,
    private performanceIndicatiorsService: PerformanceIndicatiorsService,
    private route: ActivatedRoute,
    private fb: FormBuilder,
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
    });
  }

  ngOnInit() {
    this.currencySymbol =
    this.sharedData.sharedData.currencyDTO.find(sym => sym.currencyId === this.sharedData.sharedData.currencyId).symbol;
    this.translateService.get('KeyPerformanceIndicators.PerformanceIndicatorsColumns').subscribe(data => {
      this.cols = data;
    });
    this.translateService.get('KeyPerformanceIndicators.PerformanceIndicatorsRows').subscribe(data => {
      this.rows = data;
    });
    this.getKeyPerformanceIndicatorsData();
  }

  initializeForm() {
    this.keyPerformanceIndicatorForm = this.fb.group({
      costContingencyPercent: [this.keyPerformanceIndicatorsData.costContingencyPercent],
      ebitdaPercent: [this.keyPerformanceIndicatorsData.ebitdaPercent],
      firstMonthPositiveCashFlow: [this.keyPerformanceIndicatorsData.firstMonthPositiveCashFlow],
      grossMarginPercent: [this.keyPerformanceIndicatorsData.grossMarginPercent],
      onShoreFTEPercent: [this.keyPerformanceIndicatorsData.onShoreFTEPercent],
      serviceLineBlendedTargetEbitda: [this.keyPerformanceIndicatorsData.serviceLineBlendedTargetEbitda],
      variancePercent: [this.keyPerformanceIndicatorsData.variancePercent],
      serviceLineEbitdaPercentList: [this.keyPerformanceIndicatorsData.serviceLineEbitdaPercentList],
      serviceLineRevenueList: [this.keyPerformanceIndicatorsData.serviceLineRevenueList]
    });
  }

  getKeyPerformanceIndicatorsData() {
    this.performanceIndicatiorsService.getKeyPerformanceIndicatorsData(this.pipSheetId).subscribe(keyPerformanceIndicatorsData => {
      this.keyPerformanceIndicatorsData = keyPerformanceIndicatorsData;
      this.initializeForm();
    });
  }

  onRevenueRowCollapse() {
    this.collapsedRevenue = !this.collapsedRevenue;
  }

  onEBITDARowCollapse() {
    this.collapsedEBITDA = !this.collapsedEBITDA;
  }
  public highlightRow(rowindex) {
    this.selectedRow = rowindex;
  }
}
