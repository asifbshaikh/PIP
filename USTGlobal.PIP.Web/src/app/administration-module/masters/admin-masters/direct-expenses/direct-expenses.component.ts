import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { IDefaultLabel } from '@shared/domain/defaultlabel';
import { TranslateService } from '@ngx-translate/core';
import { SharedDataService } from '@global';

@Component({
  selector: 'app-direct-expense',
  templateUrl: './direct-expenses.component.html',
})
export class DirectExpensesComponent implements OnInit {
  directExpensesForm: FormGroup;
  cols: any = [];
  defaultLabelDTO: IDefaultLabel[] = [];
  isActive: false;
  constructor(
    private translateService: TranslateService,
    private fb: FormBuilder,
    private sharedData: SharedDataService
  ) { }

  ngOnInit() {
    this.translateService.get('Masters.DefaultLabels').subscribe((data) => {
      this.cols = data;
    });

    this.initializeForm();
    this.bindFormData();
  }

  initializeForm() {
    this.directExpensesForm = this.fb.group({
      defaultLabelDTO: this.fb.array([])
    });
  }

  bindFormData() {
    const directExpensesForm = this.directExpensesDataForm;
    let defaultLabelDTO = this.defaultLabelDTO;
    defaultLabelDTO = this.sharedData.sharedData.defaultLabelDTO;
    defaultLabelDTO.forEach((directExpenses) => {
      directExpensesForm.push(this.milestoneMasterData(directExpenses));
    });
  }

  get directExpensesDataForm() {
    return this.directExpensesForm.get('defaultLabelDTO') as FormArray;
  }
  private milestoneMasterData(defaultLabels: IDefaultLabel): FormGroup {
    const directExpensesForm = this.fb.group({
      name: [defaultLabels.name],
      isActive: [],
      startDate: [],
      endDate: [],
      comments: []
    });
    return directExpensesForm;
  }

}
