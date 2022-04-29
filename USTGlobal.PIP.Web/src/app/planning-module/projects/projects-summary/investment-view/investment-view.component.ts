import { UserWorkflowService } from './../../../../shared-module/services/user-workflow.service';
import { IWorkflowFlag } from './../../../../shared-module/domain/IWorkflowFlag';
import { IPipSheetWorkflowStatus } from './../../../../shared-module/domain/IPipSheetWorkflowStatus';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators, FormControl } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { SummaryService } from '@shared/services/summary.service';
import { Mastermapper, NegativeValuePipe, IInvestPercent, Constants } from '@shared';
import { SharedDataService, NotificationService } from '@global';
import { IRoleAndAccount } from '@shared/domain/IRoleAndAccount';
import { ICheckRole } from '@shared/domain/ICheckRole';

@Component({
  selector: 'app-investment-view',
  templateUrl: './investment-view.component.html'
})
export class InvestmentViewComponent implements OnInit {
  @Output() investmentLoad = new EventEmitter();
  investmentLoaded = false;
  investmentViewForm: FormGroup;
  translationData: any = {};
  investmentForm: FormArray;
  isCurrencyConverted = false;
  pipSheetId: number;
  investmentViewData: any[];
  dropdownList: any[] = [];
  selectedCorporatePercent: any = { label: '', value: { code: 0, id: 0 } };
  calculatedCorporatePercent: any;
  calculatedNetInvestment: any;
  saveInvestPercent: IInvestPercent = { pipSheetId: 0, corporateTarget: 0 };
  convertedCurrency: any[] = [];
  currencySymbol: string;
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  loggedInUserId: number;
  roleAndAccount: IRoleAndAccount[];
  checkRole: ICheckRole;
  dashboardId: number;
  submitClick = false;
  isDataAvailable = false;

