import { UserWorkflowService } from './../../shared-module/services/user-workflow.service';
import { UserRoleService } from '@shared/services/user-role.service';
import { ICheckRole } from './../../shared-module/domain/ICheckRole';
import { IRoleAndAccount } from './../../shared-module/domain/IRoleAndAccount';
import { IWorkflowFlag } from './../../shared-module/domain/IWorkflowFlag';
import { IPipSheetWorkflowStatus } from './../../shared-module/domain/IPipSheetWorkflowStatus';
import { Component, OnInit, ViewChild, EventEmitter, Output, Type } from '@angular/core';
import { Router } from '@angular/router';
import { ProjectService } from '@shared/services/project.service';
import { IProject } from '@shared/domain/project';
import { SelectItem, MenuItem, ConfirmationService, MessageService, DialogService } from 'primeng/api';
import { Mastermapper } from '@shared/mapper/master/mastermapper';
import { Table } from 'primeng/table';
import { SharedDataService } from '@global';
import { Constants } from '../../shared-module/infrastructure';
import { IPipVersion } from '@shared/domain/IPipVersion';
import { TranslateService } from '@ngx-translate/core';
import { Dropdown } from 'primeng/dropdown';
import { IPipCheckIn } from '@shared/domain';
import { CheckOutDialogComponent } from './CheckOutDialog/CheckOutDialog.component';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html'
})
export class ProjectsComponent implements OnInit {
  pipCheckIn: IPipCheckIn;
  projects: IProject[];
  accountNameEntities: SelectItem[];
  contractingEntities: SelectItem[];
  servicePorfolios: SelectItem[];
  serviceLines: SelectItem[];
  deliveryTypes: SelectItem[];
  billingType: SelectItem[];
  status: SelectItem[];
  loadComponent = false;
  cols: any[];
  showSearch = false;
  projectIDMenuItems: MenuItem[];
  projectForLastShownVersionMenu: IProject;
  pipSheetWorkflowStatus: IPipSheetWorkflowStatus;
  workflowFlag: IWorkflowFlag;
  wfstatus: any;
  roleAndAccount: IRoleAndAccount[];
  roleAccountForCheckIn: IRoleAndAccount[];
  checkRole: ICheckRole;
  checkRoleForCheckInOut: ICheckRole;
  loggedInUserId: number;
  isDataAvailable = false;
  dashboardId = 1; // 1 represents project list grid
  displayDelete = false;
  showOnlyReadOnly = 3;
  isDummy = false;
  isEditor = false;

  @ViewChild('dt')
  private table: Table;

  @ViewChild('st')
  private st: Dropdown;

  @ViewChild('ce')
  private ce: Dropdown;

  @ViewChild('sl')
  private sl: Dropdown;

  @ViewChild('sp')
  private sp: Dropdown;

  @ViewChild('dtype')
  private dtype: Dropdown;

  @ViewChild('bt')
  private bt: Dropdown;

  constructor(
    private projectService: ProjectService,
    private router: Router,
    private sharedData: SharedDataService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private translateService: TranslateService,
    private dialogService: DialogService,
    private userWorkflowService: UserWorkflowService
  ) {
  }

  ngOnInit() {
    this.projectService.getProjects().subscribe(value => {
      //  this.projects = value;
      this.isEditor = value.isEditor;
      this.checkType(this.router.url, value.projectDTO);
      this.isDataAvailable = true;
    }, () => {
      this.translateService.get('ErrorMessage.ProjectList').subscribe(msg => {
        this.messageService.add({ severity: 'error', detail: msg });
      });
    });

    this.contractingEntities = new Mastermapper().getContractingEntityComboItems(this.sharedData.sharedData.contractingEntityDTO);
    this.servicePorfolios = new Mastermapper().getServicePortfolioComboItems(this.sharedData.sharedData.servicePortfolioDTO);
    this.serviceLines = new Mastermapper().getServiceLineComboItems(this.sharedData.sharedData.serviceLineDTO);
    this.deliveryTypes = new Mastermapper().getDeliveryTypesComboItems(this.sharedData.sharedData.projectDeliveryTypeDTO);
    this.billingType = new Mastermapper().getBillingTypesComboItems(this.sharedData.sharedData.projectBillingTypeDTO);

    this.status = [
      { label: '--- Select --- ', value: { 'id': -1, 'code': '', 'name': null } },
      { label: 'Not Submitted', value: { id: 1, name: 'Not Submitted', code: 'NS' } },
      { label: 'Approved', value: { id: 2, name: 'Approved', code: 'A' } },
      { label: 'Approval Pending', value: { id: 3, name: 'Approval Pending', code: 'AP' } }
    ];

    this.translateService.get('SHARED.WORKFLOWSTATUS').subscribe(wfstatus => {
      this.wfstatus = wfstatus;
    });
    this.roleAndAccount = this.sharedData.sharedData.userRoleAccountDTO.roleAndAccountDTO;
    this.loggedInUserId = this.sharedData.sharedData.userRoleAccountDTO.userId;
    if (this.roleAndAccount != null) {
      this.checkRole = this.userWorkflowService.getUserSpecificRoles(this.roleAndAccount);
    }
  }

  checkType(url: string, data: IProject[]) {
    if (url === '/projects') {
      this.projects = data.filter(project => project.isDummy === false);
      this.isDummy = false;
      this.sharedData.isDummy = this.isDummy;
    } else {
      this.projects = data.filter(project => project.isDummy === true);
      this.isDummy = true;
      this.sharedData.isDummy = this.isDummy;
    }
  }

  viewOlderVersions(): () => void {
    return () => {
      this.router.navigate([
        this.isDummy ? Constants.uiRoutes.sample : Constants.uiRoutes.projects,
        this.projectForLastShownVersionMenu.projectId,
        'versions'
      ]);
    };
  }

  onClearFilters() {
    this.table.reset();
    this.resetDropDowns();
  }

  resetDropDowns() {
    const item = { label: '--- select ---', value: { 'id': -1, 'code': '', 'name': null } };
    this.ce.selectedOption = item;
    this.st.selectedOption = item;
    this.dtype.selectedOption = item;
    this.sp.selectedOption = item;
    this.sl.selectedOption = item;
    this.bt.selectedOption = item;
  }

  navigateToVersionGrid(projectId: number): void {
    this.router.navigate([
      this.isDummy ? Constants.uiRoutes.sample : Constants.uiRoutes.projects,
      projectId,
      'versions',
    ]);
  }

  onSearchFilters() {
    this.showSearch = !this.showSearch;
  }

  onRefreshClick() {
    this.projectService.getProjects().subscribe(value => {
      //  this.projects = value;
      this.checkType(this.router.url, value.projectDTO);
    }, () => {
      this.translateService.get('ErrorMessage.ProjectList').subscribe(msg => {
        this.messageService.add({ severity: 'error', detail: msg });
      });
    });
  }
}
