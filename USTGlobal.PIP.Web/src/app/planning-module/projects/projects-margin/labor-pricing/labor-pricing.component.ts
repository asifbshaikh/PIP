import { OverrideNotificationStatus } from './../../../../shared-module/domain/override-notification-status';
import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { IRoleAndAccount } from './../../../../shared-module/domain/IRoleAndAccount';
import { ICheckRole } from './../../../../shared-module/domain/ICheckRole';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';

import { Component, OnInit, AfterViewInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl, Form, AbstractControl } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { Mastermapper } from '@shared/mapper/master/mastermapper';
import { LabourPricingService } from '@shared/services/labour-pricing.service';
import { ActivatedRoute } from '@angular/router';
import { Constants } from '@shared/infrastructure/constants';
import { isNullOrUndefined } from 'util';
import { ResourceMapper } from '@shared/mapper/master/resourcemapper';
import { DateService } from '@core/services/date.service';
import { ILabor, ICurrency, IBackgroundCalculations, ProjectPeriod, ResourcePeriod, ILocation, Milestone } from '@shared';
import { LaborPricingMapper } from '@shared/mapper/master/laborpricingmapper';
import { MessageService, SelectItem } from 'primeng/api';
import { SharedDataService, NotificationService } from '@global';
import { ValidationService } from '@core';
import { IBackgroundParam } from '@shared/domain/IBackgroundParam';


@Component({
  selector: 'app-labor-pricing',
  templateUrl: './labor-pricing.component.html',
})
export class LaborPricingComponent implements OnInit, AfterViewInit {

  laborPricingDetails: LaborPricingMapper;
  pipSheetId: number;
  isMarginSet: boolean;
  laborPricingForm: FormGroup;
  frozenCols: any[] = [];
  laborPricingCols: any[] = [];
  periodCols: any[] = [];
  whichMargins;
  setColumnHeading: string;
  utilization: SelectItem[];
  currencyData: ICurrency;
  backgroundCalValues: IBackgroundCalculations;
  totalOfMarginPercent: number;
  totalOfCappedCost: number;
  totalOfTotalRevenue: number;
  totalRevenue: string[] = [];
  ratePerhour: number;
  yearCostPerHour: number;
  inflatedCappedCost: number;
  standardCost = 0;
  isComputed = false;
  staffHours = 0;
  message = { type: '', text: '' };
  inflationDetailsPerResource = [];
  isInflationApplicable: boolean;
  periodWiseInflation = [];
  inflationPerResource = 0;
  calcInflation = true;
  isEvent = false;
  colSpanSize = 0;
  endYear: number;
  nonBillableCategory: SelectItem[];
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  roleAndAccount: IRoleAndAccount[];
  checkRole: ICheckRole;
  projectId: number;
  dashboardId: number;
  isDataAvailable = false;
  loggedInUserId: number;

