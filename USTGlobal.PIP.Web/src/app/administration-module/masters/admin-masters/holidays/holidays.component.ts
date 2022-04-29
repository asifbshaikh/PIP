import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { IHoliday, ILocation, Mastermapper, IBillingSchedule } from '@shared';
import { TranslateService } from '@ngx-translate/core';
import { SharedDataService } from '@global';
import { SelectItem } from 'primeng/api';
import { IYear } from '@shared/domain/IYear';
import { Paginator } from 'primeng/paginator';

@Component({
  selector: 'app-holidays',
  templateUrl: './holidays.component.html',
  styleUrls: ['./holidays.component.scss']
})
export class HolidaysComponent implements OnInit {
  holidaysForm: FormGroup;
  cols: any = [];
  isActive: false;
  holidayDTO: IHoliday[] = [];
  locationDTO: ILocation[] = [];
  billingYearDTO: IYear[] = [];
  locations: SelectItem[];
  years: SelectItem[];
  selectedMasterData: IHoliday[];
  selectedLocation: SelectItem;
  selectedYear: SelectItem = { value: -1 };
  AllLocations: FormArray;
  selectedLocationId = -1;
  constructor(
    private translateService: TranslateService,
    private fb: FormBuilder,
    private sharedData: SharedDataService,
  ) { }

  @ViewChild('p') paginator: Paginator;

  ngOnInit() {
    this.translateService.get('Masters.Holidays').subscribe((data) => {
      this.cols = data;
    });
    this.initializeForm();
    this.bindFormData();
    this.onYearChange(this.selectedYear);
    this.onLocationChange(this.selectedLocationId);
  }

  initializeForm() {
    this.holidaysForm = this.fb.group({
      holidayDTO: this.fb.array([]),
    });
  }

  bindFormData() {
    const holidayForm = this.holidayForm;
    let holidayDTO = this.holidayDTO;
    holidayDTO = this.sharedData.sharedData.holidayDTO;
    this.locationDTO = this.sharedData.sharedData.locationDTO;
    this.billingYearDTO = this.sharedData.sharedData.billingYearDTO;
    this.locations = new Mastermapper().getLocationComboItems(this.locationDTO, true);
    this.years = new Mastermapper().getYearComboItems(this.billingYearDTO, true);
    holidayDTO.forEach((holiday, index) => {
      holiday.locationName = this.locationDTO.find(locName => locName.locationId === holiday.locationId).locationName;
      holidayForm.push(this.MasterData(holiday, index));
    });

    this.AllLocations = this.holidayForm;
  }
  get holidayForm() {
    return this.holidaysForm.get('holidayDTO') as FormArray;
  }

  private MasterData(holiday: IHoliday, index: number): FormGroup {
    const holidayForm = this.fb.group({
      locationId: [holiday.locationId],
      locationName: [holiday.locationName],
      date: [holiday.date],
      isActive: [],
      startDate: [],
      endDate: [],
      comments: []
    });
    return holidayForm;
  }

  onLocationChange(id: number) {
    this.selectedLocationId = id;
    const clonedDTO = this.fb.array([]);
    const filteredLocation = this.fb.array([]);

    Object.assign(clonedDTO, this.AllLocations);


    if (this.selectedLocationId !== -1 || this.selectedYear.value !== -1) {
      if (this.selectedYear.value !== -1 && this.selectedLocationId !== -1) {
        if (this.selectedYear.value.id === -1 && this.selectedLocationId !== -1) {
          clonedDTO.controls.filter(location => ((<FormGroup>location).controls.locationId.value === id))
            .forEach(loc => {
              filteredLocation.push(loc);           // location
            });
        }
        clonedDTO.controls.filter(location => ((<FormGroup>location).controls.locationId.value === id)
          && (new Date((<FormGroup>location).controls.date.value).getFullYear() === +this.selectedYear.label))
          .forEach(loc => {
            filteredLocation.push(loc);           // both
          });
        this.holidaysForm.setControl('holidayDTO', filteredLocation);
      } else if (this.selectedYear.value === -1 && this.selectedLocationId !== -1) {
        clonedDTO.controls.filter(location => ((<FormGroup>location).controls.locationId.value === id))
          .forEach(loc => {
            filteredLocation.push(loc);           // location
          });
        this.holidaysForm.setControl('holidayDTO', filteredLocation);
      }
      else if (this.selectedLocationId === -1 && this.selectedYear.value.id !== -1) {
        clonedDTO.controls.filter(location => (new Date(((<FormGroup>location).controls.date.value))
          .getFullYear() === +this.selectedYear.label)).forEach(loc => {
            filteredLocation.push(loc);           // holiday
          });
        this.holidaysForm.setControl('holidayDTO', filteredLocation);
      }
      else if (this.selectedLocationId === -1 && this.selectedYear.value.id === -1) {
        this.holidaysForm.setControl('holidayDTO', clonedDTO);
      }
      else if (this.selectedYear.value !== -1 && this.selectedLocationId === -1) {
        clonedDTO.controls.filter(location => ((<FormGroup>location).controls.locationId.value === id))
          .forEach(loc => {
            filteredLocation.push(loc);           // location
          });
        this.holidaysForm.setControl('holidayDTO', filteredLocation);
      }
      else if (this.selectedYear.value === -1 && this.selectedLocationId !== -1) {
        clonedDTO.controls.filter(location => (new Date(((<FormGroup>location).controls.date.value))
          .getFullYear() === +this.selectedYear.label)).forEach(loc => {
            filteredLocation.push(loc);           // holiday
          });
        this.holidaysForm.setControl('holidayDTO', filteredLocation);
      }
    }
    else {
      this.holidaysForm.setControl('holidayDTO', clonedDTO);
    }
  }



  onYearChange(year) {
    if (this.selectedYear.value !== -1) {
      this.selectedYear.label = year;
      this.onLocationChange(this.selectedLocationId);
    }
    else {
      this.onLocationChange(this.selectedLocationId);
    }
  }

  paginate() {
    this.paginator._first = 1;
  }
}
