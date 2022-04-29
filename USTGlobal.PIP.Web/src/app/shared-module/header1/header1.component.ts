import { IPIPSheetComment } from './../domain/IPIPSheetComment';
import { PipsheetCommentsService } from './../services/pipsheet-comments.service';
import { UserWorkflowService } from './../services/user-workflow.service';
import { ICheckRole } from './../domain/ICheckRole';
import { IRoleAndAccount } from './../domain/IRoleAndAccount';
import { IWorkflowFlag } from './../domain/IWorkflowFlag';
import { IPipSheetWorkflowStatus } from './../domain/IPipSheetWorkflowStatus';
import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { Router, ActivatedRoute, RoutesRecognized, NavigationEnd } from '@angular/router';
import { Constants } from '../infrastructure/index';
import { SharedDataService, NotificationService } from '@global';
import { SelectItem, MessageService, MenuItem } from 'primeng/api';
import { Mastermapper } from '@shared/mapper/master/mastermapper';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ICurrency } from '@shared/domain/ICurrency';
import { DateService } from '@core/services/date.service';
import { ProjectService } from '@shared/services/project.service';
import { BreadcrumbService } from '@shared/breadcrumb/breadcrumb.service';
import { IHeader1 } from '@shared/domain/IHeader1';
import { Breadcrumb } from '@shared/breadcrumb/breadcrumb.model';
import { TranslateService } from '@ngx-translate/core';
import * as moment from 'moment';
import { IPipCheckIn } from '@shared/domain';
import { IPipOverride } from '@shared/domain/IPipOverride';

@Component({
  selector: 'app-header1',
  templateUrl: './header1.component.html'
})
export class Header1Component implements OnInit, AfterViewInit {
  pipCheckIn: IPipCheckIn;
  isDataAvailable = false;
  projectComponents: Array<{ projectCompType: string, projectCompPath: string }> = [];
  projectId: number;
  projectDuration: string;
  projectName: string;
  sfProjectId: string;
  pipSheetId: number;
  accountId: number;
  currencyControl: FormGroup;
  countries: SelectItem[];
  selectedCurrency: ICurrency;
  activeTabIndex = 0;
  tabMenuItems: MenuItem[];
  activeTabMenuItem: MenuItem;
  breadcrumbUrl: string;
  currency: string;
  totalClientPrice: string;
  ebitdaPercent: string;
  accountName: string;
  version: string;
  totalProjectCost: string;
  info: [];
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  roleAndAccount: IRoleAndAccount[];
  checkRole: ICheckRole;
  loggedInUserId: number;
  userName: string;
  dashboardId: number;
  isNewVersionAllowed = false;
  submitClick = false;
  resendClick = false;
  isDirty: boolean;
  displayPosition: boolean;
  position: string;
  displayBasic: boolean;
  displayMaximizable: boolean;
  comment: string;
  msgs: any;
  chat: [];
  count = 0;
  pipsheetComments: IPIPSheetComment[];
  overrideCols: any[] = [];
  displayOverride: boolean;
  overriddenItems: IPipOverride;
  isDummy: boolean;

  @ViewChild('tabData') tabData: any;

  isProjectApprovalPending: boolean;
  isProjectApproved: boolean;
  isProjectNotSubmitted: boolean;
  isAnyVersionApproved = false;
  @ViewChild('scroll') scroll: any;


  constructor
    (
      private router: Router,
      private activatedRoute: ActivatedRoute,
      private notificationService: NotificationService,
      private sharedDataService: SharedDataService,
      private fb: FormBuilder,
      private dateService: DateService,
      private projectService: ProjectService,
      private messageService: MessageService,
      private breadcrumbService: BreadcrumbService,
      private translateService: TranslateService,
      private userWorkflowService: UserWorkflowService,
      private pipsheetCommentsService: PipsheetCommentsService
    ) {
  }
  ngAfterViewInit(): void {
    this.notificationService.isFormDirty.subscribe(isDirty => {
      this.isDirty = isDirty;
    });
    if (this.scroll) {
      this.scroll.nativeElement.scrollTo(0, this.scroll.nativeElement.scrollHeight);
    }
  }