  constructor(
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private sharedData: SharedDataService,
    private translateService: TranslateService,
    private summaryService: SummaryService,
    private userWorkflowService: UserWorkflowService,
    private notificationService: NotificationService
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
      this.dashboardId = parseInt(data['params'].dashboardId, 10);
    });
  }

  ngOnInit() {
    this.initializeForm();
    this.translateService.get('ProjectSummary').subscribe((data) => {
      this.translationData = data;
      this.getInvestmentViewLabels();
      this.getInvestMentViewData();
      this.currencySymbol = this.sharedData.sharedData.currencyDTO.find(sym => sym.currencyId ===
        this.sharedData.sharedData.currencyId).symbol;
      this.roleAndAccount = this.sharedData.roleAndAccount;
      if (this.roleAndAccount != null) {
        this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
      }
    });
    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });
    this.pipSheetWorkflowStatus = this.sharedData.sharedData.pipSheetWorkflowStatus;
    this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus[0], this.wfstatus);
    this.loggedInUserId = this.sharedData.sharedData.userRoleAccountDTO.userId;
    this.notificationService.submitClickNotification.subscribe((submitClick) => {
      this.submitClick = submitClick;
    });
    this.isDataAvailable = true;
    this.investmentLoaded = true;
    this.investmentLoad.emit(this.investmentLoaded);
  }

  initializeForm() {
    this.investmentViewForm = this.fb.group({
      corporateTargetPercent: new FormControl()
    });
  }

  getInvestmentViewLabels() {
    this.investmentViewData = [];
    this.investmentViewData = JSON.parse(JSON.stringify(this.translationData.investmentViewLabels));
  }

  getInvestMentViewData() {
    this.summaryService.getInvestmentData(this.pipSheetId).subscribe((result) => {
      this.convertedCurrency = [];
      if (result.length > 1) {
        this.isCurrencyConverted = true;
        if (result[0]) {
          this.dropdownList = new Mastermapper().getInvestmentPercentage(result[0]['corporateTarget']);
          this.getCorporatePercent(result[0]);
          this.investmentViewData.forEach((value, key) => {
            this.calculatedCorporatePercent =
              (result[0].investmentView.totalProjectCost / (1 - this.selectedCorporatePercent.value.code * 0.01)).toFixed(2);
            if (value['field'].toUpperCase() === Constants.InvestMentView.totalClientPrice) {
              value.value = result[0].investmentView[value['field']].toFixed(2);
              this.convertedCurrency.push(value.value);
            }
            if (value['field'].toUpperCase() === Constants.InvestMentView.corporateTarget) {
              value.value = this.calculatedCorporatePercent;
              this.convertedCurrency.push(value.value);
            }
            if (value['field'].toUpperCase() === Constants.InvestMentView.netInvestment) {
              if (this.calculatedCorporatePercent < result[0].investmentView.totalClientPrice) {
                value.value = this.translationData.investmentViewNoneNeeded;
                this.convertedCurrency.push(value.value);
              } else {
                this.calculatedNetInvestment =
                  ((((this.selectedCorporatePercent.value.code * 0.01) * result[0].investmentView.totalClientPrice) -
                    result[0].investmentView.totalClientPrice) + (result[0].investmentView.totalProjectCost)).toFixed(2);
                value.value = this.calculatedNetInvestment;
                this.convertedCurrency.push(value.value);
              }
            }
          });
          this.bindFormData();
        }
        if (result[1]) {
          this.dropdownList = new Mastermapper().getInvestmentPercentage(result[1]['corporateTarget']);
          this.getCorporatePercent(result[1]);
          this.investmentViewData.forEach((value, key) => {
            this.calculatedCorporatePercent =
              (result[1].investmentView.totalProjectCost / (1 - this.selectedCorporatePercent.value.code * 0.01)).toFixed(2);
            if (value['field'].toUpperCase() === Constants.InvestMentView.totalClientPrice) {
              value.value = result[1].investmentView[value['field']].toFixed(2);
            }
            if (value['field'].toUpperCase() === Constants.InvestMentView.corporateTarget) {
              value.value = this.calculatedCorporatePercent;
            }
            if (value['field'].toUpperCase() === Constants.InvestMentView.netInvestment) {
              if (this.calculatedCorporatePercent < result[1].investmentView.totalClientPrice) {
                value.value = this.translationData.investmentViewNoneNeeded;
              } else {
                this.calculatedNetInvestment =
                  ((((this.selectedCorporatePercent.value.code * 0.01) * result[1].investmentView.totalClientPrice) -
                    result[1].investmentView.totalClientPrice) + (result[1].investmentView.totalProjectCost)).toFixed(2);
                value.value = this.calculatedNetInvestment;
              }
            }
          });
          this.bindFormData();
        }
      }
      else {
        if (result[0]) {
          this.dropdownList = new Mastermapper().getInvestmentPercentage(result[0]['corporateTarget']);
          this.getCorporatePercent(result[0]);
          this.investmentViewData.forEach((value, key) => {
            this.calculatedCorporatePercent =
              (result[0].investmentView.totalProjectCost / (1 - this.selectedCorporatePercent.value.code * 0.01)).toFixed(2);
            if (value['field'].toUpperCase() === Constants.InvestMentView.totalClientPrice) {
              value.value = result[0].investmentView[value['field']].toFixed(2);
            }
            if (value['field'].toUpperCase() === Constants.InvestMentView.corporateTarget) {
              value.value = this.calculatedCorporatePercent;
            }
            if (value['field'].toUpperCase() === Constants.InvestMentView.netInvestment) {
              if (this.calculatedCorporatePercent < result[0].investmentView.totalClientPrice) {
                value.value = this.translationData.investmentViewNoneNeeded;
              } else {
                this.calculatedNetInvestment =
                  ((((this.selectedCorporatePercent.value.code * 0.01) * result[0].investmentView.totalClientPrice) -
                    result[0].investmentView.totalClientPrice) + (result[0].investmentView.totalProjectCost)).toFixed(2);
                value.value = this.calculatedNetInvestment;
              }
            }
          });
          this.bindFormData();
        }
      }
    });
    //    this.isDataAvailable = true;
  }

  getCorporatePercent(res) {
    this.selectedCorporatePercent = this.dropdownList.find((data) => {
      if (res.investmentView.corporateTarget !== 0) {
        return data.value.id === res.investmentView.corporateTarget;
      }
      return data.value.id === 3;
    });
  }

  bindFormData() {
    this.investmentViewForm.get('corporateTargetPercent').setValue(this.selectedCorporatePercent.value);

  }

  changeOfDropDown(event) {
    this.saveInvestPercent.pipSheetId = this.pipSheetId;
    this.saveInvestPercent.corporateTarget = event.id;
    this.summaryService.saveInvestmentPercent(this.saveInvestPercent).subscribe((res) => {
      this.getInvestMentViewData();
    });
  }


}
