import { IPipCheckIn } from './../../../shared-module/domain/IPipCheckIn';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { DialogService, ConfirmationService, SelectItem, MessageService } from 'primeng/api';
import { PipVersionComponent } from '../pip-versions/pip-version.component';
import { Mastermapper, Constants, IPipVersion } from '@shared';
import { SharedDataService } from '@global';
import { ProjectService } from '@shared/services/project.service';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ReplicateService } from '@shared/services/replicate.service';
import { TranslateService } from '@ngx-translate/core';
import { ValidationService } from '@core/infrastructure/validation.service';
import { IReplicatePipSheet } from '@shared/domain/IReplicatePipSheet';
import { PipVersionService } from '@shared/services/pip-version.service';
import { IPipMainVersion } from '@shared/domain/IPipMainVersion';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'app-replicate',
  templateUrl: './replicate.component.html',
  styleUrls: ['./replicate.component.scss']
})
export class ReplicateComponent implements OnInit {
  replicateForm: FormGroup;
  @Input() pipVeraion: PipVersionComponent;
  @Input() replicateType: any;
  @Input() replicateObj: any;
  @Input() versionId: any;
  @Input() replicateNew: any;
  @Input() replicateSame: any;
  @Input() replicateExisting: any;
  @Input() replicateNewInMyPips: any;
  @Input() selectedPipSheetId: any;
  @Output() action = new EventEmitter<string>();
  accounts: SelectItem[];
  accountBasedProjectList: SelectItem[];
  isDummy: boolean;
  dashboardId: number;
  dummyProjectId: string;
  replicateCreate: boolean;
  sourceAccountName: string;
  selectedVersionList: IPipMainVersion;
  selectedProjectId: number;
  isInvalid = false;
  idFormat: string;
  constructor(
    private confirmationService: ConfirmationService,
    private sharedData: SharedDataService,
    private router: Router,
    private route: ActivatedRoute,
    private projectService: ProjectService,
    private replicateService: ReplicateService,
    private fb: FormBuilder,
    private translateService: TranslateService,
    private validate: ValidationService,
    private messageService: MessageService,
    private versionService: PipVersionService,
  ) { }

  ngOnInit() {
    this.isDummy = this.router.url.includes('samples') ? true : false;
    this.route.paramMap.subscribe(
      params => {
        // this.sourcePipSheetId = this.replicateObj[0].pipSheetId;
        this.dashboardId = 1;
      });
    this.projectService.getUserRoleForAllAccounts().subscribe(roleData => {
      this.accounts = new Mastermapper().getAccountNameForEditorRole
        (this.sharedData.sharedData.accountDTO, roleData, true);
      this.sourceAccountName = this.accounts.find(acc => acc.value.id === this.replicateObj[0].accountId).label;
    });
    this.translateService.get('ProjectHeader.format').subscribe(format => {
      this.idFormat = format;
    });
    this.initializeForm();
  }

  initializeForm() {
    this.replicateForm = this.fb.group({
      accountName: ['', [Validators.required, this.validate.validateDeselectedDropdown]],
      projectId: [((this.replicateNew && !this.isDummy) || this.replicateNewInMyPips) ? { value: '', disabled: false } :
        { value: '', disabled: true }, [Validators.required, Validators.pattern(Constants.regExType.projectName)]],
      projectNamePerSf: [((this.replicateNew) || this.replicateNewInMyPips ||
        this.replicateSame) ?
        { value: '', disabled: false } : { value: '', disabled: true },
      [Validators.required, this.validate.noWhitespaceValidator, Validators.maxLength(100)]],
      dummyProjectId: [(this.isDummy && this.replicateNew) ? { value: '', disabled: false } : { value: '', disabled: true },
      [Validators.required]],
      dropdownProjectId: [this.replicateExisting ? { value: '', disabled: false } : { value: '', disabled: true },
      [Validators.required, this.validate.validateDeselectedDropdown]],
      versions: [],
    });
  }
  confirm() {
    this.confirmationService.confirm({
      message: 'Do you really want to Replicate in new project?',
      accept: () => {
        // Actual logic to perform a confirmation
      }
    });
  }

