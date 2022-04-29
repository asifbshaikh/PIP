import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { SummaryService } from '@shared/services/summary.service';
import { SharedDataService } from '@global';

@Component({
  selector: 'app-gpm-omitting',
  templateUrl: './gpm-omitting.component.html'
})
export class GpmOmittingComponent implements OnInit {
  @Output() gpmLoad = new EventEmitter();
  gpmLoaded = false;
  translationData: any = {};
  isCurrencyConverted = false;
  pipSheetId: number;
  grossProfitData: any[];
  convertedCurrency: any[] = [];
  currencySymbol: string;
  constructor(
    private route: ActivatedRoute,
    private sharedData: SharedDataService,
    private translateService: TranslateService,
    private summaryService: SummaryService
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
    });
  }

  ngOnInit() {
    this.translateService.get('ProjectSummary').subscribe((data) => {
      this.translationData = data;
      this.grossProfitData = JSON.parse(JSON.stringify(data.gpmMonitoringLabels));
      this.getGrossProfitData();
      this.currencySymbol =
        this.sharedData.sharedData.currencyDTO.find(sym => sym.currencyId === this.sharedData.sharedData.currencyId).symbol;
    });
  }

  getGrossProfitData() {
    this.grossProfitData = [];
    this.grossProfitData = JSON.parse(JSON.stringify(this.translationData.gpmMonitoringLabels));
    this.summaryService.getGrossProfitData(this.pipSheetId).subscribe(result => {
      if (result.length > 1) {
        this.isCurrencyConverted = true;
        this.grossProfitData.forEach(item => {
          item.value = result[0][item.field];
          this.convertedCurrency.push(item.value);
        });
        this.grossProfitData.forEach(item => {
          item.value = result[1][item.field];
        });
      }
      else {
        if (result) {
          this.grossProfitData.forEach(item => {
            item.value = result[0][item.field];
          });
        }
      }
    });
    this.gpmLoaded = true;
    this.gpmLoad.emit(this.gpmLoaded);
  }
}
