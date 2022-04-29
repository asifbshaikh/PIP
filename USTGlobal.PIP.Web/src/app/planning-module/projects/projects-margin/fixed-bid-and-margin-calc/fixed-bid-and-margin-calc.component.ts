import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormArray } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { ResourceMapper, ProjectPeriod, IFixBidMarginCalculation, FixedBidMarginLabels, MarginType } from '@shared';
import { DateService } from '@core';
import { ActivatedRoute } from '@angular/router';
import { FixedBidAndMarginCalcService } from '@shared/services/fixed-bid-and-margin-calc.service';

@Component({
  selector: 'fixed-bid-and-margin-calc',
  templateUrl: './fixed-bid-and-margin-calc.component.html'
})
export class FixedBidAndMarginCalcComponent implements OnInit {
  fixedBidAndMarginCalculationData: IFixBidMarginCalculation = {
    fixBidMarginDTO: [],
    marginDTO: {
      isMarginSet: false,
      which: 0,
      marginPercent: 60
    },
    projectPeriodDTO: [],
    marginBeforeAdjustment: 0
  };
  pipSheetId: number;
  fixedBidCols: any[] = [];
  periodCols: any[] = [];
  fixedBidMarginLabels = FixedBidMarginLabels;
  translationData: any = {};
  marginType = MarginType;
  isDataAvailable = false;

  constructor(
    private route: ActivatedRoute,
    private translateService: TranslateService,
    private dateService: DateService,
    private fixedBidAndMarginCalculationService: FixedBidAndMarginCalcService
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
    });
  }

  ngOnInit() {
    this.translateService.get('FixedBidAndMarginCalc').subscribe(data => {
      this.translationData = data;
      this.fixedBidCols = data.FixedBidColumns;
    });

    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      this.fixedBidAndMarginCalculationService.getFixBidData(this.pipSheetId)
        .subscribe(data => {
          this.getPeriods(data.projectPeriodDTO);
          this.fixedBidAndMarginCalculationData = data;
          this.isDataAvailable = true;
        });
    }
    else {
      this.isDataAvailable = true;
    }
  }

  private getPeriods(periods: ProjectPeriod[]) {
    this.periodCols = new ResourceMapper(this.dateService).computeDyanmicColumns(periods);
  }
}
