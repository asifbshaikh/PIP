import { Component, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators, FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { SummaryService } from '@shared/services/summary.service';
import { Constants } from '@shared';
import { SharedDataService } from '@global';
@Component({
  selector: 'app-summary',
  templateUrl: './summary.component.html'
})
export class SummaryComponent implements OnInit {
@Output() summaryLoad = new EventEmitter();
summaryLoaded = false;
  translationData: any = {};
  isCurrencyConverted = false;
  pipSheetId: number;
  projectSummaryData: any[];
  convertedCurrency: any[] = [];
  vacationAbsenceLabel: string;
  totalQualifyingDiscountsLabel: string;
  currencySymbol: string;

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private translateService: TranslateService,
    private summaryService: SummaryService,
    private sharedData: SharedDataService
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
    });
  }

  ngOnInit() {
    this.translateService.get('ProjectSummary').subscribe((data) => {
      this.translationData = data;
      this.vacationAbsenceLabel = Constants.effortSummary.vacationAbsence;
      this.totalQualifyingDiscountsLabel = Constants.effortSummary.totalQualifyingDiscounts;
      this.projectSummaryData = JSON.parse(JSON.stringify(this.translationData.projectSummaryLabels));
      this.getSummaryData();
      this.currencySymbol =
          this.sharedData.sharedData.currencyDTO.find(sym => sym.currencyId === this.sharedData.sharedData.currencyId).symbol;
    });
  }

  getSummaryData() {
    this.projectSummaryData = [];
    this.projectSummaryData = JSON.parse(JSON.stringify(this.translationData.projectSummaryLabels));
    this.summaryService.getSummaryData(this.pipSheetId).subscribe(result => {
      if (result.length > 1) {
        this.isCurrencyConverted = true;
        this.projectSummaryData.forEach(item => {
          item.value = result[0][item.field];
          this.convertedCurrency.push(item.value);
          item.value = result[1][item.field];
        });
      }
      else {
        this.projectSummaryData.forEach(item => {
          item.value = result[0][item.field];
        });
      }
    });
    this.summaryLoaded = true;
    this.summaryLoad.emit(this.summaryLoaded);
  }
}
