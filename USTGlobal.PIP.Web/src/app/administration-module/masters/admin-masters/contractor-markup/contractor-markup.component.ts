import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { FormBuilder, FormGroup, FormArray } from '@angular/forms';
import { SharedDataService } from '@global';
import { IResourceMarkup } from '@shared';

@Component({
  selector: 'app-contractor-markup',
  templateUrl: './contractor-markup.component.html',
  styleUrls: ['./contractor-markup.component.scss']
})
export class ContractorMarkupComponent implements OnInit {
  contractorMarkupForm: FormGroup;
  cols: any = [];
  isActive: false;
  markupDTO: IResourceMarkup[] = [];
  constructor(
    private translateService: TranslateService,
    private fb: FormBuilder,
    private sharedData: SharedDataService
  ) { }

  ngOnInit() {
    this.translateService.get('Masters.ContractorMarkup').subscribe((data) => {
      this.cols = data;
    });

    this.initializeForm();
    this.bindFormData();
  }

  initializeForm() {
    this.contractorMarkupForm = this.fb.group({
      markupDTO: this.fb.array([])
    });
  }

  bindFormData() {
    const markupForm = this.markupForm;
    let markupDTO = this.markupDTO;
    markupDTO = this.sharedData.sharedData.markupDTO;
    markupDTO.forEach((markup, index) => {
      markupForm.push(this.MasterData(markup, index));
    });
  }
  get markupForm() {
    return this.contractorMarkupForm.get('markupDTO') as FormArray;
  }

  private MasterData(markup: IResourceMarkup, index: number): FormGroup {
    const markupForm = this.fb.group({
      name: [markup.name],
      isActive: [],
      startDate: [],
      endDate: [],
      comments: []
    });
    return markupForm;
  }
}
