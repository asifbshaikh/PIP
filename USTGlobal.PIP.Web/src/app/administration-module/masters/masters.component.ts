import { Component, OnInit } from '@angular/core';
import { RoutesRecognized, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { MasterService } from '@shared/services/master.service';
import { ILocation } from '@shared';
import { SelectItem } from 'primeng/api';

@Component({
  selector: 'app-masters',
  templateUrl: './masters.component.html'
})
// This complete component may need refactoring
export class MastersComponent implements OnInit {

  masterCols: any[] = [];
  adminDropdownItems: SelectItem[];
  masterId: number;
  breadcrumbUrl: string;
  showDialog = false;
  location: ILocation;
  savedLocation: ILocation;

  constructor(
    private router: Router,
    private masterService: MasterService,
    private translateService: TranslateService,
  ) {
  }

  ngOnInit() {
    this.translateService.get('MasterDropdown.masterDropdownItem').subscribe((data) => {
      this.adminDropdownItems = data;
      this.onDropdownChange(this.adminDropdownItems[0].value);
    });
    this.router.events.subscribe((event) => {
      if (event instanceof RoutesRecognized) {
        this.breadcrumbUrl = event.url;
      }
    });
  }

  onDropdownChange(masterId: number) {
    this.masterId = masterId;
  }

  onRefreshClick(event) {
  }

  onAddNewMaster() {
    this.showDialog = true;
    this.location = this.getLocation();
  }

  // write this method in service
  getLocation(): ILocation {
    return {
      comments: '',
      countryID: 0,
      ebitdaSeatCost: 0,
      endDate: null,
      hoursPerDay: 0,
      hoursPerMonth: 0,
      inflationRate: 0,
      isActive: false,
      locationId: 0,
      locationName: '',
      refUSD: 0,
      startDate: null,
      status: 4,
      isDeleted: false
    };
  }

  onSave($event) {
    this.savedLocation = $event;
  }


}
