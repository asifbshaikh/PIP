import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { FormBuilder, FormGroup, FormArray } from '@angular/forms';
import { SharedDataService } from '@global';
import { IDeliveryType, IBillingType } from '@shared';
import { IDeliveryBillingType } from '@shared/domain/IDeliveryBillingType';

@Component({
  selector: 'app-project-delivery-type',
  templateUrl: './project-delivery-type.component.html',
  styleUrls: ['./project-delivery-type.component.scss']
})
export class ProjectDeliveryTypeComponent implements OnInit {
  projectDeliveryForm: FormGroup;
  cols: any = [];
  columns: any = [];
  projectDeliveryTypeDTO: IDeliveryType[] = [];
  projectDeliveryBillingTypeDTO: IDeliveryBillingType[] = [];
  isDeliveryTypeId: boolean;
  deliveryTypeId: number;
  constructor(
    private translateService: TranslateService,
    private fb: FormBuilder,
    private sharedData: SharedDataService
  ) { }

  ngOnInit() {
    this.translateService.get('Masters.DeliveryType').subscribe((data) => {
      this.cols = data;
    });

    this.translateService.get('Masters.BillingType').subscribe((data) => {
      this.columns = data;
    });

    this.initializeForm();
    this.bindFormData();

  }


  initializeForm() {
    this.projectDeliveryForm = this.fb.group({
      projectDeliveryTypeDTO: this.fb.array([]),
      projectDeliveryBillingTypeDTO: this.fb.array([])
    });
  }

  bindFormData() {
    const form = this.form;
    const dependentForm = this.dependentForm;
    let billingTypeForm = this.projectDeliveryBillingTypeDTO;
    this.projectDeliveryTypeDTO = this.sharedData.sharedData.projectDeliveryTypeDTO;
    this.projectDeliveryTypeDTO.forEach((data, index) => {
      form.push(this.masterData(data, index));
    });

    billingTypeForm = this.sharedData.sharedData.projectDeliveryBillingTypeDTO;
    billingTypeForm.forEach((data, index) => {
      data.deliveryTypeName = this.projectDeliveryTypeDTO.find(deliveryTypeName => deliveryTypeName.projectDeliveryTypeId
        === data.projectDeliveryTypeId).deliveryType;
      dependentForm.push(this.dependentMasterData(data, index));
    });
  }

  get form() {
    return this.projectDeliveryForm.get('projectDeliveryTypeDTO') as FormArray;
  }
  get dependentForm() {
    return this.projectDeliveryForm.get('projectDeliveryBillingTypeDTO') as FormArray;
  }
  private masterData(data: IDeliveryType, index: number): FormGroup {
    const form = this.fb.group({
      deliveryType: [data.deliveryType],
      isActive: [],
      startDate: [],
      endDate: [],
      comments: []
    });
    return form;
  }

  private dependentMasterData(data: IDeliveryBillingType, index: number): FormGroup {
    const form = this.fb.group({
      billingTypeName: [data.billingTypeName],
      isDeliveryTypeId: this.checkNewDeliveyType(data, index),
      deliveryTypeName: [data.deliveryTypeName],
      isActive: [],
      startDate: [],
      endDate: [],
      comments: []
    });
    return form;
  }

  checkNewDeliveyType(data: IDeliveryBillingType, index: number) {

    if (index === 0) {
      this.deliveryTypeId = data.projectDeliveryTypeId;
      return true;
    }

    if (this.deliveryTypeId === data.projectDeliveryTypeId) {
      return false;
    } else {
      this.deliveryTypeId = data.projectDeliveryTypeId;
      return true;
    }
  }
}


