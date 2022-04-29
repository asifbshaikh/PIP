import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { MarginSteps, Constants } from '@shared';
import { TranslateService } from '@ngx-translate/core';
import { NotificationService, SharedDataService } from '@global';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-projects-margin',
  templateUrl: './projects-margin.component.html'
})
export class ProjectsMarginComponent implements OnInit {

  stepPageIndex: any;
  stepLabels = [];
  isDataAvailable = false;
  isLoading = false;
  pipsheetId: number;
  constructor(
    public translate: TranslateService,
    private activateRoute: ActivatedRoute,
    private sharedDataService: SharedDataService,
    private location: Location

  ) {
    this.activateRoute.paramMap.subscribe(
      params => {
        this.pipsheetId = parseInt(params.get(Constants.uiRoutes.routeParams.pipSheetId), 10);
      });
    this.isLoading = true;
  }

  ngOnInit() {
    this.translate.get('PLANNING.Staff.MarginSteps').subscribe((resource) => {
      this.stepLabels = resource;
      this.isDataAvailable = true;
    });
    this.activateRoute.paramMap.subscribe(params => {
      this.sharedDataService.populateCommonData(this.pipsheetId).then(x => {
        this.isLoading = false;
        this.stepPageIndex = +params.get('tabIndex');
        this.location.replaceState(this.location.path().split(';')[0], '');
      });
    });
  }
}
