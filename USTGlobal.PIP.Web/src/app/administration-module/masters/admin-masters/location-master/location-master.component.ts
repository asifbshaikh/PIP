import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { ILocation } from '@shared';
import { AddNewLocationService } from '@shared/services/add-new-location.service';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'app-location-master',
  templateUrl: './location-master.component.html',
  styleUrls: ['./location-master.component.scss']
})
export class LocationMasterComponent implements OnInit, OnChanges {

  locationMasterFrom: FormGroup;
  cols: any = [];
  isActive: false;
  display = false;
  displayLocation = false;
  selectedLocation: FormGroup;
  locationDTO: ILocation[];
  statusFlag: any = [];
  openDialog = false;
  location: ILocation;
  isInitialised = false;

  @Input() savedLocation;

  constructor(
    private translateService: TranslateService,
    private fb: FormBuilder,
    private addNewLocationService: AddNewLocationService,
  ) { }

  ngOnInit() {
    this.translateService.get('Masters.Location').subscribe((data) => {
      this.cols = data;
    });
    this.translateService.get('MasterStatusFlag.statusFlag').subscribe((data) => {
      this.statusFlag = data;
    });

    this.initializeForm();
    this.getAllLocations();
    this.isInitialised = true;

  }

  ngOnChanges(): void {
    if (this.isInitialised) {
      this.locationForm.push(this.locationMasterData(this.savedLocation));
    }
  }

  initializeForm() {
    this.locationMasterFrom = this.fb.group({
      locationDTO: this.fb.array([])
    });
  }

  bindFormData() {
    const locationForm = this.locationForm;
    const locationDTO = this.locationDTO;
    locationDTO.forEach((location) => {
      locationForm.push(this.locationMasterData(location));
    });
  }

  getAllLocations() {
    this.addNewLocationService.getAllLocations().subscribe(locations => {
      this.locationDTO = locations;
      this.bindFormData();
    });
  }

  get locationForm() {
    return this.locationMasterFrom.get('locationDTO') as FormArray;
  }


  private locationMasterData(location: ILocation): FormGroup {
    const locationForm = this.fb.group({
      locationId: [location.locationId],
      locationName: [location.locationName],
      hoursPerDay: [location.hoursPerDay],
      hoursPerMonth: [location.hoursPerMonth],
      isActive: [location.isActive],
      startDate: [location.startDate],
      endDate: [location.endDate],
      comments: [location.comments],
      status: [location.status],
      isDeleted: [false],
    });
    return locationForm;
  }

  get locationStatus() {
    return this.locationMasterFrom.get('locationDTO').value;
  }

  onLocationSelected() {
    this.location = this.selectedLocation.value;
    this.openDialog = true;
  }

  onSaved(savedLocation: ILocation) {
    let location;
    if (!isNullOrUndefined(savedLocation)) {
      if (savedLocation.isDeleted === true) {
        this.deleteLocation(savedLocation);
      }
      if (savedLocation.locationId === 0) {
        location = this.locationMasterData(savedLocation);
        this.locationForm.push(location);
      } else {
        this.updateLocation(savedLocation);
      }
    }
    this.selectedLocation = null;
    this.openDialog = false;
  }


  updateLocation(savedLocation: ILocation) {
    this.selectedLocation.patchValue({
      locationId: savedLocation.locationId,
      locationName: savedLocation.locationName,
      hoursPerMonth: savedLocation.hoursPerMonth,
      hoursPerDay: savedLocation.hoursPerDay,
      comments: savedLocation.comments,
      startDate: savedLocation.startDate,
      status: savedLocation.status,
      endDate: savedLocation.endDate,
      isActive: savedLocation.isActive
    });
  }

  deleteLocation(savedLocation: ILocation) {
    this.locationForm.removeAt(this.locationForm.value.findIndex(loc => loc.locationId === savedLocation.locationId));
  }
}