  ngOnInit() {
    this.translateService.get('SHARED.HEADER1.overrideColumns').subscribe(cols => {
      this.overrideCols = cols;
    });
    const userRoleDTO = this.sharedDataService.sharedData.userRoleAccountDTO;
    this.router.events.subscribe((event) => {
      if (event instanceof RoutesRecognized) {
        this.projectId = event.state.root.firstChild.params.projectId;
        this.pipSheetId = event.state.root.firstChild.params.pipSheetId;
        this.accountId = event.state.root.firstChild.params.accountId;
        this.dashboardId = parseInt(event.state.root.firstChild.params.dashboardId, 10);
        this.pipSheetId = this.pipSheetId !== undefined ? this.pipSheetId : 0;
        this.breadcrumbUrl = event.url;
        this.populateProjectDuration();
        this.getRoleandWorkflowStatusData();
        if (this.pipSheetId === 0) {
          this.displayPosition = false;
          this.displayOverride = false;
        }
      }
      if (event instanceof NavigationEnd) {
        this.setActiveTabMenuItem();
      }
    });

    this.tabMenuItems = [
      {
        label: Constants.projectTabMenuItems.staff,
        command: this.navigateTo(Constants.uiRoutes.staff)
      },
      {
        label: Constants.projectTabMenuItems.margin,
        command: this.navigateTo(Constants.uiRoutes.margin)
      },

      {
        label: Constants.projectTabMenuItems.summary,
        command: this.navigateTo(Constants.uiRoutes.summary)
      }

    ];

    this.userName = userRoleDTO.firstName + ' ' + userRoleDTO.lastName;

    this.setActiveTabMenuItem();

    this.translateService.get('ProjectHeader.info').subscribe(info => {
      this.info = info;
    });
    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });
    this.notificationService.durationExistsNotification.subscribe((projectDuration) => {
      this.projectDuration = projectDuration;
    });
    this.notificationService.projectNameExistsNotification.subscribe((projectName) => {
      this.projectName = projectName;
    });
    this.notificationService.sfProjectIdExistsNotification.subscribe((sfProjectId) => {
      this.sfProjectId = sfProjectId;
    });
    this.notificationService.currencyDataChangeNotification.subscribe((currency) => {
      this.currency = currency;
    });
    this.notificationService.totalClientPriceExistsNotification.subscribe((tcp) => {
      this.totalClientPrice = tcp;
    });
    this.notificationService.percentEbitdaExistsNotification.subscribe((percentEbitda) => {
      this.ebitdaPercent = percentEbitda;
    });
    this.notificationService.submitClickNotification.subscribe((submitClick) => {
      this.submitClick = submitClick;
    });
    this.notificationService.resendClickNotification.subscribe((resendClick) => {
      this.resendClick = resendClick;
    });
    this.notificationService.isAnyVersionApproved.subscribe((versionApproved) => {
      this.isAnyVersionApproved = versionApproved;
    });
    this.countries = new Mastermapper().getCountryDetails(this.sharedDataService.sharedData.countryDTO,
      this.sharedDataService.sharedData.currencyDTO);
  }

  getPIPSheetComments(position: string) {
    this.position = position;

    // get comments  :
    this.pipsheetCommentsService.GetPIPSheetComments(+this.pipSheetId).subscribe(comments => {
      this.pipsheetComments = comments;
      this.count = this.pipsheetComments.length;
      this.displayPosition = true;
      if (this.router.url === '/projects') {
        this.displayPosition = false;
      }
    });
  }

  SavePIPSheetComment(comment) {
    if ((<string>comment.value).trim() !== '') {

      const pipsheetComment: IPIPSheetComment = {
        comment: comment.value,
        isDeleted: false,
        projectId: this.projectId,
        pIPSheetId: this.pipSheetId,
        userId: this.loggedInUserId,
        pipSheetCommentId: 0,
        userName: '',
        commentTimeStamp: null
      };

      this.pipsheetCommentsService.SavePIPSheetComment(pipsheetComment).subscribe((pipsheetCommentId: number) => {
        pipsheetComment.pipSheetCommentId = pipsheetCommentId;
        pipsheetComment.userName = this.userName;
        pipsheetComment.commentTimeStamp = new Date().toString();
        this.pipsheetComments.push(pipsheetComment);
        this.count += 1;
        comment.value = '';
        setTimeout(() => {
          this.scroll.nativeElement.scrollTo(0, this.scroll.nativeElement.scrollHeight);
        }, 0);
      });
    }
  }

  deleteComment(pipSheetCommentId, index: number, projectId: number) {
    this.pipsheetCommentsService.DeletePIPSheetComment(pipSheetCommentId, projectId).subscribe(result => {
      if (result === true) {
        this.pipsheetComments[index].isDeleted = true;
      }
    });
  }

  showBasicDialog() {
    this.displayBasic = true;
  }

  showMaximizableDialog() {
    this.displayMaximizable = true;
  }

  getRoleandWorkflowStatusData() {
    if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
      this.projectService.getWorkflowStatusAccountRole(this.pipSheetId, this.accountId)
        .subscribe(roleAndWfStatus => {
          this.pipSheetWorkflowStatus = roleAndWfStatus.pipSheetWorkflowStatus;
          this.workflowFlag = this.userWorkflowService.getWorkflowFlag(this.pipSheetWorkflowStatus, this.wfstatus);
          this.roleAndAccount = roleAndWfStatus.roleAndAccountDTO;
          this.loggedInUserId = this.sharedDataService.sharedData.userRoleAccountDTO.userId;
          if (this.roleAndAccount != null) {
            this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
          }
          this.isDataAvailable = true;
        });
    }
    else {
      this.isDataAvailable = true;
    }
  }

  getFormGroupControls() {
    const currencyControls = this.currencyControl.controls;
    return currencyControls;
  }

  getProjectId(): string {
    let projectId = '';
    this.activatedRoute.firstChild.params.subscribe((params: any) => {
      if (params.hasOwnProperty('projectId') !== '') {
        projectId = params.projectId;
      }
    });
    return projectId;
  }

  getPipSheetId(): string {
    let pipSheetId = '';
    this.activatedRoute.firstChild.params.subscribe((params: any) => {
      if (params.hasOwnProperty('pipSheetId') !== '') {
        pipSheetId = params.pipSheetId;
      }
    });
    return pipSheetId;
  }

  navigateTo(path): () => void {
    return (): void => {
      if (!this.isDirty) {

        this.projectId = parseInt(this.getProjectId(), 10);
        this.pipSheetId = parseInt(this.getPipSheetId(), 10);
        if (this.projectId > -1) {
          switch (path) {
            case Constants.uiRoutes.staff:
              this.activeTabIndex = 0;
              break;
            case Constants.uiRoutes.margin:
              this.activeTabIndex = 1;
              break;
            case Constants.uiRoutes.summary:
              this.activeTabIndex = 2;
              break;
          }
          this.router.navigate([this.router.url.includes('samples') ? Constants.uiRoutes.sample :
            Constants.uiRoutes.projects, this.projectId, this.pipSheetId, this.accountId, this.dashboardId, path]);
        }
      } else {
        setTimeout(() => {
          this.tabData.activeItem = this.tabData.model[this.activeTabIndex];
        }, 0);
        this.notificationService.showDialog();
      }
    };
  }

  setCurrencyControls(currencyData: ICurrency) {
    this.selectedCurrency = currencyData;

    this.currencyControl.patchValue({
      localCurrency: currencyData.symbol,
      usdToLocal: currencyData.usdToLocal,
      localToUSD: currencyData.localToUSD,
      currencyId: currencyData.currencyId,
      factorUsed: currencyData.factors,
      country: this.countries.filter(country => country.value.id === currencyData.countryId)[0].value
    });
  }

  populateProjectDuration() {
    if (this.projectId > 0) {
      this.projectService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
        this.setProjectBreadcrumb(headerInfo.header1);
        if (headerInfo.header1 ? ((headerInfo.header1.startDate && headerInfo.header1.endDate) ? true : false) : false) {
          const startDate = new Date(headerInfo.header1.startDate);
          const endDate = new Date(headerInfo.header1.endDate);
          const durationStartDate = new Date(startDate.getDate().toString()
            + ' ' + this.dateService.getMonthName(startDate.getMonth()).toString()
            + ' ' + startDate.getFullYear().toString());
          const durationEndDate = new Date(endDate.getDate().toString() + ' ' + this.dateService.getMonthName(endDate.getMonth()).toString()
            + ' ' + endDate.getFullYear().toString());
          this.projectDuration = moment(durationStartDate).format('MM-DD-YYYY') + '-' + moment(durationEndDate).format('MM-DD-YYYY');
          this.sfProjectId = headerInfo.header1.sfProjectId;
          this.currency = headerInfo.header1.currency;
          this.totalClientPrice = headerInfo.header1.totalClientPrice ? (headerInfo.header1.totalClientPrice.toString() === '0' ?
            '0.00' : headerInfo.header1.totalClientPrice.toString()) : '0.00';
          this.ebitdaPercent = headerInfo.headerEbitda.projectEBITDAPercent ?
            (headerInfo.headerEbitda.projectEBITDAPercent.toString() === '0' ?
              '0.00' : headerInfo.headerEbitda.projectEBITDAPercent.toString()) : '0.00';
        }
        else {
          this.sfProjectId = headerInfo.header1 ? headerInfo.header1.sfProjectId : '';
          this.currency = headerInfo.header1 ? headerInfo.header1.currency : '';
          this.totalClientPrice = headerInfo.header1.totalClientPrice ? (headerInfo.header1.totalClientPrice.toString() === '0' ?
            '0.00' : headerInfo.header1.totalClientPrice.toString()) : '0.00';
          this.ebitdaPercent = headerInfo.headerEbitda == null ? '0.00' : headerInfo.headerEbitda.projectEBITDAPercent ?
            (headerInfo.headerEbitda.projectEBITDAPercent.toString() === '0' ?
              '0.00' : headerInfo.headerEbitda.projectEBITDAPercent.toString()) : '0.00';
        }
        this.isNewVersionAllowed = ((headerInfo.header1.totalVersionsPresent < 5) ? true : false);
      });
    }
    else {
      this.projectDuration = '';
      this.sfProjectId = '';
      this.totalClientPrice = '';
      this.ebitdaPercent = '';
      this.setProjectBreadcrumb();
    }
  }

  setProjectBreadcrumb(headerData?: IHeader1) {
    const isVersionRoute = this.router.url.match(Constants.uiRoutes.versions);

    let newBreadcrumbData: Breadcrumb[];
    if (headerData) {
      newBreadcrumbData = [
        {
          label: Constants.breadcrumbLabels.projects,
          url: Constants.uiRoutes.projects
        },
        {
          label: headerData.projectName,
          url: !isVersionRoute ? `${Constants.uiRoutes.projects}/${this.projectId}/${Constants.uiRoutes.versions}` : ''
        }];
    }
    else {
      newBreadcrumbData = [
        {
          label: Constants.breadcrumbLabels.projects,
          url: Constants.uiRoutes.projects
        }];
    }

    if (!isVersionRoute && headerData) {
      newBreadcrumbData.push({
        label: `Version ${headerData.versionNumber}`,
        url: ''
      });
    }
    this.breadcrumbService.updateBreadcrumb(newBreadcrumbData);
  }

  setActiveTabMenuItem(): void {
    if (this.router.url.match('Staff')) {
      this.activeTabMenuItem = this.tabMenuItems[0];
      this.activeTabIndex = 0;
    } else if (this.router.url.match('Margin')) {
      this.activeTabMenuItem = this.tabMenuItems[1];
      this.activeTabIndex = 1;
    } else if (this.router.url.match('Summary')) {
      this.activeTabMenuItem = this.tabMenuItems[2];
      this.activeTabIndex = 2;
    }
  }

  getInfoData() {
    if (this.projectId > 0) {
      this.projectService.getHeader1Data(this.projectId, this.pipSheetId).subscribe(headerInfo => {
        if (headerInfo.header1 ? ((headerInfo.header1.startDate && headerInfo.header1.endDate) ? true : false) : false) {
          const startDate = new Date(headerInfo.header1.startDate);
          const endDate = new Date(headerInfo.header1.endDate);
          const durationStartDate = new Date(startDate.getDate().toString()
            + ' ' + this.dateService.getMonthName(startDate.getMonth()).toString()
            + ' ' + startDate.getFullYear().toString());
          const durationEndDate = new Date(endDate.getDate().toString() + ' ' + this.dateService.getMonthName(endDate.getMonth()).toString()
            + ' ' + endDate.getFullYear().toString());
          this.projectDuration = moment(durationStartDate).format('MMM YYYY') + '-' + moment(durationEndDate).format('MMM YYYY');
          this.accountName = headerInfo.header1.sfAccountName;
          this.projectName = headerInfo.header1.projectName;
          this.version = headerInfo.header1.versionNumber.toString();
          this.totalProjectCost = headerInfo.headerEbitda ? headerInfo.headerEbitda.totalProjectCost.toString() : '';
        }
        else {
          this.accountName = headerInfo.header1.sfAccountName;
          this.projectName = headerInfo.header1.projectName;
          this.version = headerInfo.header1.versionNumber.toString();
          this.totalProjectCost = headerInfo.headerEbitda ? headerInfo.headerEbitda.totalProjectCost.toString() : '';
        }
        this.isNewVersionAllowed = ((headerInfo.header1.totalVersionsPresent < 5) ? true : false);
      });
    }
    else {
      this.accountName = '';
      this.projectName = '';
      this.version = '';
      this.projectDuration = '';
      this.totalProjectCost = '';
    }
  }

  onCheckInClick() {

    if (!this.isDirty) {
      if (this.pipSheetId > 0 && this.pipSheetId !== undefined) {
        const pipCheckIn: IPipCheckIn = {
          pipSheetId: this.pipSheetId,
          isCheckedOut: true,
          checkedInOutBy: null
        };
        this.projectService.updatePIPSheetCheckIn(pipCheckIn).subscribe((res: number) => {
          this.translateService.get('SuccessMessage.PipCheckIn').subscribe(msg => {
            this.messageService.add({ severity: 'success', detail: msg });
            this.router.navigate([
              this.router.url.includes('samples') ? Constants.uiRoutes.sample : Constants.uiRoutes.projects
            ]);
          },
            () => {
              this.translateService.get('ErrorMessage.PipCheckIn').subscribe(msg => {
                this.messageService.add({ severity: 'error', detail: msg });
              });
            });
        });
      }
    } else {
      this.notificationService.showDialog();
    }
  }

  onOverrideClick() {
    this.displayOverride = true;
    this.projectService.getPipOverride(this.pipSheetId).subscribe(overrideItems => {
      this.overriddenItems = overrideItems;
    });
  }

  navigateToTabIndex(tabindex: number, stepIndex: number) {
    this.isDummy = this.router.url.includes('samples') ? true : false;
    if (tabindex === 0) {
      this.router.navigate([this.isDummy ? Constants.uiRoutes.sample :
        Constants.uiRoutes.projects, this.projectId, this.pipSheetId, this.accountId,
      this.dashboardId, Constants.uiRoutes.staff, { tabIndex: stepIndex }]);
    }
    else {
      this.router.navigate([this.isDummy ? Constants.uiRoutes.sample :
        Constants.uiRoutes.projects, this.projectId, this.pipSheetId, this.accountId,
      this.dashboardId, Constants.uiRoutes.margin, { tabIndex: stepIndex }]);
    }
  }
}

