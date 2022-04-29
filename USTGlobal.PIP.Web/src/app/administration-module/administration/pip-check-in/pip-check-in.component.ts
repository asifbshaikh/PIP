import { ICheckOutPipVersion, IPipCheckInProject } from '@shared';
import { PipCheckinService } from '../../../shared-module/services/pip-checkin.service';
import { SharedDataService } from '@global';
import { TranslateService } from '@ngx-translate/core';
import { Mastermapper } from './../../../shared-module/mapper/master/mastermapper';
import { SelectItem, MessageService } from 'primeng/api';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { version } from 'process';

@Component({
  selector: 'app-pip-check-in',
  templateUrl: './pip-check-in.component.html'
})

export class PipCheckInComponent implements OnInit {

  pipCheckIn: FormGroup;
  accountName: SelectItem[];
  sfProjectId: SelectItem[];
  versionHeaderCols: any[] = [];
  selectedVersions: any[] = [];
  isSelectionValid = false;
  selectedProjectId;
  isProjectsPresent = true;
  selectedAccount;
  disableCheckBox = true;

  constructor(
    private fb: FormBuilder,
    private translateService: TranslateService,
    private sharedData: SharedDataService,
    private pipCheckinService: PipCheckinService,
    private messageService: MessageService
  ) { }

  ngOnInit() {

    this.initializeForm();

    //  Version header columns
    this.getColumns('Administration.PipCheckIn.VersionColumns').subscribe(verCols => {
      this.versionHeaderCols = verCols;
    });

    this.accountName = new Mastermapper().getAccountComboItems(this.sharedData.sharedData.accountDTO, true);
  }

  initializeForm() {
    this.pipCheckIn = this.fb.group({
      selectedProject: null,
      versions: this.fb.array([])
    });
  }

  getColumns(path: string) {
    return this.translateService.get(path);
  }

  public addFirstItem(): SelectItem {
    const subItem = {
      'id': -1,
      'code': '',
      'name': null
    };
    const item: SelectItem = { label: '--- select ---', value: subItem };
    return item;
  }

  onAccountNameChange(singleAccountData: any) {
    this.selectedAccount = singleAccountData;
    if (singleAccountData.id) {
      this.pipCheckinService.getProjects(singleAccountData.id).subscribe((projectData) => {
        if (projectData.length > 0) {
          this.isProjectsPresent = true;
          this.sfProjectId = new Mastermapper().getPipCheckInProjectComboItems(projectData);
          this.sfProjectId.unshift(this.addFirstItem());
          this.initializeForm();
          this.selectedProjectId = null;
        } else {
          if (singleAccountData.id !== -1) {
            this.isProjectsPresent = false;
          } else {
            this.isProjectsPresent = true;
          }
            this.initializeForm();
            this.sfProjectId = new Mastermapper().getPipCheckInProjectComboItems(null);
            this.sfProjectId.unshift(this.addFirstItem());
        }
      });
    }
  }

  onSfProjectIdChange(selectedSFProjectId: any) {
    this.pipCheckIn.controls.selectedProject.setValue(selectedSFProjectId);
    this.selectedProjectId = selectedSFProjectId;
    if (selectedSFProjectId.id) {
      this.pipCheckinService.getVersions(selectedSFProjectId.id).subscribe((versionData) => {
        if (versionData.length > 0) {
          const versionControls = this.fb.array([]);
          versionData.forEach((singleVersion, index) => {
            versionControls.push(this.versionData(singleVersion, index));
          });
          this.pipCheckIn.setControl('versions', versionControls);
          this.disableCheckBox = false;
        } else {
          this.initializeForm();
          this.pipCheckIn.controls.selectedProject.setValue(selectedSFProjectId);
          this.disableCheckBox = true;
        }
      });
    }
  }

  private versionData(singleVersion: ICheckOutPipVersion, index: number): FormGroup {
    const dataForm = this.fb.group({
      pipSheetId: [singleVersion.pipSheetId],
      versionNumber: [singleVersion.versionNumber],
      checkedOutByName: [singleVersion.checkedOutByName],
      CheckedOutByUID: [singleVersion.checkedOutByUID],
      isChecked: false,
    });
    return dataForm;
  }

  get pipCheckinForm() {
    return this.pipCheckIn.controls.versions as FormGroup;
  }

  onSelect() {
    if (this.selectedVersions.length > 0) {
      this.isSelectionValid = true;
    }
    else {
      this.isSelectionValid = false;
    }
  }

  onCheckIn(formData) {
    const selectedRows: any[] = [];
    this.selectedVersions.forEach(selectedVersion => {
      selectedRows.push(selectedVersion.value);
    });

    this.pipCheckinService.SaveCheckedInVersions(selectedRows).subscribe(success => {
      this.translateService.get('Administration.PipCheckIn.SuccessMessage.PipCheckInSuccess').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
        this.initializeForm();
        this.onSfProjectIdChange(this.selectedProjectId);
        this.selectedVersions = [];
      });
    }, () => {
      this.translateService.get('Administration.PipCheckIn.ErrorMessage.PipCheckInFail').subscribe(msg => {
        this.messageService.add({ severity: 'error', detail: msg });
      });
    });
    this.isSelectionValid = false;
  }
}
