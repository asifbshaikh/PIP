import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { IMilestone } from '@shared';
import { TranslateService } from '@ngx-translate/core';
import { SharedDataService } from '@global';

@Component({
  selector: 'app-milestone',
  templateUrl: './milestone.component.html',
})
export class MilestoneComponent implements OnInit {

  milestoneForm: FormGroup;
  cols: any = [];
  milestoneDTO: IMilestone[] = [];
  isActive: false;
  constructor(
    private translateService: TranslateService,
    private fb: FormBuilder,
    private sharedData: SharedDataService
  ) { }

  ngOnInit() {
    this.translateService.get('Masters.Milestone').subscribe((data) => {
      this.cols = data;
    });

    this.initializeForm();
    this.bindFormData();
  }

  initializeForm() {
    this.milestoneForm = this.fb.group({
      milestoneDTO: this.fb.array([])
    });
  }

  bindFormData() {
    const milestoneForm = this.milestoneDataForm;
    let milestoneDTO = this.milestoneDTO;
    milestoneDTO = this.sharedData.sharedData.milestoneDTO;
    milestoneDTO.forEach((milestone) => {
      milestoneForm.push(this.milestoneMasterData(milestone));
    });
  }

  get milestoneDataForm() {
    return this.milestoneForm.get('milestoneDTO') as FormArray;
  }
  private milestoneMasterData(milestone: IMilestone): FormGroup {
    const milestoneForm = this.fb.group({
      milestoneName: [milestone.milestoneName],
      isActive: [],
      startDate: [],
      endDate: [],
      comments: []
    });
    return milestoneForm;
  }
}
