import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { SharedDataService } from '@global';
import { IContractingEntity } from '@shared';

@Component({
  selector: 'app-contracting-entity',
  templateUrl: './contracting-entity.component.html',
  styleUrls: ['./contracting-entity.component.scss']
})
export class ContractingEntityComponent implements OnInit {

  contractingEntityForm: FormGroup;
  cols: any = [];
  isActive: false;
  contractingEntityDTO: IContractingEntity[] = [];
  constructor(
    private translateService: TranslateService,
    private fb: FormBuilder,
    private sharedData: SharedDataService
  ) { }

  ngOnInit() {
    this.translateService.get('Masters.ContractingEntity').subscribe((data) => {
      this.cols = data;
    });
    this.initializeForm();
    this.bindFormData();
  }

  initializeForm() {
    this.contractingEntityForm = this.fb.group({
      contractingEntityDTO: this.fb.array([])
    });
  }

  bindFormData() {
    const contractingForm = this.contractingForm;
    let contractingEntityDTO = this.contractingEntityDTO;
    contractingEntityDTO = this.sharedData.sharedData.contractingEntityDTO;
    contractingEntityDTO.forEach((contractingEntity, index) => {
      contractingForm.push(this.MasterData(contractingEntity, index));
    });
  }
  get contractingForm() {
    return this.contractingEntityForm.get('contractingEntityDTO') as FormArray;
  }

  private MasterData(contractingEntity: IContractingEntity, index: number): FormGroup {
    const contractingForm = this.fb.group({
      code: [contractingEntity.code],
      name: [contractingEntity.name],
      isActive: [],
      startDate: [],
      endDate: [],
      comments: []
    });
    return contractingForm;
  }
}
