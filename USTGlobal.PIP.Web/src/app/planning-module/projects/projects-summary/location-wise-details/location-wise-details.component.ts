import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { LocationWiseDetailsService } from '@shared/services/location-wise-details.service';
import { ActivatedRoute } from '@angular/router';
import { ILocationWiseDetailsSummary } from '@shared/domain/ILocationWiseDetailsSummary';
import { ILocationWiseDetails } from '@shared/domain/ILocationWiseDetails';
import { ILocationWisePeriodDetails } from '@shared/domain/ILocationWisePeriodDetails';
import { ISummaryLocations } from '@shared/domain/ISummaryLocations';
import { ResourceMapper } from '@shared';
import { SharedDataService } from '@global';

@Component({
  selector: 'app-location-wise-details',
  templateUrl: './location-wise-details.component.html'
})
export class LocationWiseDetailsComponent implements OnInit {
  cols: any;
  locationCols: any = [];
  locationWiseDetailsForm: FormGroup;
  pipSheetId: number;
  currencySymbol: string;
  translationData: any;
  locationPeriods: ISummaryLocations[];
  selectedRow: any;
  locationWiseDetailsData: ILocationWiseDetailsSummary = {
    locationWiseDetails: [], summaryLocations: []
  };

  constructor(
    private translateService: TranslateService,
    private fb: FormBuilder,
    private sharedData: SharedDataService,
    private route: ActivatedRoute,
    private locationWiseDetailsService: LocationWiseDetailsService,
  ) {
    this.route.paramMap.subscribe(data => {
      this.pipSheetId = parseInt(data['params'].pipSheetId, 10);
    });
  }

  ngOnInit() {
    this.currencySymbol =
      this.sharedData.sharedData.currencyDTO.find(sym => sym.currencyId === this.sharedData.sharedData.currencyId).symbol;
    this.translateService.get('LocationWiseDetails.LocationWiseDetailsColumns').subscribe(data => {
      this.cols = data;
    });
    this.initailizeForm();
    this.getLocationWiseDetailsData();
  }

  initailizeForm() {
    this.locationWiseDetailsForm = this.fb.group({
      locationWiseDetails: this.fb.array([])
    });
  }
  bindFormData() {
    this.locationWiseDetailsData.locationWiseDetails.forEach((locationWiseDetail, index) => {
      this.locationWiseForm.push(this.locationWiseData(locationWiseDetail, index));
    });
  }
  highlightRow(rowindex) {
    this.selectedRow = rowindex;
  }
  getLocationWiseDetailsData() {
    this.locationWiseDetailsService.getLocationWiseDetails(this.pipSheetId).subscribe(locationDetails => {
      this.translateService.get('ProjectSummary').subscribe((label) => {
        locationDetails.locationWiseDetails.forEach((locationWiseRow, index) => {
          this.translationData = label.locationWiseDetailsLabels[index].label;
          locationWiseRow.locationWiseDetailsLabel = this.translationData;
        });
        this.getPeriods(locationDetails.summaryLocations);
        this.locationWiseDetailsData.locationWiseDetails = locationDetails.locationWiseDetails;
        this.locationWiseDetailsData.summaryLocations = locationDetails.summaryLocations;
        this.bindFormData();
      });
    });
  }

  get locationWiseForm() {
    return this.locationWiseDetailsForm.get('locationWiseDetails') as FormArray;
  }

  private locationWiseData(locationWiseDetail: ILocationWiseDetails, index: number): FormGroup {
    const locationWiseForm = this.fb.group({
      pipSheetId: this.pipSheetId,
      descriptionId: [locationWiseDetail.descriptionId],
      total: [locationWiseDetail.total],
      label: [locationWiseDetail.locationWiseDetailsLabel],
      summaryLocationDTO: this.locationWisePeriodData(locationWiseDetail.summaryLocationDTO, index),
    });
    return locationWiseForm;
  }

  private locationWisePeriodData(periodPrice: ILocationWisePeriodDetails[], uid: number): FormArray {
    const periodData = this.fb.array([]);

    periodPrice.forEach((period, index) => {
      periodData.push(this.formulateLocationWisePeriodForm(period, index, uid));
    });

    return periodData;
  }

  private formulateLocationWisePeriodForm(period: ILocationWisePeriodDetails, index: number, uid: number) {
    const periodForm = this.fb.group({
      descriptionId: [period.descriptionId],
      locationId: [this.locationWiseDetailsData.summaryLocations[index].locationId],
      amount: [period.amount]
    });
    return periodForm;
  }
  private getPeriods(periods: ISummaryLocations[]) {
    this.locationPeriods = periods;
    if (periods.length > 0) {
      this.locationCols = new ResourceMapper(null).computeDyanmicLocationColumns(periods);
    }
  }
}
