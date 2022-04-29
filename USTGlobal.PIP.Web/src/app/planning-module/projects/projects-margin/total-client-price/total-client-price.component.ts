import { HttpClient } from '@angular/common/http';
import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { SharedDataService } from '@global';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';
import { IRoleAndAccount } from '@shared/domain/IRoleAndAccount';
import { ICheckRole } from './../../../../shared-module/domain/ICheckRole';
import { NotificationService } from './../../../../global-module/services/notifications.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, FormArray, FormControl, AbstractControl } from '@angular/forms';
import { TotalClientPrice, ITotalClientPrice, ITotalClientPricePeriods, IplForcast, IplForecastPeriod, NegativeValuePipe } from '@shared';
import { TotalClientPriceService } from '@shared/services/total-client-price.service';
import { ProjectPeriod, ResourceMapper } from '@shared';
import { DateService } from '@core/services/date.service';
import { TranslateService } from '@ngx-translate/core';
import { MessageService, MenuItem } from 'primeng/api';
import { isNullOrUndefined } from 'util';
import { startWith, pairwise } from 'rxjs/operators';

@Component({
  selector: 'app-total-client-price',
  templateUrl: './total-client-price.component.html',
  providers: [NegativeValuePipe]
})
export class TotalClientPriceComponent implements OnInit {
  totalClientPriceForm: FormGroup;
  projectPeriod: ProjectPeriod[];
  colSpanSize = 0;
  translationData: any;
  pipSheetId: number;
  projectId: number;
  periodTotals: number[] = [];
  totalClientPriceData: TotalClientPrice = {
    clientPriceDTO: [], projectPeriodDTO: [], feesAtRisk: 0, currencyId: 0, netEstimatedRevenue: 0, plForecastDTO: []
  };
  contextMenuItems: MenuItem[];
  currentCellIndex = -1;
  currentCellValue = -1;
  currentRowIndex = -1;
  currentRowData: any;
  periodCols: any = [];
  feesAtRisk: number;
  netEstimatedRevenue: number;
  isValid: Boolean = false;
  message = { type: '', text: '' };
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  roleAndAccount: IRoleAndAccount[];
  checkRole: ICheckRole;
  isDataAvailable = false;
  loggedInUserId: number;
  dashboardId: number;
  isSaveClicked = false;
  peakCumulativeIndex: number;
  positiveCumulativeIndex: number;
  currency: string;


  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private totalClientPriceService: TotalClientPriceService,
    private dateService: DateService,
    private translateService: TranslateService,
    private messageService: MessageService,
    private notificationService: NotificationService,
    private sharedDataService: SharedDataService,
    private userWorkflowService: UserWorkflowService,
    private negativePipe: NegativeValuePipe
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
      this.projectId = parseInt(data['params'].projectId, 10);
      this.dashboardId = parseInt(data['params'].dashboardId, 10);
    });
  }

  ngOnInit() {
    this.contextMenuItems = [{
      label: 'REPEAT >>',
      command: (event: any) => {
        this.onCellClick(true);
      }
    }];
    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });
    this.initializeForm();
    this.getTotalClientPriceData();
    this.notificationService.currencyDataChangeNotification.subscribe((currency) => {
      this.currency = currency;
    });

  }

  initializeForm() {
    this.totalClientPriceForm = this.fb.group({
      clientPriceDTO: this.fb.array([]),
      plForecastDTO: this.fb.array([])
    });
  }

  enableDisableForm() {
    let flag = false;
    if (this.dashboardId === 3) {     // To be opened in readonly mode
      flag = true;
    }
    else {
      flag = this.userWorkflowService.isFormDisabled(this.checkRole, this.workflowFlag, this.loggedInUserId, this.dashboardId);
    }
    if (flag) {
      setTimeout(() => {
        this.totalClientPriceForm.disable();
      }, 200);
    }
  }

  configureDirtyCheck() {
    this.totalClientPriceForm.valueChanges.subscribe(() => {
      if (this.totalClientPriceForm.dirty) {
        this.notificationService.notifyFormDirty(true);
      }
    });
  }

  bindFormData() {
    this.totalClientPriceData.clientPriceDTO.forEach((price, index) => {
      this.clientPriceFrom.push(this.clientPriceData(price, index));
    });
  }

  bindPlForcastFormData() {
    this.totalClientPriceData.plForecastDTO.forEach((price, index) => {
      this.plForcastForm.push(this.plForcastData(price, index));
    });
  }

  getTotalClientPriceData() {
    this.totalClientPriceData = {
      clientPriceDTO: [], projectPeriodDTO: [], feesAtRisk: 0, currencyId: 0, netEstimatedRevenue: 0, plForecastDTO: []
    };
    this.initializeForm();
    this.roleAndAccount = this.sharedDataService.roleAndAccount;
    this.loggedInUserId = this.sharedDataService.sharedData.userRoleAccountDTO.userId;
    if (this.roleAndAccount != null) {
      this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
    }
    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      // get total client price
      this.totalClientPriceService.getTotalClientPrice(this.pipSheetId).subscribe(data => {
        this.getPeriods(data.projectPeriodDTO);
        // check Total Client Price data available or fetch default data

        this.totalClientPriceData.currencyId = data.currencyId;
        if (data.clientPriceDTO.length > 1) {
          this.totalClientPriceData.clientPriceDTO = data.clientPriceDTO;
        }
        else {
          this.totalClientPriceData.clientPriceDTO = data.clientPriceDTO;
          this.totalClientPriceData.clientPriceDTO = this.totalClientPriceService.getDefaultTotalClientPrice
            (this.projectPeriod, this.pipSheetId, this.totalClientPriceData.clientPriceDTO);
        }
        this.totalClientPriceData.projectPeriodDTO = data.projectPeriodDTO;
        this.totalClientPriceData.plForecastDTO = data.plForecastDTO;
        this.feesAtRisk = data.feesAtRisk;
        this.netEstimatedRevenue = data.netEstimatedRevenue;
        this.bindFormData();
        this.onPriceChange();
        this.bindPlForcastFormData();
        this.showTotalInvoicedPeriods();
        this.showProjectCost();
        this.showNetCashFlow();
        this.showCumulativeCashFlow();
        this.invoicePlanValidations();
        // this.invoicePlanErrorMsg();
        this.pipSheetWorkflowStatus = this.sharedDataService.sharedData.pipSheetWorkflowStatus;
        this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus[0], this.wfstatus);
        this.isDataAvailable = true;
        this.enableDisableForm();
        this.configureDirtyCheck();
        this.isSaveClicked = false;
      });
    }
    else {
      this.isDataAvailable = true;
    }
  }


  /* total_client_price_From binding start */
  get clientPriceFrom() {
    return this.totalClientPriceForm.get('clientPriceDTO') as FormArray;
  }

  get client() {
    return this.clientPriceFrom.controls;
  }

  get plForcastForm() {
    return this.totalClientPriceForm.get('plForecastDTO') as FormArray;
  }

  get plForcast() {
    return this.plForcastForm.controls;
  }

  private clientPriceData(totalClientPrice: ITotalClientPrice, index: number): FormGroup {
    const clientPriceForm = this.fb.group({
      uId: [index],
      clientPriceId: [totalClientPrice.clientPriceId],
      pipSheetId: this.pipSheetId,
      descriptionId: [index],
      totalPrice: [totalClientPrice.totalPrice],
      clientPricePeriodDTO: this.totalClientPricePeriodData(totalClientPrice.clientPricePeriodDTO, index),
    });
    return clientPriceForm;
  }

  private totalClientPricePeriodData(periodPrice: ITotalClientPricePeriods[], uid: number): FormArray {
    const periodPriceData = this.fb.array([]);

    periodPrice.forEach((period, index) => {
      periodPriceData.push(this.formulateTotalClinetPricePeriodForm(period, index, uid));
    });

    return periodPriceData;
  }

  private formulateTotalClinetPricePeriodForm(period: ITotalClientPricePeriods, index: number, uid: number) {
    const periodForm = this.fb.group({
      uId: [uid],
      clientPriceId: [period.clientPriceId],
      billingPeriodId: [this.totalClientPriceData.projectPeriodDTO[index].billingPeriodId],
      price: [period.price]
    });
    return periodForm;
  }

  private plForcastData(plForcast: IplForcast, index: number): FormGroup {
    const plForcastForm = this.fb.group({
      pipSheetId: this.pipSheetId,
      descriptionId: [plForcast.descriptionId],
      total: [plForcast.total],
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
      billingPeriodId: [this.totalClientPriceData.projectPeriodDTO[index].billingPeriodId],
      price: [period.price]
    });
    return periodForm;
  }

  private getPeriods(periods: ProjectPeriod[]) {
    this.projectPeriod = periods;
    if (periods.length > 0) {
      this.periodCols = new ResourceMapper(this.dateService).computeDyanmicColumns(periods);
    }
  }
  /* total_client_price_From binding end */
  onSave(clientFormData) {
    this.isSaveClicked = true;
    this.totalClientPriceService.saveTotalClientPrice(clientFormData).subscribe(success => {
      this.totalClientPriceService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
        this.notificationService.notifyTotalClientPriceExists(headerInfo.header1.totalClientPrice.toString());
        this.notificationService.notifyPercentEbitdaExists(headerInfo.headerEbitda.projectEBITDAPercent.toString());
      });
      this.getTotalClientPriceData();
      this.getOverrideNotificationStatus();
      this.translateService.get('SuccessMessage.TotalClientPrice').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
      });
      this.notificationService.notifyFormDirty(false);
    }, () => {
      this.translateService.get('ErrorMessage.TotalClientPrice').subscribe(msg => {
        this.messageService.add({ severity: 'error', detail: msg });
      });
      this.isSaveClicked = false;
    });
  }

  onPriceChange() {
    const priceTotals = this.totalClientPriceService.priceCalculations(this.clientPriceFrom.value);
    this.clientPriceFrom.controls.forEach((row, index) => {
      const paidPrice = row.get('totalPrice') as FormControl;
      if (index !== 5) {
        paidPrice.setValue(priceTotals.totalPrice[index]);
      }
    });
  }

  // Show  Total Invoiced Periods
  private showTotalInvoicedPeriods() {
    const totalInvoicePeriods = this.client[2].get('clientPricePeriodDTO') as FormArray;
    for (let i = 0; i < totalInvoicePeriods.controls.length; i++) {
      const priceControl = totalInvoicePeriods.controls[i].get('price') as FormControl;
      if (this.client[1].value.totalPrice > 0) {
        priceControl.setValue(this.totalClientPriceService.computeTotalInvoicedPeriods(this.client, i));
      } else {
        priceControl.setValue(this.totalClientPriceService.computeTotalInvoicedPeriods(this.client, i));
      }
    }
    this.showNetCashFlow();
    this.invoicePlanValidations();
    this.onPriceChange();
    // this.invoicePlanErrorMsg();
  }

  // Show Project Cost
  private showProjectCost() {
    const periodWiseProjectCost = this.client[3].get('clientPricePeriodDTO') as FormArray;
    for (let i = 0; i < periodWiseProjectCost.controls.length; i++) {
      const priceControl = periodWiseProjectCost.controls[i].get('price') as FormControl;
      priceControl.setValue(this.totalClientPriceService.computeProjectCost(this.plForcast, i));
    }
  }

  // Show Net Cash Flow
  private showNetCashFlow() {
    const periodWiseNetCashFlow = this.client[4].get('clientPricePeriodDTO') as FormArray;
    for (let i = 0; i < periodWiseNetCashFlow.controls.length; i++) {
      const priceControl = periodWiseNetCashFlow.controls[i].get('price') as FormControl;
      priceControl.setValue(this.totalClientPriceService.computeNetCashFlow(this.client, i));
    }
    this.showCumulativeCashFlow();
    this.showProjectCost();
  }

  // Show Cumulative Cash Flow
  private showCumulativeCashFlow() {
    const periodWiseCumulativeCashFlow = this.client[5].get('clientPricePeriodDTO') as FormArray;
    for (let i = 0; i < periodWiseCumulativeCashFlow.length; i++) {
      const priceControl = periodWiseCumulativeCashFlow.controls[i].get('price') as FormControl;
      if (i === 0) {
        priceControl.setValue(this.totalClientPriceService.computeCumulativeCashFlow(this.client, i, periodWiseCumulativeCashFlow));
      }
      else {
        priceControl.setValue(this.totalClientPriceService.computeCumulativeCashFlow(this.client, i, periodWiseCumulativeCashFlow));
      }
    }
  }

  invoicePlanValidations() {
    const totalClientPrice = this.client[0].value.totalPrice;
    const totalInvoicePlan = this.client[1].value.totalPrice;
    const totalDifference = (totalClientPrice - totalInvoicePlan);
    if (totalInvoicePlan > 0) {
      if (totalInvoicePlan < 1 && totalInvoicePlan !== 0) {
        this.isValid = false;
        this.message.text = 'Override enteries above zero, but under 1';
        this.message.type = 'error';
      }
      else if ((totalInvoicePlan - totalClientPrice) > 1) {
        this.isValid = false;
        this.message.text = 'Invoice sums are high by ' + Math.abs(totalDifference).toFixed(2);
        this.message.type = 'error';
      }
      else if ((totalInvoicePlan - totalClientPrice) < -1) {
        this.isValid = false;
        this.message.text = 'Invoice sums are short by ' + Math.abs(totalDifference).toFixed(2);
        this.message.type = 'error';
      }
      else {
        this.isValid = true;
        this.message.type = 'Applied';
      }
    }
    else {
      this.isValid = true;
      this.message.type = 'Applied';
    }
  }

  invoicePlanErrorMsg() {
    const totalClientPrice = this.client[0].value.totalPrice;
    const totalInvoicePlan = this.client[1].value.totalPrice;
    const totalDifference = (totalClientPrice - totalInvoicePlan);
    if (this.isValid === false) {
      this.message.text = 'Invoice sums are short by ' + totalDifference.toFixed(2);
      this.message.type = 'error';
    }
    else if (this.isValid === true) {
      this.message.type = 'Applied';
    }
  }

  getClientPriceControls(): AbstractControl[] {
    const prices = this.totalClientPriceForm.controls.clientPriceDTO as FormArray;
    const priceGroup = prices.controls;
    return priceGroup;
  }

  onCellClick(isContextEvent: boolean, event?, rowData?, rowIndex?: number) {
    if (!isContextEvent) {
      this.currentCellIndex = +event.target.id;
      this.currentRowIndex = rowIndex;
      this.currentRowData = rowData;
    } else {
      const periods: FormArray = (<FormArray>this.getClientPriceControls()[this.currentRowIndex]).controls['clientPricePeriodDTO'];
      this.currentCellValue = +<FormGroup>periods.controls[this.currentCellIndex].value.price;
      periods.controls.forEach((lostRevenueControl, index) => {
        if (index > this.currentCellIndex) {
          (<FormGroup>lostRevenueControl).controls['price'].setValue(this.currentCellValue);
        }
      });
      this.onPriceChange();
      this.showTotalInvoicedPeriods();
      this.scrollToEnd((periods.length - 1).toString());
    }
    this.getPositiveNetCashFlowMonth();
    this.getPositiveCumulativeCashFlowMonth();
    this.getPeakNegativeCashFlowAmount();
  }
  scrollToEnd(length: string) {
    document.getElementById(length).focus();
  }

  getOverrideNotificationStatus() {
    this.userWorkflowService.getOverrideNotificationStatus(+this.pipSheetId).subscribe(item => {
      const overrideNotification = item;
      if (overrideNotification.clientPrice || overrideNotification.riskManagement
        || overrideNotification.vacationAbsence || overrideNotification.ebitdaStdOverhead) {
        this.notificationService.showNotificationDialog(this.pipSheetId);
      }
    });
  }

  getPositiveNetCashFlowMonth() {
    const monthWiseNetCashFlow = this.totalClientPriceForm.value.clientPriceDTO[4].clientPricePeriodDTO;
    // this.totalClientPriceData.clientPriceDTO[4].clientPricePeriodDTO;
    for (let i = 0; i < monthWiseNetCashFlow.length; i++) {
      if (monthWiseNetCashFlow[i].price > 0) {
        return this.getMonth(this.periodCols[i].header);
      }
    }
  }

  getPositiveCumulativeCashFlowMonth() {
    const monthWiseCumulativeCashFlow = this.totalClientPriceForm.value.clientPriceDTO[5].clientPricePeriodDTO;
    //  this.totalClientPriceData.clientPriceDTO[5].clientPricePeriodDTO;
    for (let i = 0; i < monthWiseCumulativeCashFlow.length; i++) {
      if (monthWiseCumulativeCashFlow[i].price > 0) {
        return (i + 1) + ' Month' + ', &nbsp' + this.getMonth(this.periodCols[i].header);
      }
    }
  }

  getPeakNegativeCashFlowAmount() {

    const monthWiseCumulativeCashFlow = this.totalClientPriceForm.value.clientPriceDTO[5].clientPricePeriodDTO;
    // this.totalClientPriceData.clientPriceDTO[5].clientPricePeriodDTO;
    const negativeCashFlowAmounts = monthWiseCumulativeCashFlow.filter(amount => amount.price < 0)
      .map(a => a.price);

    // const areAllEqual = negativeCashFlowAmounts.length > 1 ? negativeCashFlowAmounts.every((val, index, arr) => val === arr[0]) : false;

    if (negativeCashFlowAmounts && negativeCashFlowAmounts.length > 0) {
      const min = negativeCashFlowAmounts.reduce((a, b) => Math.min(a, b));
      this.peakCumulativeIndex = monthWiseCumulativeCashFlow.findIndex(item => item.price === min);

      const amount = +min.toFixed(2);
      return this.negativePipe.transform(amount.toLocaleString()) + ', &nbsp' +
        this.getMonth(this.periodCols[this.peakCumulativeIndex].header);
    }
  }

  getMonth(header: string) {
    return header.split('\n')[1];
  }
  get Currency() {
    return this.sharedDataService.sharedData.currencyDTO.find(c => c.currencyId === this.totalClientPriceData.currencyId).symbol;
  }
}