  constructor(
    private fb: FormBuilder,
    private translateService: TranslateService,
    private labourPricingService: LabourPricingService,
    private route: ActivatedRoute,
    private dateService: DateService,
    private messageService: MessageService,
    private validate: ValidationService,
    private sharedDataService: SharedDataService,
    private notificationService: NotificationService,
    private userWorkflowService: UserWorkflowService
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
      this.projectId = parseInt(data.get(Constants.uiRoutes.routeParams.projectId), 10);
      this.dashboardId = parseInt(data.get(Constants.uiRoutes.routeParams.dashboardId), 10);
    });
  }

  ngOnInit() {
    const laborPricingData = new Mastermapper();
    this.translateService.get('LaborPricing.FrozenColumns').subscribe(cols => {
      this.frozenCols = cols;
    });
    this.translateService.get('LaborPricing.LaborPricingColumns').subscribe(cols => {
      this.laborPricingCols = cols;
    });

    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });

    this.whichMargins = laborPricingData.getTargetMargin();
    this.utilization = laborPricingData.getUtilizationDetails();
    this.nonBillableCategory = laborPricingData.getNonBillableCategorieComboItems(
      this.sharedDataService.sharedData.nonBillableCategoryDTO);

    this.buildForm();

    this.roleAndAccount = this.sharedDataService.roleAndAccount;
    this.loggedInUserId = this.sharedDataService.sharedData.userRoleAccountDTO.userId;
    if (this.roleAndAccount != null) {
      this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
    }

    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      // get labour pricing data
      this.labourPricingService.getLaborPricingDetails(this.pipSheetId).subscribe(data => {
        this.laborPricingDetails = data;
        // get periods
        this.getPeriods(this.laborPricingDetails.projectPeriodDTO); //  for adding dynamic period columns

        this.computeTotalStaffHours(this.laborPricingDetails.resourceLaborPricingDTOs);

        if (isNullOrUndefined(this.laborPricingDetails.marginDTO)) {
          this.laborPricingDetails.marginDTO = {
            createdBy: 1,
            isMarginSet: 0,
            marginId: 0,
            marginPercent: 0,
            pipSheetId: this.pipSheetId,
            updatedBy: 1,
            which: 0
          };
          this.BindFormData(false);
        } else {
          if (this.laborPricingDetails.marginDTO.which === 0) {
            this.laborPricingDetails.marginDTO.which = 1;   // Default Ebitda % should be selected
          }
          this.BindFormData(true);
        }
      });

    }
    else {
      this.isDataAvailable = true;
    }

    const backgroundParam = <IBackgroundParam>{
      pipSheetId: this.pipSheetId,
      isMarginSet: false,
      which: 1,
      marginPercent: 0,
      isInitLoad: true,
      inflatedCappedCost: 1,
      totalInflation: 1
    };
    this.labourPricingService.getBackgroundCalculations(backgroundParam).subscribe(backgroundCal => {
      this.backgroundCalValues = backgroundCal;
    });
  }

  ngAfterViewInit() {
    this.laborPricingForm.valueChanges.subscribe(() => {
      if (this.laborPricingForm.dirty) {
        this.notificationService.notifyFormDirty(true);
      }
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
        this.laborPricingForm.disable();
      }, 200);
    }
  }

  get isDeliveryTypeRestricted() {
    if (!isNullOrUndefined(this.laborPricingDetails)) {
      return this.laborPricingDetails.isDeliveryTypeRestricted;
    }
  }

  buildForm() {
    this.laborPricingForm = this.fb.group({
      isSetMargin: [this.isMarginSet, []],
      which: ['', []],
      marginPercent: ['', [Validators.pattern(Constants.regExType.percentageLessThanEqual100WithDecimalPrecisionTwo)]],
      laborPricing: this.fb.array([])
    });
  }

  calculateRevenuePerRow(period: ResourcePeriod[]): number {
    let totalRevenue = 0;
    period.forEach(data => {
      totalRevenue += data.revenue;
    });

    return totalRevenue;
  }

  // To Enable and Disable Margin fields
  onSetMarginSwitch() {
    this.isMarginSet = this.laborPricingForm.controls['isSetMargin'].value;
    this.isMarginSet ? this.laborPricingForm.controls['marginPercent'].enable() :
      this.laborPricingForm.controls['marginPercent'].disable();

    this.translateService.get('LaborPricing.ColumnHeading').subscribe(columnHeading => {
      this.setColumnHeading = this.isMarginSet ? columnHeading.IfSetMarginOn : columnHeading.IfSetMarginOff;
    });
    this.marginPercentValidationMsg();
  }

  onSave() {
    if (this.laborPricingForm.valid) {
      const formData = JSON.parse(JSON.stringify(this.laborPricingForm.getRawValue()));
      this.composeSaveData(formData);
      const value = JSON.stringify(this.laborPricingDetails);
      this.labourPricingService.saveLaborPricingDetails(this.laborPricingDetails).subscribe(success => {
        this.labourPricingService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
          this.notificationService.notifyTotalClientPriceExists(headerInfo.header1.totalClientPrice.toString());
          this.notificationService.notifyPercentEbitdaExists(headerInfo.headerEbitda.projectEBITDAPercent.toString());
        });
        this.translateService.get('SuccessMessage.LaborPricingSave').subscribe(msg => {
          this.messageService.add({ severity: 'success', detail: msg });
        });
        this.laborPricingForm.markAsPristine();
        this.notificationService.notifyFormDirty(false);
        this.getOverrideNotificationStatus();
      }, () => {
        this.translateService.get('ErrorMessage.LaborPricingSave').subscribe(msg => {
          this.messageService.add({ severity: 'error', detail: msg });
        });
      });
    }
  }

  private getPeriods(periods: ProjectPeriod[]) {
    if (this.laborPricingCols.length > 0) {
      this.periodCols = new ResourceMapper(this.dateService).computeDyanmicColumns(periods);
      this.laborPricingCols = this.laborPricingCols = this.laborPricingCols.concat(this.periodCols);
    }
  }

  private composeSaveData(formData: any) {
    const marginDetails = this.laborPricingDetails.marginDTO;
    let index = -1;

    marginDetails.isMarginSet = formData.isSetMargin ? 1 : 0;
    marginDetails.marginPercent = isNullOrUndefined(formData.marginPercent) ? 0 :
      formData.marginPercent === '' ? 0 : formData.marginPercent;
    marginDetails.which = formData.which.id;
    marginDetails.createdBy = 1;
    marginDetails.updatedBy = 1;

    // bind labor pricing data
    this.laborPricingDetails.resourceLaborPricingDTOs.forEach(res => {
      index++;
      for (let i = 0; i < res.projectResourcePeriodDTO.length; i++) {
        res.projectResourcePeriodDTO[i].revenue = formData.laborPricing[index].periods[i].revenue;
        res.projectResourcePeriodDTO[i].priceAdjustment = formData.laborPricing[index].periods[i].priceAdjustment;
        res.projectResourcePeriodDTO[i].costHours = formData.laborPricing[index].periods[i].costHours;
        res.projectResourcePeriodDTO[i].cappedCost = formData.laborPricing[index].periods[i].cappedCost;
        res.projectResourcePeriodDTO[i].billRate = formData.laborPricing[index].periods[i].billRate;
        res.projectResourcePeriodDTO[i].costRate = formData.laborPricing[index].periods[i].costRate;
      }
      res.utilizationType = formData.laborPricing[index].utilizationType.id;
      res.nonBillableCategoryId = formData.laborPricing[index].nonBillableCategory.id;
      res.rate = formData.laborPricing[index].rate;
      res.cost = formData.laborPricing[index].cost;
      res.ratePerHour = +formData.laborPricing[index].ratePerHour;
      res.yr1PerHour = +formData.laborPricing[index].yearCostPerHour;
      res.margin = +formData.laborPricing[index].percentageMargin;
      res.cappedCost = +formData.laborPricing[index].cappedCost;
      res.totalRevenue = +formData.laborPricing[index].totalRevenue;
    });
  }

  private BindFormData(doesDataExists: boolean) {
    let targetMargin;
    if (!doesDataExists) {
      targetMargin = this.whichMargins[0].value;
      this.laborPricingForm.patchValue({
        isSetMargin: false,
        which: this.whichMargins[0].value,
        marginPercent: ''
      });
      this.isMarginSet = false;
    } else {
      targetMargin = this.whichMargins.find(item => item.value.id === this.laborPricingDetails.marginDTO.which);
      this.laborPricingForm.patchValue({
        isSetMargin: this.laborPricingDetails.marginDTO.isMarginSet,
        which: targetMargin.value,
        marginPercent: this.laborPricingDetails.marginDTO.marginPercent
      });
      this.isMarginSet = this.laborPricingForm.controls['isSetMargin'].value;
    }
    this.onSetMarginSwitch();
    this.getLabourPricingForm(this.laborPricingDetails.resourceLaborPricingDTOs);
    this.calculateOnSwitch();
    this.onMarginPercentChange();

    this.pipSheetWorkflowStatus = this.sharedDataService.sharedData.pipSheetWorkflowStatus;
    this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus[0], this.wfstatus);
    this.isDataAvailable = true;
    this.enableDisableForm();
  }


  private getLabourPricingForm(laborData: ILabor[]) {

    const laborDataForm: FormArray = this.fb.array([]);
    laborData.forEach(data => {

      // periods part
      const periods: FormArray = this.fb.array([]);

      for (let i = 0; i < data.projectResourcePeriodDTO.length; i++) {
        periods.push(this.fb.group({
          revenue: (this.isDeliveryTypeRestricted) ? 0 : [data.projectResourcePeriodDTO[i].revenue, []],
          totalHours: [data.projectResourcePeriodDTO[i].totalHours, []],
          priceAdjustment: [data.projectResourcePeriodDTO[i].priceAdjustment, []],
          costHours: [data.projectResourcePeriodDTO[i].costHours, []],
          cappedCost: [data.projectResourcePeriodDTO[i].cappedCost, []],
          billRate: [data.projectResourcePeriodDTO[i].billRate, []],
          costRate: [data.projectResourcePeriodDTO[i].costRate, []],
        }));
      }
      const serviceLine = this.sharedDataService.sharedData.resourceServiceLineDTO.find(sl =>
        sl.resourceServiceLineId === data.resourceServiceLineId).resourceServiceLineName;
      const utilType = this.utilization.find(t => t.value.id === data.utilizationType);
      const nonBillableCatType = this.nonBillableCategory.find(n => n.value.id ===
        (isNullOrUndefined(data.nonBillableCategoryId) ? -1 : data.nonBillableCategoryId));

      laborDataForm.push(this.fb.group({
        alias: data.alias,
        optionalPhase: data.milestoneName,
        location: data.locationName,
        locationId: data.locationId,
        resourceId: data.resourceId,
        ustRole: data.name,
        serviceLine: serviceLine,
        utilizationType: utilType.value,
        nonBillableCategory: [nonBillableCatType.value, [Validators.required, this.validate.validateDeselectedDropdown]],
        rate: data.rate,
        cost: data.cost,
        percent: data.percent,
        ratePerHour: (this.isDeliveryTypeRestricted) ? 0 : data.ratePerHour,
        yearCostPerHour: data.yr1PerHour,
        percentageMargin: (this.isDeliveryTypeRestricted) ? 0 : data.margin,
        cappedCost: data.cappedCost,
        totalRevenue: (this.isDeliveryTypeRestricted) ? 0 : data.totalRevenue,
        stdCost: data.standardCostRate,
        periods: periods,
        totalHoursPerResource: data.totalHoursPerResource,
        costHrsPerResource: data.costHrsPerResource,
        gradeClientRole: data.gradeClientRole
      }));
    });
    this.laborPricingForm.setControl('laborPricing', laborDataForm);
  }

  showRatePerHr(laborPricing, index: number) {
    const laborForm = (<FormArray>this.laborPricingForm.controls['laborPricing']).controls[index];
    const rateperhrControl = laborForm.get('ratePerHour');
    const rateControl = laborForm.get('rate');
    const billabilityControl = laborForm.get('nonBillableCategory');
    let yr1Cost = 0;

    const isBillable = laborPricing.utilizationType.id;

    if (this.isDeliveryTypeRestricted) {
      if (isBillable) {
        billabilityControl.disable();
      } else {
        rateperhrControl.setValue(0);
        rateControl.setValue('');
        billabilityControl.enable();
      }
    } else {
      let computedRatePerHour = 0;

      if (this.isMarginSet) {
        if (isBillable) {
          if ((!isNullOrUndefined(laborPricing.cost) && laborPricing.cost !== '') && +laborPricing.cost >= 0) {
            yr1Cost = +laborPricing.cost;
          } else {
            yr1Cost = laborPricing.yearCostPerHour;
          }
          billabilityControl.disable();
          const x = (yr1Cost * this.backgroundCalValues.g13) + this.backgroundCalValues.g14;
          const y = (x / this.backgroundCalValues.g15) / (this.staffHours === 0 ? 1 : this.staffHours);
          computedRatePerHour = y / this.backgroundCalValues.g16;
          rateperhrControl.setValue(computedRatePerHour);
        } else {
          billabilityControl.enable();
          computedRatePerHour = 0;
          rateperhrControl.setValue(computedRatePerHour);
        }
        // rateperhrControl.setValue(computedRatePerHour);
      } else {
        if (isBillable) {
          billabilityControl.disable();
          if ((!isNullOrUndefined(laborPricing.rate) && laborPricing.rate !== '') && +laborPricing.rate >= 0) {
            computedRatePerHour = +laborPricing.rate;

            // In case of Applied Rate overriden, when Set Margin = OFF, then Standard rate should always show master value (Bug 3673 Fix)
            const corpBillingRate = this.sharedDataService.sharedData.corpBillingRateDTO.filter(cbr => {
              return cbr.locationId === laborPricing.locationId && cbr.resourceId === laborPricing.resourceId;
            })[0].rate;
            rateperhrControl.setValue(corpBillingRate);
          } else {
            const corpBillingRate = this.sharedDataService.sharedData.corpBillingRateDTO.filter(cbr => {
              return cbr.locationId === laborPricing.locationId && cbr.resourceId === laborPricing.resourceId;
            })[0].rate;
            if (this.checkInflation()) {
              computedRatePerHour = this.applyInflationForRatePerHour(laborPricing.locationId, corpBillingRate);
            } else {
              computedRatePerHour = corpBillingRate;
            }
            rateperhrControl.setValue(computedRatePerHour);
          }
        } else {
          billabilityControl.enable();
          computedRatePerHour = 0;
          rateperhrControl.setValue(computedRatePerHour);
        }
      }
      this.calculateMonthlyRevenue(laborPricing, computedRatePerHour, index);
      this.calculateCellWiseBillRate(laborPricing, index);
    }
  }

  onAppliedCostChange(laborPricing, rowIndex: number, isEvent: boolean) {
    const laborPricingForm = this.laborPricingForm.get('laborPricing') as FormArray;

    const isMarginSet = isNullOrUndefined(this.laborPricingForm.controls.isSetMargin.value) ? false :
      this.laborPricingForm.controls.isSetMargin.value;

    const which: number = isNullOrUndefined(this.laborPricingForm.controls.which.value)
      ? 0 : this.laborPricingForm.controls.which.value.id;

    const marginPercent = parseFloat(isNullOrUndefined(this.laborPricingForm.controls.marginPercent) ? 0
      : this.laborPricingForm.controls.marginPercent.value === '' ? 0 : this.laborPricingForm.controls.marginPercent.value);


    let totalInflation: number;
    laborPricingForm.controls.forEach((control, index) => {
      this.showCostPerHr(control.value, index, false);
    });
    totalInflation = this.labourPricingService.computeTotalCappedCostWithoutInflationPerResource(this.laborPricingDetails);

    const backgroundParam = <IBackgroundParam>{
      pipSheetId: this.pipSheetId,
      isMarginSet: isMarginSet,
      which: which,
      marginPercent: marginPercent,
      isInitLoad: false,
      inflatedCappedCost: this.totalOfCappedCost,
      totalInflation: totalInflation
    };
    this.labourPricingService.getBackgroundCalculations(backgroundParam)
      .subscribe(backgroundCal => {
        this.backgroundCalValues = backgroundCal;
        this.showRatePerHr(laborPricing, rowIndex);
        this.showCostPerHr(laborPricing, rowIndex, isEvent);
        this.calculateMarginPercent(laborPricing.totalRevenue, laborPricing.cappedCost);
      });
  }


  showCostPerHr(laborPricing, index: number, isEvent: boolean) {
    this.isInflationApplicable = false;
    // this.isEvent = isEvent;
    let yearCostPerHour: number;
    const laborForm = (<FormArray>this.laborPricingForm.controls['laborPricing']).controls[index];
    const yearCostPerHrControl = laborForm.get('yearCostPerHour');
    if ((!isNullOrUndefined(laborPricing.cost) && laborPricing.cost !== '') && +laborPricing.cost >= 0) {
      laborPricing.yearCostPerHour = +laborPricing.cost;
      yearCostPerHour = +laborPricing.cost;
      this.isInflationApplicable = this.checkInflation();
      this.applyInflationForYear1Cost(laborPricing.locationId, yearCostPerHour, laborPricing, index);

    }
    else {
      yearCostPerHour = laborPricing.stdCost;
      if (laborPricing.percent !== null && laborPricing.percent > 0) {
        yearCostPerHour = ((laborPricing.percent / 100) * laborPricing.stdCost) + (laborPricing.stdCost);
      } else {
        yearCostPerHour = laborPricing.stdCost;
      }
      laborPricing.yearCostPerHour = +yearCostPerHour.toFixed(2);
      yearCostPerHrControl.setValue(yearCostPerHour.toFixed(2));
      this.isInflationApplicable = this.checkInflation();
      if (this.isInflationApplicable) {
        yearCostPerHour = this.applyInflationForYear1Cost(laborPricing.locationId, yearCostPerHour, laborPricing, index);
        laborPricing.yearCostPerHour = yearCostPerHour;
        yearCostPerHrControl.setValue(yearCostPerHour);
      }
      // START: To calculate Cost Rate Per Resoource Per Period
      else {
        this.setCellWiseCostRate(index, yearCostPerHour);
      }
      // END
    }
    this.calculateCappedCost(laborPricing, index);
  }

  calculateCappedCost(laborPricing, index: number) {
    let cappedCost: number;
    this.inflatedCappedCost = 0;
    this.inflationPerResource = 0;
    const currentYear = new Date().getFullYear();


    const laborForm = (<FormArray>this.laborPricingForm.controls['laborPricing']).controls[index];
    const cappedCostControl = laborForm.get('cappedCost');
    const periodsForm = laborForm.get('periods') as FormGroup;
    const labor = this.laborPricingDetails.resourceLaborPricingDTOs[index];
    if (this.isInflationApplicable && this.inflationDetailsPerResource.length > 0) {

      this.laborPricingDetails.projectPeriodDTO.forEach((period, periodIndex) => {
        if (period.year >= currentYear) {
          this.calculateInflationBasedCappedCost(period, labor, periodIndex, laborPricing.yearCostPerHour, index);
        } else {
          // capped cost should also be calculated for backdated projects.
          cappedCost = (laborPricing.costHrsPerResource * (laborPricing.yearCostPerHour.toFixed(2)));

          // START: To calculate Capped Cost Per Resoource Per Period
          this.setCellWiseCappedCost(index, periodIndex, (labor.projectResourcePeriodDTO[periodIndex].costHours
            * laborPricing.yearCostPerHour.toFixed(2)));
          // END
        }
      });
      cappedCost = (this.inflatedCappedCost === 0) ? cappedCost : this.inflatedCappedCost;
      labor.totalInflation = this.inflationPerResource;
    } else {
      const x = (laborPricing.costHrsPerResource * laborPricing.yearCostPerHour);
      cappedCost = x;

      // START: To calculate Capped Cost Per Resoource Per Period
      this.laborPricingDetails.projectPeriodDTO.forEach((period, periodIndex) => {
        this.setCellWiseCappedCost(index, periodIndex, (labor.projectResourcePeriodDTO[periodIndex].costHours
          * laborPricing.yearCostPerHour));
      });
      // END
    }
    cappedCostControl.setValue(parseFloat(cappedCost ? cappedCost.toFixed(2) : '0'));
    this.calculateTotalRevenue();
    this.computeTotalofTotals();
  }

  calculateInflationBasedCappedCost(period: ProjectPeriod, resource: ILabor, periodIndex: number, baseYear1Cost: any, index: number) {
    let perPeriodCappedCost = 0;
    const costHoursPerPeriod = resource.projectResourcePeriodDTO[periodIndex].costHours;
    const year1Cost = (this.inflationDetailsPerResource.find(specificYear => specificYear.year === period.year).year1Cost);
    let inflationPerPeriod = 0;
    perPeriodCappedCost = (year1Cost * costHoursPerPeriod);

    // calulate inflation based on year
    inflationPerPeriod = perPeriodCappedCost - (costHoursPerPeriod * baseYear1Cost);
    resource.projectResourcePeriodDTO[periodIndex].inflation = inflationPerPeriod;

    // START: To calculate Capped Cost Per Resoource Per Period
    this.setCellWiseCappedCost(index, periodIndex, perPeriodCappedCost);
    // END

    // per resource  overrall inflation total
    this.inflationPerResource += inflationPerPeriod;

    this.inflatedCappedCost += perPeriodCappedCost;
  }

  calculateTotalRevenue() {
    let cappedCost = 0;
    let margin = 0;
    const laborPricingArray = this.getLaborPricingFormArray();
    laborPricingArray.controls.forEach((laborPricingArrcontrol: FormGroup) => {
      if (!this.isDeliveryTypeRestricted) {
        let totalRevenue = 0;

        (<FormArray>laborPricingArrcontrol.controls.periods).controls.forEach((period: FormGroup) => {
          totalRevenue = totalRevenue + (+period.value.revenue) + ((+period.value.priceAdjustment) === null
            ? 0 : period.value.priceAdjustment);
        });

        laborPricingArrcontrol.get('totalRevenue').setValue(parseFloat(totalRevenue.toFixed(2)));
        cappedCost = laborPricingArrcontrol.get('cappedCost').value;
        margin = this.calculateMarginPercent(totalRevenue, cappedCost);
        laborPricingArrcontrol.get('percentageMargin').setValue(parseFloat(margin.toFixed(2)));
      } else {
        laborPricingArrcontrol.get('totalRevenue').setValue(0);
        laborPricingArrcontrol.get('percentageMargin').setValue(0);
      }
    });
    this.computeTotalofTotals();
  }



  calculateMonthlyRevenue(laborPricing, ratePerhour: any, index: number) {
    let revenue;
    const laborForm: FormGroup = (<FormArray>this.laborPricingForm.controls['laborPricing']).controls[index] as FormGroup;
    const periods: FormGroup[] = (<FormArray>laborForm.controls['periods']).controls as FormGroup[]; // use .get here
    const isMarginSet = isNullOrUndefined(this.laborPricingForm.controls.isSetMargin.value) ? false :
      this.laborPricingForm.controls.isSetMargin.value;
    laborPricing.periods.forEach((p, Itemindex) => {
      if (this.isDeliveryTypeRestricted) {
        periods[Itemindex].controls['revenue'].setValue(0);
      } else {
        if (laborPricing.totalHoursPerResource && laborPricing.totalHoursPerResource !== 0) {
          if (isMarginSet) {
            revenue = p.totalHours * ratePerhour;
          } else {
            revenue = p.totalHours * (ratePerhour.toFixed(2));
          }
          periods[Itemindex].controls['revenue'].setValue(parseFloat(revenue.toFixed(2)));
        } else {
          revenue = 0;
          periods[Itemindex].controls['revenue'].setValue(parseFloat(revenue.toFixed(2)));
        }

      }
    });

    this.calculateTotalRevenue();
  }

  onWhichSetMarginChange(data) {
    const laborPricingForm = this.laborPricingForm.get('laborPricing') as FormArray;

    const isMarginSet = isNullOrUndefined(this.laborPricingForm.controls.isSetMargin.value) ? false :
      this.laborPricingForm.controls.isSetMargin.value;

    const which: number = isNullOrUndefined(this.laborPricingForm.controls.which.value)
      ? 0 : this.laborPricingForm.controls.which.value.id;

    const marginPercent = parseFloat(isNullOrUndefined(this.laborPricingForm.controls.marginPercent) ? 0
      : this.laborPricingForm.controls.marginPercent.value === '' ? 0 : this.laborPricingForm.controls.marginPercent.value);


    let totalInflation: number;
    laborPricingForm.controls.forEach((control, index) => {
      this.showCostPerHr(control.value, index, false);
    });
    totalInflation = this.labourPricingService.computeTotalCappedCostWithoutInflationPerResource(this.laborPricingDetails);

    const backgroundParam = <IBackgroundParam>{
      pipSheetId: this.pipSheetId,
      isMarginSet: isMarginSet,
      which: which,
      marginPercent: marginPercent,
      isInitLoad: false,
      inflatedCappedCost: this.totalOfCappedCost,
      totalInflation: totalInflation
    };
    this.labourPricingService.getBackgroundCalculations(backgroundParam)
      .subscribe(backgroundCal => {
        this.backgroundCalValues = backgroundCal;
        laborPricingForm.controls.forEach((control, index) => {
          this.showRatePerHr(control.value, index);
          this.showCostPerHr(control.value, index, false);
          this.calculateMarginPercent(control.value.totalRevenue, control.value.cappedCost);
          this.computeTotalofTotals();
        });
      });
  }

  onMarginPercentChange() {
    const laborPricingForm = this.laborPricingForm.get('laborPricing') as FormArray;


    const isMarginSet = isNullOrUndefined(this.laborPricingForm.controls.isSetMargin.value) ? false :
      this.laborPricingForm.controls.isSetMargin.value;

    const which: number = isNullOrUndefined(this.laborPricingForm.controls.which.value)
      ? 0 : this.laborPricingForm.controls.which.value.id;

    const marginPercent = parseFloat(isNullOrUndefined(this.laborPricingForm.controls.marginPercent) ? 0
      : this.laborPricingForm.controls.marginPercent.value === '' ? 0 : this.laborPricingForm.controls.marginPercent.value);


    let totalInflation: number;
    laborPricingForm.controls.forEach((control, index) => {
      this.showCostPerHr(control.value, index, false);
    });
    totalInflation = this.labourPricingService.computeTotalCappedCostWithoutInflationPerResource(this.laborPricingDetails);
    const backgroundParam = <IBackgroundParam>{
      pipSheetId: this.pipSheetId,
      isMarginSet: isMarginSet,
      which: which,
      marginPercent: marginPercent,
      isInitLoad: false,
      inflatedCappedCost: this.totalOfCappedCost,
      totalInflation: totalInflation
    };
    this.labourPricingService.getBackgroundCalculations(backgroundParam).subscribe(backgroundCal => {
      this.backgroundCalValues = backgroundCal;
      laborPricingForm.controls.forEach((control, index) => {
        this.showRatePerHr(control.value, index);
        this.showCostPerHr(control.value, index, false);
        this.calculateMarginPercent(control.value.totalRevenue, control.value.cappedCost);
        this.computeTotalofTotals();
      });

    });
  }

  marginPercentValidationMsg() {
    if (this.laborPricingForm.controls.marginPercent.value >= 100 && this.laborPricingForm.controls.isSetMargin.value) {
      this.message.text = 'Margin cannot be calculated at 100% or above';
      this.message.type = 'error';
    }
    else {
      this.message.text = '';
      this.message.type = 'success';
    }
  }

  private getLaborPricingFormArray(): FormArray {
    const laborPricingArray = this.laborPricingForm.controls.laborPricing as FormArray;
    return laborPricingArray;
  }



  private onUtilizationchange(rowData: FormGroup, index: number) {
    this.showRatePerHr(rowData.value, index);
    this.showCostPerHr(rowData.value, index, false);
    this.calculateMarginPercent(rowData.value.totalRevenue, rowData.value.cappedCost);
    this.computeTotalofTotals();
  }

  calculateOnSwitch() {
    const laborPricingForm = this.laborPricingForm.get('laborPricing') as FormArray;


    const isMarginSet = isNullOrUndefined(this.laborPricingForm.controls.isSetMargin.value) ? false :
      this.laborPricingForm.controls.isSetMargin.value;

    const which: number = isNullOrUndefined(this.laborPricingForm.controls.which.value)
      ? 0 : this.laborPricingForm.controls.which.value.id;

    const marginPercent = parseFloat(isNullOrUndefined(this.laborPricingForm.controls.marginPercent) ? 0
      : this.laborPricingForm.controls.marginPercent.value === '' ? 0 : this.laborPricingForm.controls.marginPercent.value);

    let totalInflation: number;
    laborPricingForm.controls.forEach((control, index) => {
      this.showCostPerHr(control.value, index, false);
    });
    totalInflation = this.labourPricingService.computeTotalCappedCostWithoutInflationPerResource(this.laborPricingDetails);
    const backgroundParam = <IBackgroundParam>{
      pipSheetId: this.pipSheetId,
      isMarginSet: isMarginSet,
      which: which,
      marginPercent: marginPercent,
      isInitLoad: false,
      inflatedCappedCost: this.totalOfCappedCost,
      totalInflation: totalInflation
    };
    this.labourPricingService.getBackgroundCalculations(backgroundParam).subscribe(backgroundCal => {
      this.backgroundCalValues = backgroundCal;
      laborPricingForm.controls.forEach((control, index) => {
        this.showRatePerHr(control.value, index);
        this.showCostPerHr(control.value, index, false);
        this.calculateMarginPercent(control.value.totalRevenue, control.value.cappedCost);
        this.computeTotalofTotals();
      });
    });
  }


  private calculateMarginPercent(totalRevenue: number, cappedCost: number): number {
    return this.labourPricingService.calculateMarginPercent(totalRevenue, cappedCost);
  }

  computeTotalofTotals() {
    let data = [];
    let totalOfCappedCost = 0;

    if (this.isDeliveryTypeRestricted) {
      data = this.laborPricingForm.value.laborPricing;
      data.forEach(labor => {
        totalOfCappedCost = totalOfCappedCost + labor.cappedCost;
      });
      this.totalOfCappedCost = totalOfCappedCost;
      this.totalOfMarginPercent = 0;
    } else {

      let totalOfTotalRevenue = 0;

      data = this.laborPricingForm.value.laborPricing;
      data.forEach(labor => {
        totalOfTotalRevenue = totalOfTotalRevenue + labor.totalRevenue;
        totalOfCappedCost = totalOfCappedCost + labor.cappedCost;
      });

      this.totalOfTotalRevenue = totalOfTotalRevenue;
      this.totalOfCappedCost = totalOfCappedCost;

      this.totalOfMarginPercent = this.computeTotalMargin(this.totalOfTotalRevenue, this.totalOfCappedCost);

      this.computePeriodWiseTotal();
    }

  }

  computePeriodWiseTotal() {
    let data = [];
    data = this.laborPricingForm.value.laborPricing;

    for (let index = 0; index < this.laborPricingDetails.projectPeriodDTO.length; index++) {
      if (this.isDeliveryTypeRestricted) {
        this.totalRevenue[index] = '0';
      } else {
        let totalResourcePeriod = 0;
        for (let i = 0; i < data.length; i++) {
          for (let j = index; j < data[i].periods.length; j++) {
            totalResourcePeriod = totalResourcePeriod +
              data[i].periods[j].revenue + data[i].periods[j].priceAdjustment;
            break;
          }
        }
        this.totalRevenue[index] = totalResourcePeriod.toFixed(2);
      }
    }
  }

  computeTotalStaffHours(data: ILabor[]) {
    this.staffHours = this.labourPricingService.computeTotalStaffHours(data);
  }

  computeTotalMargin(totalRevenue: number, cappedCost: number) {
    const marginPercentage = this.labourPricingService
      .computeTotalMargin(totalRevenue, cappedCost);
    return marginPercentage;
  }

  // Inflation work :

  checkInflation(): boolean {
    // return this.labourPricingService.checkInflation(this.laborPricingDetails);
    const ProjectYears = this.getYearsinProject();
    let isApplicable = false;
    const currentYear = new Date().getFullYear();
    const totalYears = ProjectYears.length;
    const endYear = ProjectYears[totalYears - 1].year;


    if (ProjectYears.length > 0) {
      if (currentYear < endYear) {
        isApplicable = true;
      }
    }
    return isApplicable;
  }

  getYearsinProject() {
    const years = this.laborPricingDetails.projectPeriodDTO.filter(
      (period, index, periods) => periods.findIndex(t => t.year === period.year) === index
    );

    return years.sort();
  }

  getInflationRateOfLocation(locationId: number): number {
    return this.sharedDataService.sharedData.locationDTO.find(x => x.locationId === locationId).inflationRate;
  }

  applyInflationForYear1Cost(locationId: number, year1cost: number, laborPricing: any, index: number): number {
    // check accross the project duration if atleast one FTE is assigned against that resource.
    if (laborPricing.totalHoursPerResource !== 0) {
      // apply year wise inflation
      this.inflationDetailsPerResource = this.computeYearWiseInflationYear1cost(locationId, parseFloat(year1cost.toFixed(2)));

      // START: To calculate Cost Rate Per Resoource Per Period
      this.setCellWiseCostRate(index, (this.inflationDetailsPerResource.length > 0
        ? this.inflationDetailsPerResource[0].year1Cost : year1cost));
      // END

      return (this.inflationDetailsPerResource.length > 0 ? this.inflationDetailsPerResource[0].year1Cost : year1cost);
    } else {

      // START: To calculate Cost Rate Per Resoource Per Period
      this.setCellWiseCostRate(index, year1cost);
      // END

      return year1cost;
    }
  }

  isFTEAllotedForMinimum1Period(locationId: number): boolean {
    let flag = false;
    this.laborPricingDetails.resourceLaborPricingDTOs.forEach(resource => {
      if (resource.locationId === locationId) {
        flag = resource.totalHoursPerResource !== 0 ? true : false;
      }
    });

    return flag ? true : false;
  }

  computeYearWiseInflationYear1cost(locationId: number, currentYear1Cost: number) {
    const currentYear = new Date().getFullYear();
    const inflationDetails = [];
    let inflation = 0;
    const projectDuration = this.getYearsinProject();
    const inflationRate = this.getInflationRateOfLocation(locationId);
    let updatedYear1Cost = 0;
    const totalYears = projectDuration.length;
    let diffYears = [];

    if (currentYear !== projectDuration[0].year) {
      diffYears = this.computeDiffrentialYears(currentYear, projectDuration[0].year, projectDuration[totalYears - 1].year);
    }
    if (diffYears.length > 0) {
      if (currentYear !== projectDuration[0].year) {
        // no change in current year 1 cost

      } else {
        // derivating the previous years inflations
        for (let i = 0; i < diffYears.length; i++) {
          inflation = 0;
          if (i === 0) {
            updatedYear1Cost = currentYear1Cost + inflation;
          } else {
            inflation = (inflationRate / 100) * currentYear1Cost;
            updatedYear1Cost = currentYear1Cost + inflation;
          }
          currentYear1Cost = updatedYear1Cost;
        }
      }
    }

    // computation based on project start year
    projectDuration.forEach((particularYear) => {

      if (particularYear.year >= currentYear) {

        //  starts here

        inflation = 0;

        // first item project duration indicates -> project start year
        if (currentYear === particularYear.year) {
          updatedYear1Cost = currentYear1Cost + inflation;

        } else {
          // if (this.isEvent && index === 0) {
          //   updatedYear1Cost = currentYear1Cost;
          // } else {
          inflation = (inflationRate / 100) * currentYear1Cost;
          updatedYear1Cost = currentYear1Cost + inflation;
          //  }
        }
        inflationDetails.push({
          year: particularYear.year,
          inflation: inflation,
          year1Cost: updatedYear1Cost
        });

        currentYear1Cost = updatedYear1Cost;
      } // ends here
    });
    return inflationDetails;
  }

  // Assuming project start year will never be before current year
  computeDiffrentialYears(currentYear: number, projectStartYear: number, projectEndYear: number) {
    const diff = projectStartYear - currentYear;
    const diffrentialYear = [];
    if (diff > 0) { // project start year is greater than current year
      for (let i = 1; i <= diff; i++) {
        diffrentialYear.push(projectStartYear - i);
      }
    } else { // project start year is smaller than current year
      if (currentYear < projectEndYear) {
        // calculation should be done by current year  as a base
        // Inflation should be applied
        const CurEndYearDiff = projectEndYear - currentYear;
        diffrentialYear.push(projectEndYear);

        for (let i = 1; i < CurEndYearDiff; i++) {
          diffrentialYear.push(projectEndYear - i);
        }
      }
    }
    return diffrentialYear;
  }


  // Rate Per Hour Inflation Work :


  applyInflationForRatePerHour(locationId: number, ratePerHour: number): number {
    // check accross the project duration if atleast one FTE is assigned against that resource.
    const rate = this.computeYearWiseInflationRatePerHour(locationId, ratePerHour);
    return rate;
  }

  computeYearWiseInflationRatePerHour(locationId: number, ratePerHour: number): number {

    const currentYear = new Date().getFullYear();
    let inflation = 0;
    const projectDuration = this.getYearsinProject();
    const inflationRate = this.getInflationRateOfLocation(locationId);
    let updatedRatePerhour = 0;
    const totalYears = projectDuration.length;


    //  if current year and Project start year is same ratePerHour is not applied.

    if (currentYear >= projectDuration[0].year) {
      updatedRatePerhour = ratePerHour;
    } else {
      const diffYears = this.computeDiffrentialYears(currentYear, projectDuration[0].year, projectDuration[totalYears - 1].year);
      if (diffYears.length > 0) {
        // derivating the previous years inflations
        for (let i = 0; i < diffYears.length; i++) {
          inflation = 0;
          inflation = (inflationRate / 100) * ratePerHour;
          updatedRatePerhour = ratePerHour + inflation;
          ratePerHour = updatedRatePerhour;
        }
      }
    }
    return updatedRatePerhour;
  }

  getOverrideNotificationStatus() {
    let overrideNotification: OverrideNotificationStatus;
    this.userWorkflowService.getOverrideNotificationStatus(+this.pipSheetId).subscribe(item => {
      overrideNotification = item;
      if (overrideNotification.clientPrice || overrideNotification.riskManagement
        || overrideNotification.vacationAbsence || overrideNotification.ebitdaStdOverhead) {
        this.notificationService.showNotificationDialog(this.pipSheetId);
      }
    });
  }

  setCellWiseCappedCost(resourceIndex: number, periodIndex: number, cappedCost: number) {
    const laborForm: FormGroup = (<FormArray>this.laborPricingForm.controls['laborPricing']).controls[resourceIndex] as FormGroup;
    const periods: FormGroup[] = (<FormArray>laborForm.controls['periods']).controls as FormGroup[];
    periods[periodIndex].controls['cappedCost'].setValue(cappedCost);
  }

  setCellWiseCostRate(resourceIndex: number, year1Cost: number) {
    const laborForm = (<FormArray>this.laborPricingForm.controls['laborPricing']).controls[resourceIndex] as FormGroup;
    const periods: FormGroup[] = (<FormArray>laborForm.controls['periods']).controls as FormGroup[];

    if (this.isInflationApplicable) {
      // Set Year Wise Inflated Cost Rates
      periods.forEach((period, index) => {
        const periodCostRate = isNullOrUndefined(this.inflationDetailsPerResource
          .find(x => x.year === this.laborPricingDetails.projectPeriodDTO[index].year))
          ? (this.inflationDetailsPerResource.length > 0 ? this.inflationDetailsPerResource[0].year1Cost : year1Cost)
          : this.inflationDetailsPerResource.find(x => x.year === this.laborPricingDetails.projectPeriodDTO[index].year).year1Cost;
        period.controls['costRate'].setValue(periodCostRate);
      });
    } else {
      // Assign the same value Year1Cost to all the periods
      periods.forEach((period, index) => {
        period.controls['costRate'].setValue(year1Cost);
      });
    }
  }

  calculateCellWiseBillRate(laborPricing, resourceIndex: number) {
    const laborForm = (<FormArray>this.laborPricingForm.controls['laborPricing']).controls[resourceIndex] as FormGroup;
    const periods: FormGroup[] = (<FormArray>laborForm.controls['periods']).controls as FormGroup[];
    const isBillable = laborPricing.utilizationType.id;
    let yr1Cost = 0;
    let computedRatePerHour = 0;

    // When IsMarginSet = True, then inflation is not applied; IsMarginSet = False, then Inflation is Applied.
    if (this.isDeliveryTypeRestricted) {
      this.setCellWiseBillRateZero(periods);
    } else {
      if (this.isMarginSet) {
        if (isBillable) {
          // Impact of Year 1 Cost Rate
          if ((!isNullOrUndefined(laborPricing.cost) && laborPricing.cost !== '') && +laborPricing.cost >= 0) {
            yr1Cost = +laborPricing.cost;
          } else {
            yr1Cost = laborPricing.yearCostPerHour;
          }

          // Calculation of Bill Rate when Set Margin = YES
          const x = (yr1Cost * this.backgroundCalValues.g13) + this.backgroundCalValues.g14;
          const y = (x / this.backgroundCalValues.g15) / (this.staffHours === 0 ? 1 : this.staffHours);
          computedRatePerHour = y / this.backgroundCalValues.g16;

          periods.forEach((period, index) => {
            period.controls['billRate'].setValue(computedRatePerHour);
          });

        } else {
          this.setCellWiseBillRateZero(periods);
        }
      }
      else {
        if (isBillable) {
          // Calculation of Bill Rate when Set Margin = NO
          if ((!isNullOrUndefined(laborPricing.rate) && laborPricing.rate !== '') && +laborPricing.rate >= 0) {
            computedRatePerHour = +laborPricing.rate;
            periods.forEach((period, index) => {
              period.controls['billRate'].setValue(+laborPricing.rate);
            });
          } else {
            const corpBillingRate = this.sharedDataService.sharedData.corpBillingRateDTO.filter(cbr => {
              return cbr.locationId === laborPricing.locationId && cbr.resourceId === laborPricing.resourceId;
            })[0].rate;
            if (this.checkInflation()) {
              computedRatePerHour = this.applyInflationForRatePerHour(laborPricing.locationId, corpBillingRate);
            } else {
              computedRatePerHour = corpBillingRate;
            }
            periods.forEach((period, index) => {
              period.controls['billRate'].setValue(computedRatePerHour);
            });
          }
        } else {
          this.setCellWiseBillRateZero(periods);
        }
      }
    }
  }

  setCellWiseBillRateZero(periods: FormGroup[]) {
    periods.forEach((period, index) => {
      period.controls['billRate'].setValue(0);
    });
  }
}
