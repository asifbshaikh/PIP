import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { IServicePortfolio, IServiceLine } from '@shared';
import { TranslateService } from '@ngx-translate/core';
import { SharedDataService } from '@global';

@Component({
  selector: 'app-service-portfolio-group',
  templateUrl: './service-portfolio-group.component.html',
  styleUrls: ['./service-portfolio-group.component.scss']
})
export class ServicePortfolioGroupComponent implements OnInit {
  servicePortfolioForm: FormGroup;
  cols: any = [];
  columns: any = [];
  isActive: false;
  servicePortfolioDTO: IServicePortfolio[] = [];
  serviceLineDTO: IServiceLine[] = [];
  isPortfolioId: boolean;
  portfolioId: number;
  constructor(
    private translateService: TranslateService,
    private fb: FormBuilder,
    private sharedData: SharedDataService
  ) { }

  ngOnInit() {
    this.initializeForm();
    this.translateService.get('Masters.ServicePortfolioGroup').subscribe((data) => {
      this.cols = data;
    });

    this.translateService.get('Masters.ServiceLine').subscribe((data) => {
      this.columns = data;
    });

    this.bindFormData();
  }

  initializeForm() {
    this.servicePortfolioForm = this.fb.group({
      servicePortfolioDTO: this.fb.array([]),
      serviceLineDTO: this.fb.array([])
    });
  }

  bindFormData() {
    const form = this.form;
    this.servicePortfolioDTO = this.sharedData.sharedData.servicePortfolioDTO;
    this.servicePortfolioDTO.forEach((data, index) => {
      form.push(this.masterData(data, index));
    });
    this.bindDependentFormData();
  }

  bindDependentFormData() {
    const dependentForm = this.dependentForm;
    this.serviceLineDTO = this.sharedData.sharedData.serviceLineDTO;
    this.serviceLineDTO.forEach((data, index) => {
      data.portfolioName = this.servicePortfolioDTO.find(portfolioName => portfolioName.servicePortfolioId
        === data.servicePortfolioId).portfolioName;
      dependentForm.push(this.dependentMasterData(data, index));
    });
  }

  get form() {
    return this.servicePortfolioForm.get('servicePortfolioDTO') as FormArray;
  }

  get dependentForm() {
    return this.servicePortfolioForm.get('serviceLineDTO') as FormArray;
  }

  private masterData(data: IServicePortfolio, index: number): FormGroup {
    const form = this.fb.group({
      portfolioName: [data.portfolioName],
      isActive: [],
      startDate: ['01/01/2019'],
      endDate: [],
      comments: [],
    });
    return form;
  }

  private dependentMasterData(data: IServiceLine, index: number): FormGroup {
    const form = this.fb.group({
      serviceLineName: [data.serviceLineName],
      servicePortfolioId: [data.servicePortfolioId],
      serviceLineId: [data.serviceLineId],
      isPortfolioId: this.checkNewPortfolio(data, index),
      portfolioName: [data.portfolioName],
      isActive: [],
      startDate: ['01/01/2019'],
      endDate: [],
      comments: []
    });
    return form;
  }

  checkNewPortfolio(data: IServiceLine, index: number) {

    if (index === 0) {
      this.portfolioId = data.servicePortfolioId;
      return true;
    }

    if (this.portfolioId === data.servicePortfolioId) {
      return false;
    } else {
      this.portfolioId = data.servicePortfolioId;
      return true;
    }
  }
}
