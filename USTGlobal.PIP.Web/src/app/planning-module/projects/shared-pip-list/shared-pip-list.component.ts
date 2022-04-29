import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { FormGroup, FormBuilder, FormArray } from '@angular/forms';
import { Location } from '@angular/common';
import { ISharePIP } from '@shared/domain/ISharePIP';
import { SharePipService } from '@shared/services/share-pip.service';
import { Mastermapper } from '@shared/mapper/master/mastermapper';
import { MessageService, ConfirmationService } from 'primeng/api';
import { SharedDataService } from '@global';
import { ActivatedRoute } from '@angular/router';
import { Constants } from '@shared';
import { Table } from 'primeng/table';
import { Dropdown } from 'primeng/dropdown';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'app-shared-pip-list',
  templateUrl: './shared-pip-list.component.html'
})
export class SharedPipListComponent implements OnInit {
  sharePipListForm: FormGroup;
  cols: any[] = [];
  sharePipList: ISharePIP[] = [];
  shareRoles;
  shareRolesFilter;
  projectId: number;
  headerMessage: string;
  showDeleteDialog = false;
  sharePipToBeDeleted;
  showSearch = false;
  rowIndex = -1;
  filteredSharePipList = [];
  filterUserName = '';
  filterUId = '';
  filterVersion = '';
  filterRoleId = -1;
  sortOrder = false;

  @ViewChild('dt')
  private table: Table;

