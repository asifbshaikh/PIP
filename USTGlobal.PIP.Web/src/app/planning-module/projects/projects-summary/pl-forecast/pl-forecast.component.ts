import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { ProjectPeriod, IplForcast, IplForecastPeriod, ResourceMapper } from '@shared';
import { IplForecastSummary } from '@shared/domain/IplForecastSummary';
import { SummaryService } from '@shared/services/summary.service';
import { ActivatedRoute } from '@angular/router';
import { DateService } from '@core';
import { SharedDataService } from '@global';

@Component({
  selector: 'app-pl-forecast',
  templateUrl: './pl-forecast.component.html'
})
export class PlForecastComponent implements OnInit {
  @Output() forecastload = new EventEmitter();
  @Output() plforecastData: EventEmitter<IplForcast> = new EventEmitter<IplForcast>();
  forecastloaded = false;
  plForecastForm: FormGroup;
  projectPeriod: ProjectPeriod[];
  pipSheetId: number;
  periodCols: any = [];
  translationData: any = {};
  currencySymbol: string;
  rowGroupMetadata: any;
  rowIndex: number;
  collapsedPriceClient = false;
  collapsedProjectCost = false;
  selectedRow: any;
  collapse = false;
  year: any;
  check_year = 'Year';
  pPeriod: any;
  isCollapse = false;
  colIndex: number;
  yearList: Array<{ year: any, collapse: boolean }> = [];
  plForecastData: IplForecastSummary = {
    plForecastDTO: [], projectPeriodDTO: []
  };
  constructor(
    private translateService: TranslateService,
    private summaryService: SummaryService,
    private route: ActivatedRoute,
    private sharedData: SharedDataService,
    private dateService: DateService,
    private fb: FormBuilder,
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
    });
  }

  ngOnInit() {
    this.initializeForm();
    this.getPlForecastData();
    this.currencySymbol =
      this.sharedData.sharedData.currencyDTO.find(sym => sym.currencyId === this.sharedData.sharedData.currencyId).symbol;
  }

  initializeForm() {
    this.plForecastForm = this.fb.group({
      plForecastDTO: this.fb.array([])
    });
  }

  bindPlForcastFormData() {
    this.plForecastData.plForecastDTO.forEach((price, index) => {
      this.plForcastForm.push(this.plForcastData(price, index));
    });
  }
  highlightRow(rowindex) {
    this.selectedRow = rowindex;
  }
  onExpandCollapseColumn(header, index) {
    this.year = header;
    for (let i = 0; i < this.yearList.length; i++) {
      if (this.yearList[i].year === Number(this.year)) {
        this.yearList[i].collapse = !this.yearList[i].collapse;
      }
    }
    for (let i = 0; i < this.periodCols.length; i++) {
      if (this.periodCols[i].year === Number(this.year) && this.periodCols[i].billingPeriodId !== 0) {
        this.periodCols[i].isCollapse = !this.periodCols[i].isCollapse;
      }
    }
 }

  getPlForecastData() {
    this.summaryService.getPLForecast(this.pipSheetId).subscribe(data => {
      this.translateService.get('ProjectSummary').subscribe((label) => {
        data.plForecastDTO.forEach((plForecastRow, index) => {
          this.translationData = label.plForeCastLabels[index].label;
          plForecastRow.plForecastLabels = this.translationData;
        });
      });
      this.getPeriods(data.projectPeriodDTO);
      this.plForecastData.plForecastDTO = data.plForecastDTO;
      this.plForecastData.projectPeriodDTO = data.projectPeriodDTO;
      this.plforecastData.emit(data.plForecastDTO);
      this.bindPlForcastFormData();
    });
    this.forecastloaded = true;
    this.forecastload.emit(this.forecastloaded);
  }

  get plForcastForm() {
    return this.plForecastForm.get('plForecastDTO') as FormArray;
  }

  get plForcast() {
    return this.plForcastForm.controls;
  }
  private plForcastData(plForcast: IplForcast, index: number): FormGroup {
    const plForcastForm = this.fb.group({
      pipSheetId: this.pipSheetId,
      descriptionId: [plForcast.descriptionId],
      total: [plForcast.total],
      label: [plForcast.plForecastLabels],
      plForecastPeriodDTO: this.plForcastPeriodData(plForcast.plForecastPeriodDTO, index),
    });
    return plForcastForm;
  }
  private plForcastPeriodData(periodPrice: IplForecastPeriod[], uid: number): FormArray {
    const periodPriceData = this.fb.array([]);

    periodPrice.forEach((period, index) => {
      periodPriceData.push(this.formulatePlForcastPeriodForm(period, index, uid));
    });

    return periodPriceData;
  }

  private formulatePlForcastPeriodForm(period: IplForecastPeriod, index: number, uid: number) {
    const periodForm = this.fb.group({
      billingPeriodId: [this.plForecastData.projectPeriodDTO[index].billingPeriodId],
      price: [period.price],
      year: [period.year],
      isCollapse: true
    });
    return periodForm;
  }

  private getPeriods(periods: ProjectPeriod[]) {
    this.projectPeriod = periods;
    if (this.projectPeriod.length > 0) {
      this.periodCols = new ResourceMapper(this.dateService).computeDyanmicPLForecasteColumns(periods);
    }
    this.pPeriod = this.projectPeriod.filter((item) => item.billingPeriodId === 0);
    for (let i = 0; i < this.pPeriod.length; i++) {
      this.year = this.pPeriod[i].year;
      this.yearList.push({ year: this.year, collapse: this.collapse });
    }
  }

  onExpandCollapse(index) {
    switch (!index.undefined) {
      case index === 0:
        this.collapsedPriceClient = !this.collapsedPriceClient;
        break;
      case index === 10:
        this.collapsedProjectCost = !this.collapsedProjectCost;
        break;
    }
  }
}
