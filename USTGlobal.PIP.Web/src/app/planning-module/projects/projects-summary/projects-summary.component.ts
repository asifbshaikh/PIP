import { Location } from '@angular/common';
import { IRoleAndAccount } from './../../../shared-module/domain/IRoleAndAccount';
import { ICheckRole } from './../../../shared-module/domain/ICheckRole';
import { IPipSheetWorkflowStatus } from './../../../shared-module/domain/IPipSheetWorkflowStatus';
import { IWorkflowFlag } from './../../../shared-module/domain/IWorkflowFlag';
import { Constants, ISubmitPipSheet, IplForcast } from '@shared';
import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { IUsers } from '@shared/domain/IUsers';
import { SharedDataService } from '@global';

@Component({
  selector: 'app-projects-summary',
  templateUrl: './projects-summary.component.html'
})
export class ProjectsSummaryComponent implements OnInit {
  pipSheet: ISubmitPipSheet;
  sharePIPSheetFrom: FormGroup;
  pipSheetId: number;
  projectId: number;
  accountId: number;
  dashboardId: number;
  comments: string;
  approverComments: string;
  public href = '';
  version: string;
  isLoading = false;
  isValid = false;
  plForecastData: IplForcast;
  stepPageIndex: any;
  stepLabels = [];
  pipsheetId: number;

  @Input()
  get pipURL() {
    const str = this.router.url;
    const splitted = str.split('/', 7);
    splitted[splitted.length - 1] = 'Staff';
    const url = splitted.join('/');
    this.href = location.origin + url;
    return this.href;
  }
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private fb: FormBuilder,
    private translateService: TranslateService,
    private sharedData: SharedDataService,
    private activateRoute: ActivatedRoute,
    private sharedDataService: SharedDataService,
    private location: Location
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
      this.projectId = parseInt(data['params'].projectId, 10);
      this.accountId = parseInt(data['params'].accountId, 10);
      this.dashboardId = parseInt(data['params'].dashboardId, 10);
    }),
      this.activateRoute.paramMap.subscribe(
        params => {
          this.pipsheetId = parseInt(params.get(Constants.uiRoutes.routeParams.pipSheetId), 10);
        });
    // this.isLoading = true;
  }

  ngOnInit() {
    // this.activateRoute.queryParamMap.subscribe(params => {
    //   // this.sharedDataService.populateCommonData(this.pipsheetId).then(x => {
    //     this.stepPageIndex = +params.get('tabIndex');
    //     this.location.replaceState(this.location.path().split(';')[0], '');
    //   });
    // });
    this.stepPageIndex = 0;
    // this.isLoading = false;
    this.translateService.get('PLANNING.Staff.SummarySteps').subscribe((resource) => {
      this.stepLabels = resource;
    });

  }

  validateComments(comments) {
    // Comments Should be mandatory
    if (comments) {
      this.isValid = true;
    }
    else {
      this.isValid = false;
    }
  }
}