  onCreateClick(replicaTypeId: number) {
    this.action.emit(null);
    if (this.replicateSame) {
      this.replicateForm.controls.accountName.setValue(this.replicateObj[0].accountId);
      this.replicateForm.controls.projectId.setValue(this.replicateObj[0].projectId);
    }
    if (this.replicateNewInMyPips || (this.isDummy && this.replicateExisting)) {
      this.isDummy = false;
    }
    const accountId = this.replicateSame ? this.replicateForm.value.accountName : this.replicateForm.value.accountName.id;
    const paymentLag = this.sharedData.sharedData.accountDTO.find(acc => acc.accountId === accountId).paymentLag;
    const user: IReplicatePipSheet = {
      sourcePIPSheetId: this.selectedPipSheetId,
      sourceProjectId: this.replicateExisting ? +this.selectedProjectId : this.replicateObj[0].projectId,
      sfProjectId: this.replicateExisting ? this.replicateForm.value.dropdownProjectId.name : (this.isDummy && this.replicateNew) ?
        this.replicateForm.value.dummyProjectId : this.replicateSame ? '' : this.replicateForm.value.projectId,
      accountId: this.replicateSame ? this.replicateObj[0].accountId : this.replicateForm.value.accountName.id,
      projectNamePerSf: this.replicateExisting ? '' : this.replicateForm.value.projectNamePerSf,
      paymentLag: paymentLag,
      isDummy: this.isDummy,
      replicateType: replicaTypeId,
      versionNumber: 1,
    };
    this.replicateService.createReplicatePipSheet(user).subscribe(data => {
      if (data.errorCode === -1) {
        this.translateService.get('ErrorMessage.DuplicateProjectId').subscribe(msg => {
          this.messageService.add({ severity: 'error', detail: msg });
        });
      }
      else {
        this.translateService.get('SuccessMessage.ReplicateSuccess').subscribe(msg => {
          this.messageService.add({ severity: 'success', detail: msg });

          this.checkInPipOnCreateClick(data.pipSheetId);

          if (this.isDummy) {
            this.router.navigate([`samples/${data.projectId}/${data.pipSheetId}/${this.replicateSame ?
              this.replicateObj[0].accountId : user.accountId}/${1}/Staff`]);
          }
          else {
            this.router.navigate([`projects/${data.projectId}/${data.pipSheetId}/${this.replicateSame ?
              this.replicateObj[0].accountId : user.accountId}/${1}/Staff`]);
          }
          this.replicateForm.reset();
        });
      }

      this.replicateCreate = true;
    });
  }
// convenience getter for easy access to form fields
get refPh() { return this.replicateForm.controls; }

  checkInPipOnCreateClick(pipSheetId: number) {
    const pipCheckIn: IPipCheckIn = {
      pipSheetId: pipSheetId,
      isCheckedOut: false,
      checkedInOutBy: null
    };
    this.versionService.updatePIPSheetCheckIn(pipCheckIn).subscribe((res: number) => { });
  }

  onCancelClick() {
    this.action.emit(null);
    this.replicateForm.reset();
  }

  onAccountNameChange(accId) {
    const selectItem = -1;
    const accountCode = accId === selectItem ? '' :
      this.sharedData.sharedData.accountDTO.find(id => +accId === id.accountId).accountCode;
    if (accId === selectItem) {
      this.replicateForm.controls.projectId.setValue(null);
      this.replicateForm.controls.dummyProjectId.setValue(null);
      this.accountBasedProjectList = [];
      this.selectedVersionList = null;
      this.isInvalid = false;
      this.selectedVersionList = null;
    }
    else {
      if ((this.replicateNew && !this.isDummy) || this.replicateNewInMyPips) {
        this.replicateForm.controls.projectId.setValue(accountCode + '-');
      }
      else {
        this.projectService.getAutoGeneratedProjectId(accId, accountCode).subscribe(dummyId => {
          this.replicateForm.controls.dummyProjectId.setValue(dummyId);
        });
        this.replicateService.getProjectListBasedOnAccountId(accId).subscribe(projectList => {
          this.accountBasedProjectList = new Mastermapper().getProjectListComboItems(projectList, true);
          this.replicateForm.controls.dropdownProjectId.reset();
          if (!isNullOrUndefined(this.selectedVersionList)) {
            this.selectedVersionList.pipSheetVersionDTO = [];
          }
          this.isInvalid = false;
        });
      }
    }
  }

  onProjectListDropdownChange(projectId) {
    this.selectedProjectId = projectId;
    this.versionService.getPipVersions(projectId).subscribe(data => {
      this.selectedVersionList = data;
      this.isInvalid = this.selectedVersionList.pipSheetVersionDTO.length > 4 ? true : false;
    });
  }
}
