import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { SummaryService } from '@shared/services/summary.service';
import { IBillingSchedule, ProjectPeriod, ResourceMapper } from '@shared';
import { ActivatedRoute } from '@angular/router';
import { DateService } from '@core';
import { SharedDataService } from '@global';

@Component({
  selector: 'app-billing',
  templateUrl: './billing.component.html'
})

export class BillingComponent implements OnInit {
  @Output() billingload = new EventEmitter();
  billingLoaded = false;
  projectPeriod: ProjectPeriod[];
  pipSheetId: number;
  periodCols: any = [];
  blendedLaborCostPerHr: number;
  blendedBillRate: number;
  translationData: any = {};
  currencySymbol: string;
  selectedRow: any;
  collapse = false;
  year: any;
  yearList: Array<{ year: any, collapse: boolean }> = [];
  check_year = 'Year';
  colIndex: number;
  count = 0;
  counter = 0;
  yearCol: any = [];
  pPeriod: any;
  headerIndex: number;
  billingScheduleData: IBillingSchedule = {
    clientPriceDTO: [], projectPeriodDTO: [], cashFlowParentDTO: [], blendedLaborCostPerHr: 0, blendedBillRate: 0, pipSheetId: 0,
  };
  constructor(
    private summaryService: SummaryService,
    private sharedData: SharedDataService,
    private route: ActivatedRoute,
    private dateService: DateService,
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
    });
  }

  ngOnInit() {
    this.getBillingScheduleData();
    this.currencySymbol =
      this.sharedData.sharedData.currencyDTO.find(sym => sym.currencyId === this.sharedData.sharedData.currencyId).symbol;

  }

  public highlightRow(rowindex) {
    this.selectedRow = rowindex;
  }
  getBillingScheduleData() {
    this.summaryService.getBillingSchedule(this.pipSheetId).subscribe(billingSchedule => {
      this.getPeriods(billingSchedule.projectPeriodDTO);
      if (billingSchedule.cashFlowParentDTO.length > 1) {
        this.billingScheduleData.cashFlowParentDTO = billingSchedule.cashFlowParentDTO;
      }
      else {
        this.billingScheduleData.cashFlowParentDTO = this.summaryService.getDefaultBillingSchedule(this.projectPeriod,
          this.pipSheetId, this.billingScheduleData.cashFlowParentDTO);
      }
      this.billingScheduleData.projectPeriodDTO = billingSchedule.projectPeriodDTO;
      this.blendedBillRate = billingSchedule.blendedBillRate;
      this.blendedLaborCostPerHr = billingSchedule.blendedLaborCostPerHr;

    });
    this.billingLoaded = true;
    this.billingload.emit(this.billingLoaded);
  }
  onExpandCollapseColumn(header, index) {
    this.year = header;
    for (let i = 0; i < this.yearList.length; i++) {
      if (this.yearList[i].year === Number(this.year)) {
        this.yearList[i].collapse = !this.yearList[i].collapse;
      }
    }
  }

  private getPeriods(periods: ProjectPeriod[]) {
    this.projectPeriod = periods;
    if (periods.length > 0) {
      this.periodCols = new ResourceMapper(this.dateService).computeCashFlowDyanmicColumns(periods);
    }
    this.pPeriod = this.projectPeriod.filter((item) => item.billingPeriodId === 0);
    for (let i = 0; i < this.pPeriod.length; i++) {

      this.year = this.pPeriod[i].year;
      this.yearList.push({ year: this.year, collapse: this.collapse });
    }
  }
}