  @ViewChild('st')
  private st: Dropdown;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private translateService: TranslateService,
    private _location: Location,
    private sharePipService: SharePipService,
    private messageService: MessageService,
    private sharedData: SharedDataService,
    private confirmationService: ConfirmationService
  ) {
    this.route.paramMap.subscribe(data => {
      this.projectId = parseInt(data.get(Constants.uiRoutes.routeParams.projectId), 10);
    });
  }

  ngOnInit() {
    const masterMapper = new Mastermapper();
    this.translateService.get('SharePIPList.SharePIPListColumns').subscribe(cols => {
      this.cols = cols;
    });
    this.initializeForm();
    this.shareRoles = masterMapper.getEditorReadOnlyRole();
    this.shareRolesFilter = masterMapper.getSelectEditorReadOnlyRole();
    this.getSharePipList();

  }
  backClicked() {
    this._location.back();
  }

  initializeForm() {
    this.sharePipListForm = this.fb.group({
      sharePipListData: this.fb.array([])
    });
  }

  get getSharePipForm() {
    return this.sharePipListForm.get('sharePipListData') as FormArray;
  }

  getSharePipList() {
    this.sharePipService.getSharePipListData(this.projectId).subscribe(pip => {
      this.sharePipList = pip;
      this.filteredSharePipList = pip;
      this.initializeForm();

      this.sharePipList.forEach(sharePip => {
        const sp = this.fb.group({
          accountId: [sharePip.accountId, []],
          projectId: [sharePip.projectId, []],
          pipSheetId: [sharePip.pipSheetId, []],
          roleId: [sharePip.roleId, []],
          sharedWithUserName: [sharePip.sharedWithUserName, []],
          sharedWithUId: [sharePip.sharedWithUId, []],
          sharedByUserId: [this.sharedData.sharedData.userRoleAccountDTO.userId, []],
          sharedWithUserId: [sharePip.sharedWithUserId, []],
          versionNumber: [sharePip.versionNumber, []],
          selectedShareRole: [{ value: this.shareRoles.find(x => x.value.id === sharePip.roleId).value, disabled: 'true' }],
          shareComments: [{ value: sharePip.shareComments, disabled: 'true' }],
          isEditClicked: [false, []],
          versionName: [sharePip.versionName],
          disableSave: true,
          disableEdit: false,
          disableDelete: false,
        });
        this.getSharePipForm.push(sp);
      });
    });
  }

  onEditClicked(sharePip, rowIndex: number) {
    sharePip.value.isEditClicked = true;
    sharePip.controls.shareComments.enable();
    sharePip.controls.selectedShareRole.enable();

    // Handle Disable - Enable Buttons
    sharePip.value.disableSave = false;
    sharePip.value.disableDelete = true;
    sharePip.value.disableEdit = true;
  }

  onSaveClick(sharePip, rowIndex: number) {
    const share = this.getSharePipForm.controls[rowIndex] as FormGroup;
    sharePip.value.roleId = sharePip.value.selectedShareRole.id;
    this.sharePipService.updateSharePipListData(sharePip.value).subscribe(result => {
      this.translateService.get('SuccessMessage.SharePipUpdate').subscribe(msg => {
        this.messageService.add({ severity: 'success', detail: msg });
        this.filteredSharePipList[rowIndex].roleId = sharePip.value.roleId;
        this.filteredSharePipList[rowIndex].shareComments = sharePip.value.shareComments;
        const updateIndex = this.sharePipList.findIndex(x => x.pipSheetId === sharePip.value.pipSheetId
          && x.sharedWithUserId === sharePip.value.sharedWithUserId
          && x.versionNumber === sharePip.value.versionNumber);
        this.sharePipList[updateIndex].roleId = sharePip.value.roleId;
        this.sharePipList[updateIndex].shareComments = sharePip.value.shareComments;
        share.controls.selectedShareRole.disable();
        share.controls.shareComments.disable();
        sharePip.value.isEditClicked = false;

        // Handle Disable - Enable Buttons
        sharePip.value.disableSave = true;
        sharePip.value.disableDelete = false;
        sharePip.value.disableEdit = false;
      });
    }, () => {
      this.translateService.get('ErrorMessage.SharePipUpdate').subscribe(msg => {
        this.messageService.add({ severity: 'error', detail: msg });
      });
    });
  }

  onDeleteClick(sharePip: any, rowIndex: number) {
    this.showDeleteDialog = true;
    this.sharePipToBeDeleted = sharePip.value;
    this.rowIndex = rowIndex;
  }

  deleteSharePipAccess() {
    this.sharePipService.deleteSharePipListData(this.sharePipToBeDeleted.pipSheetId, this.sharePipToBeDeleted.roleId,
      this.sharePipToBeDeleted.accountId, this.sharePipToBeDeleted.sharedWithUserId).subscribe(result => {
        this.translateService.get('SuccessMessage.SharePipDelete').subscribe(msg => {
          this.messageService.add({ severity: 'success', detail: msg });
          this.getSharePipForm.controls.splice(this.rowIndex, 1);
          const deletedEntry = this.filteredSharePipList.splice(this.rowIndex, 1);
          const deleteIndex = this.sharePipList.findIndex(x => x.pipSheetId === deletedEntry[0].pipSheetId
            && x.sharedWithUserId === deletedEntry[0].sharedWithUserId
            && x.roleId === deletedEntry[0].roleId
            && x.shareComments === deletedEntry[0].shareComments);
          this.sharePipList.splice(deleteIndex, 1);
          this.sharePipToBeDeleted = null;
        });
      }, () => {
        this.translateService.get('ErrorMessage.SharePipDelete').subscribe(msg => {
          this.messageService.add({ severity: 'error', detail: msg });
        });
        this.sharePipToBeDeleted = null;
      });
  }

  onRoleDropdownChange(sharePip) {
    sharePip.value.disableSave = false;
    sharePip.value.disableDelete = true;
    sharePip.value.disableEdit = true;
  }

  onSearchFilters() {
    this.showSearch = !this.showSearch;
    this.table.reset();
    this.filteredSharePipList = this.sharePipList;
    this.createFilteredForm(false);
  }

  userNameFilter(event) {
    this.filterUserName = event;
    this.advancedFilter();
  }

  uIdFilter(event) {
    this.filterUId = event;
    this.advancedFilter();
  }

  versionFilter(event) {
    this.filterVersion = event;
    this.advancedFilter();
  }

  roleFilter(event) {
    this.filterRoleId = event;
    this.advancedFilter();
  }

  advancedFilter() {
    this.filteredSharePipList = this.sharePipList.filter(a =>
      a.sharedWithUserName.toLowerCase().includes(this.filterUserName.toLowerCase())
      && a.sharedWithUId.toString().toLowerCase().includes(this.filterUId.toLowerCase())
      && a.versionName.toLowerCase().includes(this.filterVersion.toLowerCase())
    );
    if ((isNullOrUndefined(this.filterRoleId) ? -1 : this.filterRoleId) !== -1) {
      this.filteredSharePipList = this.filteredSharePipList.filter(a =>
        a.roleId === this.filterRoleId);
    }
    this.createFilteredForm(true);
  }

  createFilteredForm(isFilteredOrDefault: boolean) {
    this.initializeForm();
    if (isFilteredOrDefault) {
      this.filteredSharePipList.forEach(sharePip => {
        const sp = this.fb.group({
          accountId: [sharePip.accountId, []],
          projectId: [sharePip.projectId, []],
          pipSheetId: [sharePip.pipSheetId, []],
          roleId: [sharePip.roleId, []],
          sharedWithUserName: [sharePip.sharedWithUserName, []],
          sharedWithUId: [sharePip.sharedWithUId, []],
          sharedByUserId: [this.sharedData.sharedData.userRoleAccountDTO.userId, []],
          sharedWithUserId: [sharePip.sharedWithUserId, []],
          versionNumber: [sharePip.versionNumber, []],
          selectedShareRole: [{ value: this.shareRoles.find(x => x.value.id === sharePip.roleId).value, disabled: 'true' }],
          shareComments: [{ value: sharePip.shareComments, disabled: 'true' }],
          isEditClicked: [false, []],
          versionName: [sharePip.versionName],
          disableSave: true,
          disableEdit: false,
          disableDelete: false,
        });
        this.getSharePipForm.push(sp);
      });
    } else {
      this.sharePipList.forEach(sharePip => {
        const sp = this.fb.group({
          accountId: [sharePip.accountId, []],
          projectId: [sharePip.projectId, []],
          pipSheetId: [sharePip.pipSheetId, []],
          roleId: [sharePip.roleId, []],
          sharedWithUserName: [sharePip.sharedWithUserName, []],
          sharedWithUId: [sharePip.sharedWithUId, []],
          sharedByUserId: [this.sharedData.sharedData.userRoleAccountDTO.userId, []],
          sharedWithUserId: [sharePip.sharedWithUserId, []],
          versionNumber: [sharePip.versionNumber, []],
          selectedShareRole: [{ value: this.shareRoles.find(x => x.value.id === sharePip.roleId).value, disabled: 'true' }],
          shareComments: [{ value: sharePip.shareComments, disabled: 'true' }],
          isEditClicked: [false, []],
          versionName: [sharePip.versionName],
          disableSave: true,
          disableEdit: false,
          disableDelete: false,
        });
        this.getSharePipForm.push(sp);
      });
    }
  }

  sortByUserName() {
    if (this.sortOrder) {
      // Ascending
      this.filteredSharePipList = this.filteredSharePipList.sort((a, b) => a.sharedWithUserName.localeCompare(b.sharedWithUserName));
      this.sortOrder = false;
    } else {
      // Descending
      this.filteredSharePipList = this.filteredSharePipList.sort((a, b) => b.sharedWithUserName.localeCompare(a.sharedWithUserName));
      this.sortOrder = true;
    }
    this.createFilteredForm(true);
  }
}
