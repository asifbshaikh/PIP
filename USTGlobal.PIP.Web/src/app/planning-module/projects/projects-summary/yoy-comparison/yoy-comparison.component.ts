import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { SummaryService } from '@shared/services/summary.service';
import { ActivatedRoute } from '@angular/router';
import { DateService } from '@core';
import { IYOYComparison, IYOYComparisonSummary, IYOYComparisonPeriod } from '@shared';
import { ResourceMapper } from '@shared';

@Component({
  selector: 'app-yoy-comparison',
  templateUrl: './yoy-comparison.component.html'
})
export class YOYComparisonComponent implements OnInit {
  @Output() yoyload = new EventEmitter();
  yoyLoaded = false;
  yoyComparisonForm: FormGroup;
  pipSheetId: number;
  yearPeriod: IYOYComparisonPeriod[];
  translationData: any = {};
  yearCols: any = [];
  yoyComparisonData: IYOYComparisonSummary = {
    summaryYoyDTO: [], summaryYoyPeriodList: []
  };

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private dateService: DateService,
    private summaryService: SummaryService,
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
    });
  }

  ngOnInit() {
    this.initializeForm();
    this.getYearComparisonData();
  }

  initializeForm() {
    this.yoyComparisonForm = this.fb.group({
      summaryYoyDTO: this.fb.array([])
    });
    this.yoyLoaded = true;
    this.yoyload.emit(this.yoyLoaded);
  }

  bindYearComparisonFormData() {
    this.yoyComparisonData.summaryYoyDTO.forEach((price, index) => {
      this.yearForm.push(this.yearData(price, index));
    });
  }

  getYearComparisonData() {
    this.summaryService.getYearComparison(this.pipSheetId).subscribe(data => {
      this.getPeriods(data[0].summaryYoyPeriodList);
      this.yoyComparisonData.summaryYoyDTO = data;
      this.bindYearComparisonFormData();
    });
  }

  get yearForm() {
    return this.yoyComparisonForm.get('summaryYoyDTO') as FormArray;
  }

  get year() {
    return this.yearForm.controls;
  }
  private yearData(yearComparison: IYOYComparison, index: number): FormGroup {
    const yearForm = this.fb.group({
      descriptionId: [yearComparison.descriptionId],
      total: [yearComparison.total],
      summaryYoyPeriodList: this.yearComparisonPeriodData(yearComparison.summaryYoyPeriodList, index)
    });
    return yearForm;
  }

  private yearComparisonPeriodData(periodPrice: IYOYComparisonPeriod[], index: number): FormArray {
    const periodPriceData = this.fb.array([]);

    periodPrice.forEach((period) => {
      periodPriceData.push(this.formulateYearComparisonPeriodForm(period));
    });

    return periodPriceData;
  }

  private formulateYearComparisonPeriodForm(period: IYOYComparisonPeriod) {
    const periodForm = this.fb.group({
      yearId: [period.yearId],
      year: [period.year],
      price: [period.price]
    });
    return periodForm;
  }

  private getPeriods(periods: IYOYComparisonPeriod[]) {
    this.yearPeriod = periods;
    if (periods.length > 0) {
      this.yearCols = new ResourceMapper(this.dateService).computeDyanmicYearColumns(periods);
    }
  }
}
