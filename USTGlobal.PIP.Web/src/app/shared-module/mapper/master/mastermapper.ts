import { IAccountId } from './../../domain/IAccountId';
import { IPipCheckInProject } from './../../domain/IPipCheckInProject';
import { IRoleAndAccountMain } from './../../domain/IRoleAndAccountMain';
import { UserComboItem } from './../../domain/usercomboitem';
import { IUser } from './../../../global-module/domain/IUser';
import { IMapper } from '../imapper';
import { IMasters } from '@shared/domain/master';
import { SelectItem } from 'primeng/api';
import { ComboItems } from '@shared/domain/comboitems';
import { MultiSelectItem } from 'primeng/multiselect';
import { ComboItemValue } from '@shared/domain/comboitemvalue';
import { IContractingEntity } from '@shared/domain/contractingentity';
import { IServicePortfolio } from '@shared/domain/serviceportfolio';
import { IServiceLine } from '@shared/domain/serviceline';
import { IDeliveryType } from '@shared/domain/deliverytype';
import { IBillingType } from '@shared/domain/billingtype';
import { ILocation } from '@shared/domain/locations';
import { LocationComboItem } from '@shared/domain/locationcomboitem';
import { IResourceRoleGroup } from '@shared/domain/IResourceRoleGroup';
import { IResourceUSTRole } from '@shared/domain/IResourceUSTRole';
import { IResourceMarkup } from '@shared/domain/IResourceMarkup';
import { IProjectMilestone } from '@shared/domain/IProjectMilestone';
import { IHoliday } from '@shared/domain/IHoliday';
import { HolidayComboItem } from '@shared/domain/HolidayComboItem';
import { IProjectMilestoneGroup } from '@shared/domain/IProjectMilestoneGroup';
import { ICurrency } from '../../domain/ICurrency';
import { ICountry } from '../../domain/ICountry';
import { ICorporateTarget } from '@shared/domain/ICorporateTarget';
import { IYear } from '@shared/domain/IYear';
import { INonBillableCategory } from '@shared/domain/INonBillableCategory';
import { IAccountNameEntity } from '@shared/domain/IAccountNameEntity';
import { IRoleAndAccount } from '@shared/domain/IRoleAndAccount';
import { IUserRole } from '@shared/domain/IUserRole';
import { ICheckRole } from '@shared/domain/ICheckRole';
import { IProject } from '@shared/domain';
import { IProjectListForAccounts } from '@global/domain/IProjectListForAccount';
import { IReportKPI } from '@shared/domain/IReportKPI';
import { ISelectedKPI } from '@shared/domain/ISelectedKPI';

export class Mastermapper implements IMasters, IMapper {
  masterId: any;
  masterName: any;

  mapper(input: any): this {
    Object.assign(this, input);
    return this;
  }

  getMasterComboItems(MasterList: any): Array<SelectItem> {
    const items: Array<SelectItem> = [];

    const itemValue: Array<ComboItemValue> = [];
    if (MasterList) {
      MasterList.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.masterName;

        // item value
        subItems.id = element.masterId;
        subItems.code = element.masterName;
        subItems.name = element.masterName;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }

  }

  getContractingEntityComboItems(data: IContractingEntity[]): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();
    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.code + ' ' + element.name;

