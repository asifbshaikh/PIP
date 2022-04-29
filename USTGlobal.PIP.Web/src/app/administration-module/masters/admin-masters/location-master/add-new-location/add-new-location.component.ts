import { Component, OnInit, Input, Output, OnChanges, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ILocation } from '@shared';
import { AddNewLocationService } from '@shared/services/add-new-location.service';
import { TranslateService } from '@ngx-translate/core';
import { MessageService } from 'primeng/api';
import { LocationMasterComponent } from '../location-master.component';
import { isNullOrUndefined } from 'util';
import { UtilityService } from '@core/infrastructure/utility.service';

@Component({
  selector: 'app-add-new-location',
  templateUrl: './add-new-location.component.html',
  providers: [LocationMasterComponent]
})
export class AddNewLocationComponent implements OnInit, OnChanges {
  statusFlag = [];
  newLocationForm: FormGroup;
  display = false;
  location: ILocation;
  allLocations: ILocation[];
  status: number;
  isActive: boolean;
  isDate: boolean;
  isPastVersionActive: boolean;
  cols: any = [];
  pastVersionCols: any = [];
  setDate: any;
  pastLocationVersions: ILocation[] = [];
  isInitialised = false;
  isDataSaved = false;
  savedLocation: ILocation;
  isNewVersion = false;
  showStartDate = false;
  iconEnable = false;
  minimumDate = new Date();
  minDate: Date;
  maxDate: Date;
  @Input() selectLocation;
  // tslint:disable-next-line: no-output-on-prefix
  @Output() onLocationSave: EventEmitter<ILocation> = new EventEmitter();


  constructor(
    private fb: FormBuilder,
    private addNewLocationService: AddNewLocationService,
    private translateService: TranslateService,
    private messageService: MessageService,
    private utilityService: UtilityService
  ) { }

  ngOnInit() {
    this.minDate = this.utilityService.setCalendarMinDate();
    this.maxDate = this.utilityService.setCalendarMaxDate();
    this.getStatusFlag();

    this.translateService.get('Masters.Location').subscribe((data) => {
      this.cols = data;
    });
    this.translateService.get('Masters.PastLocationVersions').subscribe((data) => {
      this.pastVersionCols = data;
    });
    this.initializeForm();
    if (!isNullOrUndefined(this.selectLocation)) {
      this.onLocationRowSelect(this.selectLocation);
    }

    this.isInitialised = true;
    this.display = true;
  }

  ngOnChanges(): void {
    if (this.isInitialised) {
      this.onLocationRowSelect(this.selectLocation);
    }
  }


  getStatusFlag() {
    if (this.statusFlag.length === 0) {
      this.translateService.get('MasterStatusFlag.statusFlag').subscribe((data) => {
        this.statusFlag = data;
      });
    }

  }



  initializeForm() {
    this.newLocationForm = this.fb.group({
      locationId: [''],
      locationName: ['', [Validators.required]],
      hoursPerMonth: ['', [Validators.required]],
      hoursPerDay: ['', [Validators.required]],
      startDate: [''],
      comment: ['', [Validators.maxLength(256)]],
      status: [],

    });
  }

  bindForm() {

    this.initializeForm();

    this.newLocationForm.setValue({
      locationId: this.location.locationId,
      locationName: this.location.locationName,
      hoursPerMonth: this.location.hoursPerMonth,
      hoursPerDay: this.location.hoursPerDay,
      comment: this.location.comments,
      startDate: this.location.startDate,
      status: this.location.status
    });

    this.getPastLocationVersion();
  }

  getPastLocationVersion() {
    this.addNewLocationService.getPastLocationVersions(this.location.locationId).subscribe(pastLoc => {
      this.pastLocationVersions = pastLoc;
    });
  }


  createNewVersion() {
    this.display = true;
    this.isNewVersion = true;
    this.status = this.statusFlag[0].value; // not submitted

    //  GetInactiveLocationVersion
    this.addNewLocationService.getInactiveLocationVersion(this.location.locationId).subscribe(inactiveRecords => {
      if (!isNullOrUndefined(inactiveRecords) && inactiveRecords.endDate === null) {

        this.status = this.statusFlag[inactiveRecords.status].value;



        // bind the data
        this.newLocationForm.setValue({
          locationId: inactiveRecords.locationId,
          locationName: inactiveRecords.locationName,
          hoursPerMonth: inactiveRecords.hoursPerMonth,
          hoursPerDay: inactiveRecords.hoursPerDay,
          comment: inactiveRecords.comments,
          startDate: inactiveRecords.startDate,
          status: inactiveRecords.status
        });

        this.showStartDate = inactiveRecords.startDate ? false : true;

        if (this.status !== 0) {
          this.enableDisableDialog();
        }

      } else {
        // bind the data
        this.newLocationForm.setValue({
          locationId: this.location.locationId,
          locationName: '',
          hoursPerMonth: '',
          hoursPerDay: '',
          comment: '',
          startDate: '',
          status: 0
        });

      }
    });
  }


