import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { SharedDataService } from '@global';
import { IBasicAsset } from '@shared/domain/basicasset';

@Component({
  selector: 'app-basic-assets',
  templateUrl: './basic-assets.component.html',
})
export class BasicAssetsComponent implements OnInit {

  basicAssetsForm: FormGroup;
  cols: any = [];
  basicAssetDTO: IBasicAsset[] = [];
  isActive: false;
  constructor(
    private translateService: TranslateService,
    private fb: FormBuilder,
    private sharedData: SharedDataService
  ) { }

  ngOnInit() {
    this.translateService.get('Masters.BasicAssets').subscribe((data) => {
      this.cols = data;
    });

    this.initializeForm();
    this.bindFormData();
  }

  initializeForm() {
    this.basicAssetsForm = this.fb.group({
      basicAssetDTO: this.fb.array([])
    });
  }

  bindFormData() {
    let basicAssetDTO = this.basicAssetDTO;
    const basicAssetsDataForm = this.basicAssetsDataForm;
    basicAssetDTO = this.sharedData.sharedData.basicAssetDTO;
    basicAssetDTO.forEach((basicAssets) => {
      basicAssetsDataForm.push(this.basicAssetsData(basicAssets));
    });
  }

  get basicAssetsDataForm() {
    return this.basicAssetsForm.get('basicAssetDTO') as FormArray;
  }
  private basicAssetsData(basicAssets: IBasicAsset): FormGroup {
    const milestoneForm = this.fb.group({
      defaultLabel: [basicAssets.description],
      costToProject: [basicAssets.costToProject],
      isActive: [],
      startDate: [],
      endDate: [],
      comments: []
    });
    return milestoneForm;
  }

}
