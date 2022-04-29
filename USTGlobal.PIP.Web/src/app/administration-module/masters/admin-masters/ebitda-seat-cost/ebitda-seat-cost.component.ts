import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { ILocation } from '@shared';
import { TranslateService } from '@ngx-translate/core';
import { SharedDataService } from '@global';

@Component({
  selector: 'app-ebitda-seat-cost',
  templateUrl: './ebitda-seat-cost.component.html',
})
export class EbitdaSeatCostComponent implements OnInit {

  ebitdaSeatCostForm: FormGroup;
  cols: any = [];
  isActive: false;
  locationDTO: ILocation[] = [];

  constructor(
    private translateService: TranslateService,
    private fb: FormBuilder,
    private sharedData: SharedDataService
  ) { }

  ngOnInit() {
    this.translateService.get('Masters.EbitdaSeatCost').subscribe((data) => {
      this.cols = data;
    });

    this.initializeForm();
    this.bindFormData();
  }
  initializeForm() {
    this.ebitdaSeatCostForm = this.fb.group({
      locationDTO: this.fb.array([])
    });
  }

  bindFormData() {
    const ebitdaSeatCostForm = this.ebitdaForm;
    let locationDTO = this.locationDTO;
    locationDTO = this.sharedData.sharedData.locationDTO;
    locationDTO.forEach((location) => {
      ebitdaSeatCostForm.push(this.locationMasterData(location));
    });
  }

  get ebitdaForm() {
    return this.ebitdaSeatCostForm.get('locationDTO') as FormArray;
  }

  private locationMasterData(ebitda: ILocation): FormGroup {
    const ebitdaForm = this.fb.group({
      locationName: [ebitda.locationName],
      refUSD: [ebitda.refUSD],
      ebitdaSeatCost: [ebitda.ebitdaSeatCost],
      isActive: [],
      startDate: [],
      endDate: [],
      comments: []
    });
    return ebitdaForm;
  }
}