  onLocationRowSelect(event) {
    const ADD_ONE_DAY = 86400000;  // Miliseconds to add  on day while setting date
    this.display = true;
    this.isDate = false;
    this.location = event;
    this.status = this.location.status;
    this.isActive = this.location.isActive;
    this.bindForm();
    this.enableDisableDialog();
    const startDate = new Date(this.location.startDate);
    if (this.location.startDate === null) {
      this.minimumDate = new Date(new Date().getTime() + ADD_ONE_DAY);
    }
    else if (startDate <= this.minimumDate) {
      this.minimumDate = new Date(new Date().getTime() + ADD_ONE_DAY);
    }
    else {
      this.minimumDate = new Date(startDate.getTime() + ADD_ONE_DAY);
    }
  }

  onSubmit() {
    const newLocation: ILocation = {
      locationName: this.newLocationForm.getRawValue().locationName,
      hoursPerMonth: this.newLocationForm.getRawValue().hoursPerMonth,
      hoursPerDay: this.newLocationForm.getRawValue().hoursPerDay,
      comments: this.newLocationForm.getRawValue().comment,
      locationId: this.newLocationForm.getRawValue().locationId > 0 ? this.newLocationForm.getRawValue().locationId : 0,
      countryID: null,
      refUSD: 0,
      ebitdaSeatCost: 0,
      inflationRate: 0,
      startDate: this.newLocationForm.getRawValue().startDate === '' || this.newLocationForm.getRawValue().startDate === null ? null
        : new Date(this.newLocationForm.getRawValue().startDate).toLocaleString(),
      endDate: null,
      isActive: false,
      isDeleted: false,
      status: this.newLocationForm.getRawValue().startDate ? this.statusFlag[2].value : this.statusFlag[1].value
    };
    this.addNewLocationService.saveNewLocation(newLocation).subscribe(locationId => {
      if (locationId === 0) {
        this.translateService.get('ErrorMessage.AddNewLocation').subscribe(msg => {
          this.messageService.add({ severity: 'error', detail: msg });
        });
        this.isDataSaved = false;

      }
      else {
        this.translateService.get('SuccessMessage.AddNewLocation').subscribe(msg => {
          this.messageService.add({ severity: 'success', detail: msg });
        });
        this.newLocationForm.controls.locationId.setValue(locationId);
        newLocation.locationId = locationId;
        newLocation.isActive = newLocation.startDate ? true : false;
        this.savedLocation = newLocation;
        this.isDataSaved = true;
      }

      this.onDialogClose();
    });
  }

  onSave() {
    const newLocation: ILocation = {
      locationName: this.newLocationForm.value.locationName,
      hoursPerMonth: this.newLocationForm.value.hoursPerMonth,
      hoursPerDay: this.newLocationForm.value.hoursPerDay,
      comments: this.newLocationForm.value.comment,
      locationId: this.newLocationForm.value.locationId > 0 ? this.newLocationForm.value.locationId : 0,
      countryID: null,
      refUSD: 0,
      ebitdaSeatCost: 0,
      inflationRate: 0,
      startDate: null,
      endDate: null,
      isActive: false,
      isDeleted: false,
      status: this.statusFlag[0].value
    };
    this.addNewLocationService.saveNewLocation(newLocation).subscribe(locationId => {
      if (locationId === 0) {
        this.translateService.get('ErrorMessage.AddNewLocation').subscribe(msg => {
          this.messageService.add({ severity: 'error', detail: msg });
        });
        this.isDataSaved = false;
      }
      else {
        this.translateService.get('SuccessMessage.AddNewLocation').subscribe(msg => {
          this.messageService.add({ severity: 'success', detail: msg });
        });
        this.newLocationForm.controls.locationId.setValue(locationId);
        newLocation.locationId = locationId;
        this.savedLocation = newLocation;
        this.isDataSaved = true;
      }
    });
  }

  enableDisableDialog() {

    // condition to disable approved master when user sets the startDate
    if (this.status === this.statusFlag[1].value || this.status === this.statusFlag[3].value ||
      this.status === this.statusFlag[2].value && this.location.startDate === null || this.isNewVersion) {

      this.newLocationForm.get('locationName').disable();
      this.newLocationForm.get('hoursPerMonth').disable();
      this.newLocationForm.get('hoursPerDay').disable();
      this.newLocationForm.get('comment').disable();
    }
    else {
      this.newLocationForm.get('locationName').enable();
      this.newLocationForm.get('hoursPerMonth').enable();
      this.newLocationForm.get('hoursPerDay').enable();
      this.newLocationForm.get('comment').enable();
    }
  }

  onDiscardVersion() {
    this.addNewLocationService.deleteRejectedLocation(this.location.locationId).subscribe(location => {
      this.translateService.get('SuccessMessage.DeleteLocation').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
      });
      if (!this.isNewVersion) {
        this.location.isDeleted = true;
        this.onLocationSave.emit(this.location);
      }
      this.display = false;
    });
  }
  showDate() {
    this.isDate = true;
  }

  onClose() {
    this.isDate = false;
    this.newLocationForm.controls.startDate.reset();
  }
  onSetDate() {
    this.iconEnable = true;
  }

  onDialogClose() {
    this.display = false;
    if (!this.isNewVersion || (this.isNewVersion && this.status === 2)) {
      this.onLocationSave.emit(this.savedLocation);
    } else {
      this.onLocationSave.emit(null);
    }
  }
}
