import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators, FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { SummaryService } from '@shared/services/summary.service';
import { Constants, IEffortData } from '@shared';

@Component({
  selector: 'app-effort-summary',
  templateUrl: './effort-summary.component.html'
})
export class EffortSummaryComponent implements OnInit {
  @Output() effortSummaryLoad = new EventEmitter();
  summaryLoaded = false;
  translationData: any = {};
  isCurrencyConverted = false;
  effortSummaryData: any;
  pipSheetId: number;

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private translateService: TranslateService,
    private summaryService: SummaryService) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
    });
  }

  ngOnInit() {
    this.translateService.get('ProjectSummary').subscribe((data) => {
      this.translationData = data;
      this.getEffortSummary();
      this.bindEffortSummaryData();
    });
  }

  getEffortSummary() {
    this.effortSummaryData = [];
    this.effortSummaryData = JSON.parse(JSON.stringify(this.translationData.effortSummaryLabels));
  }

  bindEffortSummaryData() {
    this.summaryService.getEffortSummaryData(this.pipSheetId).subscribe((effortData: IEffortData) => {
      this.effortSummaryData.map((summary, key) => {
        if (summary.field.toUpperCase() === Constants.effortSummary.projectduration) {
          summary.value = effortData[Constants.effortSummary.months] + ' ' + this.translationData.mos
            + ' ( ' + parseFloat(effortData[Constants.effortSummary.weeks]).toFixed(1) + ' ' + this.translationData.wks + ' ) ';
        } else {
          summary.value = effortData[summary.field];
        }
      });
    });
    this.summaryLoaded = true;
    this.effortSummaryLoad.emit(this.summaryLoaded);
  }
}