        // item value
        subItems.id = element.contractingEntityId;
        subItems.code = element.code;
        subItems.name = element.name;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }

  }

  // Get account data based on editor role only
  getAccountNameEntityComboItemsOnProjectHeader(accountName: IAccountNameEntity[], roleAndAccountMain: IRoleAndAccountMain,
    selectedPipSheetId: number, dashboardId: number, flag: boolean, addFirstItem: boolean, accountId: number): Array<SelectItem> {
    // const items: Array<SelectItem> = [];
    let items: Array<SelectItem> = [];
    if (addFirstItem) {
      items = this.addFirstItem();
    }
    let accessAccounts = [], tempAccessAccount: IRoleAndAccount[] = [];

    if (selectedPipSheetId === 0 && dashboardId === 1) {
      tempAccessAccount = roleAndAccountMain.roleAndAccountDTO.filter(role => role.roleId === 3); // role id 3 for editor role
      roleAndAccountMain.accountLevelAccessIds.forEach(accId => {
        tempAccessAccount.forEach(account => {
          if (accId.accountLevelAccessIds === account.accountId) {
            accessAccounts.push(account);
          }
        });
      });
    }
    else if (dashboardId === 1 && !flag) {
      accessAccounts = roleAndAccountMain.roleAndAccountDTO.filter(role => role.roleId === 3); // role id 3 for editor role

      if (!accessAccounts.find(x => x.accountId === accountId)) {
        const sharedPip = roleAndAccountMain.sharedAccountRoles.filter(role => role.accountId === accountId)[0];
        if (sharedPip) {
          accessAccounts.push(sharedPip);
        }
      }
    }
    else if (dashboardId === 1 || dashboardId === 3) {
      accessAccounts = roleAndAccountMain.roleAndAccountDTO.filter(role => role.roleId === 3 ||
        role.roleId === 5); //  3 - Editor, 5 - ReadOnly

      if (!accessAccounts.find(x => x.accountId === accountId)) {
        const sharedPip = roleAndAccountMain.sharedAccountRoles.filter(role => role.accountId === accountId)[0];
        if (sharedPip) {
          accessAccounts.push(sharedPip);
        }
      }
    }
    else {
      accessAccounts = roleAndAccountMain.roleAndAccountDTO;
    }

    if (accessAccounts.length > 0) {
      accessAccounts.forEach(editorAccessAcc => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();
        const editorAccountData = accountName.filter(editorAccount => editorAccount.accountId === editorAccessAcc.accountId);
        if (editorAccountData.length > 0) {
          editorAccountData.forEach(singleAccount => {
            // item
            item.label = singleAccount.accountName;
            // item value
            subItems.id = singleAccount.accountId;
            item.value = subItems;
            items.push(item);
          });
        }
      });
    }
    return items;
  }

  getAccountNameForEditorRole(accountName: IAccountNameEntity[], roleAndAccountMain: IRoleAndAccountMain,
    addFirstItem: boolean): Array<SelectItem> {
    // const items: Array<SelectItem> = [];
    let items: Array<SelectItem> = [];
    if (addFirstItem) {
      items = this.addFirstItem();
    }
    const accessAccounts = [];
    let tempAccessAccount: IRoleAndAccount[] = [];
    tempAccessAccount = roleAndAccountMain.roleAndAccountDTO.filter(role => role.roleId === 3); // role id 3 for editor role
    roleAndAccountMain.accountLevelAccessIds.forEach(accountId => {
      tempAccessAccount.forEach(account => {
        if (accountId.accountLevelAccessIds === account.accountId) {
          accessAccounts.push(account);
        }
      });
    });

    if (accessAccounts.length > 0) {
      accessAccounts.forEach(editorAccessAcc => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();
        const editorAccountData = accountName.filter(editorAccount => editorAccount.accountId === editorAccessAcc.accountId);
        if (editorAccountData.length > 0) {
          editorAccountData.forEach(singleAccount => {
            // item
            item.label = singleAccount.accountName;
            // item value
            subItems.id = singleAccount.accountId;
            item.value = subItems;
            items.push(item);
          });
        }
      });
    }
    return items;
  }

  getAccountNameEntityComboItems(accountName: IAccountNameEntity[], roleAndAccount: IRoleAndAccount[])
    : Array<SelectItem> {
    const items: Array<SelectItem> = [];
    const distinctAccounts = roleAndAccount.filter(item => item.roleId === 2);

    if (accountName && distinctAccounts) {
      accountName.forEach(element => {
        distinctAccounts.forEach(elm => {
          if (element.accountId === elm.accountId) {
            const item = new ComboItems();
            const subItems = new ComboItemValue();

            // item
            item.label = element.accountName;

            // item value
            subItems.id = element.accountId;
            subItems.name = element.accountName;

            item.value = subItems;
            items.push(item);
          }
        });
      });
      return items;
    }
  }

  getRoleSpecifcAccountNameEntityComboItems(accountName: IAccountNameEntity[], roleAndAccount: IRoleAndAccount[],
    checkRole: ICheckRole, selectedAcc: number): Array<SelectItem> {
    const items: Array<SelectItem> = [];
    let distinctAccounts: IRoleAndAccount[];

    if (checkRole.isEditor) {
      distinctAccounts = roleAndAccount.filter(val => val.roleId === 3);
    }
    else {
      distinctAccounts = roleAndAccount.filter(
        (thing, i, arr) => arr.findIndex(t => t.accountId === thing.accountId) === i
      );
    }

    if (accountName && distinctAccounts) {
      accountName.forEach(element => {
        distinctAccounts.forEach(elm => {
          if (elm.roleId === 3) {
            if (element.accountId === elm.accountId) {
              const item = new ComboItems();
              const subItems = new ComboItemValue();

              // item
              item.label = element.accountName;

              // item value
              subItems.id = element.accountId;

              item.value = subItems;
              items.push(item);
            }
          }
          else {
            if (element.accountId === elm.accountId) {
              const item = new ComboItems();
              const subItems = new ComboItemValue();

              // item
              item.label = element.accountName;

              // item value
              subItems.id = element.accountId;

              item.value = subItems;
              items.push(item);
            }
          }
        });
      });
      return items;
    }
  }

  getServicePortfolioComboItems(data: IServicePortfolio[]): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();

    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.portfolioName;

        // item value
        subItems.id = element.servicePortfolioId;
        subItems.code = element.portfolioName;
        subItems.name = element.portfolioName;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }

  }

  getServiceLineComboItems(data: IServiceLine[]): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();

    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.serviceLineName;

        // item value
        subItems.id = element.serviceLineId;
        subItems.code = element.servicePortfolioId;  // temporary.. need to discuss.
        subItems.name = element.serviceLineName;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }

  }

  getDeliveryTypesComboItems(data: IDeliveryType[]): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();

    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.deliveryType;

        // item value
        subItems.id = element.projectDeliveryTypeId;
        subItems.code = element.projectDeliveryTypeId;
        subItems.name = element.deliveryType;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }

  }

  getBillingTypesComboItems(data: IBillingType[]): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();

    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.billingTypeName;

        // item value
        subItems.id = element.projectBillingTypeId;
        subItems.code = element.projectBillingTypeId;
        subItems.name = element.billingTypeName;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }

  }

  getLocationComboItems(data: ILocation[], addFirstItem: boolean = false): Array<SelectItem> {
    let items: Array<SelectItem> = [];
    if (addFirstItem) {
      items = this.addFirstItem();
    }

    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new LocationComboItem();
        // item
        item.label = element.locationName;

        // item value
        subItems.id = element.locationId;
        subItems.name = element.locationName;
        subItems.codeperday = element.hoursPerDay;
        subItems.codepermonth = element.hoursPerMonth;

        item.value = subItems;
        items.push(item);
      });
      return items;
    }
  }

  getYearComboItems(data: IYear[], addFirstItem: boolean = false): Array<SelectItem> {
    let items: Array<SelectItem> = [];
    if (addFirstItem) {
      items = this.addFirstItem();
    }
    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.year;

        // item value
        subItems.id = element.yearId;
        subItems.name = element.year;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }
  }

  getMilestoneGroupComboItems(data: IProjectMilestoneGroup[]): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();
    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.groupName;

        // item value
        subItems.id = element.milestoneGroupId;
        subItems.name = element.groupName;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }

  }

  getResourceRoleGroupComboItems(data: IResourceRoleGroup[]): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();
    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.groupName;

        // item value
        subItems.id = element.resourceGroupId;
        subItems.name = element.groupName;
        subItems.code = element.locationId;
        // subItems.code = element.masterVersionId;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }

  }

  getResourceUSTRoleComboItems(data: IResourceUSTRole[]): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();
    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.name;

        // item value
        subItems.id = element.resourceId;
        subItems.name = element.name;
        subItems.code = element.resourceGroupId;
        //  subItems.name = element.grade;
        // subItems.code = element.masterVersionId;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }

  }

  getResourceMarkupComboItems(data: IResourceMarkup[]): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();
    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.name;

        // item value
        subItems.id = element.markupId;
        subItems.code = element.percent;
        subItems.name = element.name;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }
  }

  getResourceHolidayComboItems(data: IHoliday[]): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();
    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new HolidayComboItem();

        // item
        item.label = element.holidayName;

        // item value
        subItems.holidayId = element.id;
        subItems.holidayName = element.holidayName;
        subItems.holidayDate = element.date;
        subItems.locationId = element.locationId;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }
  }

  getOptionalPhaseComboItems(data: IProjectMilestone[], addFirstItem: boolean): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();
    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.milestoneName;

        // item value
        subItems.id = element.projectMilestoneId;
        // subItems.code = element.milestoneGroupId;
        subItems.name = element.milestoneName;
        subItems.code = element.milestoneId;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }

  }



  getInvestmentPercentage(data: ICorporateTarget[]): Array<any> {
    const items: Array<SelectItem> = [];
    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();
        item.label = element.percent + '%' + ' ' + element.description;

        subItems.id = element.corporateTargetId;
        subItems.code = element.percent;

        item.value = subItems;
        items.push(item);
      });
    }
    return items;
  }

  getCountryDetails(country: ICountry[], currency: ICurrency[]): Array<SelectItem> {
    const items: Array<SelectItem> = [];

    if (country && currency) {
      country.forEach(element => {
        currency.forEach(elm => {
          if (element.countryId === elm.countryId) {
            const item = new ComboItems();
            const subItems = new ComboItemValue();

            // item
            item.label = element.name + ' - ' + elm.symbol;

            // item value
            subItems.id = element.countryId;
            subItems.name = element.name;

            item.value = subItems;
            items.push(item);
          }
        });
      });
      return items;
    }
  }

  getTargetMargin(): Array<SelectItem> {

    const items: Array<SelectItem> = [];
    items.push(
      {
        label: 'EBITDA %',
        value: {
          id: 1,
          name: 'ebitda'
        }
      },
      {
        label: 'GM % (Based on Net Rev)',
        value: {
          id: 2,
          name: 'netRevenue'
        }
      },
      {
        label: 'GM % (Based on Client Price)',
        value: {
          id: 3,
          name: 'price'
        }
      });


    return items;

  }


  getUtilizationDetails(): Array<SelectItem> {
    const items: Array<SelectItem> = [];
    items.push(
      {
        label: 'Billable',
        value: {
          id: true,
          name: 'Billable'
        }
      },
      {
        label: 'Non-billable',
        value: {
          id: false,
          name: 'non-billable'
        }
      });

    return items;
  }


  getFirstItem() {
    const item: Array<SelectItem> = this.addFirstItem();
    return item;
  }

  private addFirstItem(): Array<SelectItem> {
    const items: Array<SelectItem> = [];
    const subItem = {
      'id': -1,
      'code': '',
      'name': null
    };

    const item: SelectItem = { label: '--- select ---', value: subItem };
    items.push(item);
    return items;
  }
  getNonBillableCategorieComboItems(nonBillableCategory: INonBillableCategory[]): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();
    if (nonBillableCategory) {
      nonBillableCategory.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();
        // item
        item.label = element.category;
        // item value
        subItems.id = element.nonBillableCategoryId;
        subItems.name = element.category;
        item.value = subItems;
        items.push(item);
      });
    }
    return items;
  }

  getAccountComboItems(account: IAccountNameEntity[], addFirstItem: boolean): Array<SelectItem> {
    let items: Array<SelectItem> = [];
    if (addFirstItem) {
      items = this.addFirstItem();
    }

    // const items: Array<SelectItem> = [];
    if (account) {
      account.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();
        // item
        item.label = element.accountName;
        // item value
        subItems.id = element.accountId;
        subItems.name = element.accountName;
        subItems.code = element.accountCode;
        subItems.value = element.paymentLag;
        item.value = subItems;
        if (element.accountId !== 0) {
          items.push(item);
        }
      });
    }
    return items;
  }

  getAccountComboItemsForReport(account: IAccountNameEntity[], authAccountIds: IAccountId[],
    hasAccountLevelEditorAccess: boolean, hasFinanceApproverAccess: boolean,
    addFirstItem: boolean): Array<SelectItem> {
    let items: Array<SelectItem> = [];
    if (addFirstItem) {
      items = this.addFirstItem();
    }
    let accessAccounts = [];

    if (hasFinanceApproverAccess) {
      accessAccounts = account;
    }
    else if (hasAccountLevelEditorAccess && !hasFinanceApproverAccess) {
      account.forEach(masterAccount => {
        authAccountIds.forEach(authAccount => {
          if (masterAccount.accountId === authAccount.accountLevelAccessIds) {
            accessAccounts.push(masterAccount);
          }
        });
      });
    }

    // const items: Array<SelectItem> = [];
    if (accessAccounts) {
      accessAccounts.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();
        // item
        item.label = element.accountName;
        // item value
        subItems.id = element.accountId;
        subItems.name = element.accountName;
        subItems.code = element.accountCode;
        subItems.value = element.paymentLag;
        item.value = subItems;
        if (element.accountId !== 0) {
          items.push(item);
        }
      });
    }
    return items;
  }

  getPipCheckInProjectComboItems(project: IPipCheckInProject[]): Array<SelectItem> {
    const items: Array<SelectItem> = [];
    if (project) {
      project.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();
        // item
        item.label = element.sfProjectId;
        // item value
        subItems.id = element.projectId;
        subItems.name = element.projectName;
        item.value = subItems;
        items.push(item);
      });
    }
    return items;
  }

  getAccountComboItemsForUserList(account: IAccountNameEntity[]): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();
    if (account) {
      account.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();
        if (element.accountName !== 'Admin') {
          // item
          item.label = element.accountName;
          // item value
          subItems.id = element.accountId;
          subItems.name = element.accountName;
          item.value = subItems;
        } else {
          item.label = 'All Accounts';
          // item value
          subItems.id = element.accountId;
          subItems.name = 'All Accounts';
          item.value = subItems;

        }
        items.push(item);

      });
      // items.shift();
    }
    return items;
  }

  getNewUserUIDsComboItems(users: IUserRole[]): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();
    if (users) {
      users.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();
        // item
        item.label = element.uid + ' - ' + element.firstName + ' ' + element.lastName;
        // item value
        subItems.id = element.uid;
        subItems.name = element.uid;
        item.value = subItems;
        items.push(item);
      });
    }
    return items;
  }

  getEditorReadOnlyRole(): Array<SelectItem> {
    const items: Array<SelectItem> = [];
    items.push(
      {
        label: 'Editor',
        value: {
          id: 3,
          name: 'editor'
        }
      },
      {
        label: 'Read Only',
        value: {
          id: 5,
          name: 'readOnly'
        }
      },
    );
    return items;
  }

  getSelectEditorReadOnlyRole(): Array<SelectItem> {
    const items: Array<SelectItem> = [];
    items.push(
      {
        label: '--- select ---',
        value: {
          id: -1,
          name: 'select'
        }
      },
      {
        label: 'Editor',
        value: {
          id: 3,
          name: 'editor'
        }
      },
      {
        label: 'Read Only',
        value: {
          id: 5,
          name: 'readOnly'
        }
      },
    );
    return items;
  }

  getUserComboItems(userData: IUser[], addFirstItem: boolean = false): Array<SelectItem> {
    let items: Array<SelectItem> = [];
    if (addFirstItem) {
      items = this.addFirstItem();
    }

    if (userData.length > 0) {
      userData.forEach(element => {
        const item = new ComboItems();
        const subItems = new UserComboItem();
        // item
        item.label = element.firstName + element.lastName;

        // item value
        subItems.userId = element.userId;
        subItems.userName = element.firstName + element.lastName;

        item.value = subItems;
        items.push(item);
      });
      return items;
    }
  }
  getProjectListComboItems(data: IProject[], addFirstItem: boolean): Array<SelectItem> {
    const items: Array<SelectItem> = this.addFirstItem();

    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.sfProjectId;

        // item value
        subItems.id = element.projectId;
        subItems.code = element.accountId;
        subItems.name = element.sfProjectId;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }
  }
  getProjectListForAccountComboItems(data: IProjectListForAccounts[], addFirstItem: boolean): Array<SelectItem> {
    let items: Array<SelectItem> = [];
    if (addFirstItem) {
      items = this.addFirstItem();
    }
    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.sfProjectId;

        // item value
        subItems.id = element.projectId;
        subItems.code = element.sfProjectId;
        subItems.name = element.projectName;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }
  }

  getReportKPIComboItems(data: ISelectedKPI[]): Array<SelectItem> {
    const items: Array<SelectItem> = [];

    if (data) {
      data.forEach(element => {
        const item = new ComboItems();
        const subItems = new ComboItemValue();

        // item
        item.label = element.kpiName;

        // item value
        subItems.id = element.kpiId;
        subItems.code = element.kpiName;
        subItems.name = element.plForecastLabelId;

        item.value = subItems;
        items.push(item);
      });

      return items;
    }
  }
}
