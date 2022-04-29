
import { ConfigurationSettings } from './configuration-settings';
import { environment } from '@env';
import { ComboItems } from '@shared';

export class Constants {

  static defaultCurrencyId = 23; // USD = 23

  static regExType = {
    numeric: /^\d+$/,
    alphanumeric: /^[a-zA-Z0-9]*$/,
    alphanumericWithSpace: /^[a-zA-Z0-9 ]*$/,
    alphanumWithSpecial1: /^[a-zA-Z0-9!''#$%&( )*+,./:;=?@^_-]*$/,
    decimalPrecisionFour: /^([0-9]*([.]{1}[0-9]{0,4})?)$/,
    decimalPrecisionTwo: /^((?!\.)[0-9]*([.]{1}[0-9]{0,2})?)$/,
    negativedecimalPrecisionFour: /^(-?[0-9]*([.]{1}[0-9]{0,4})?)$/,
    negativeDecimalPrecisionTwo: /^(-?[0-9]*([.]{1}[0-9]{0,2})?)$/,
    alph1anum1: '^.*(?=.{7,})(?=.*[0-9])(?=.*[a-zA-Z]).*$',
    phoneKey: /^[a-zA-Z0-9( )-]*$/,
    email: /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/,
    projectName: '^([a-zA-Z0-9]{4,})\+-([0-9]{4,})\+-([0-9]{2,})\+-([0-9]{2,})\+$',
    // dummyProjectId: '^([a-zA-z]{4,})\+-([a-zA-z]{4,})\+-([0-9]{1,})\+$',
    percentageWithDecimalPrecisionTwo: /(^100(\.0{1,2})?$)|(^([1-9]([0-9])?|0)(\.[0-9]{1,2})?$)/,
    percentageLessThanEqual100WithDecimalPrecisionTwo: /(^99(\.0{1,2})?$)|(^([1-9]([0-9])?|0)(\.[0-9]{1,2})?$)/,
    notAllowSpaceInBeginning: /^\s*(\w.*)$/,
    alphanumericSpaceNotAllowedInBeginning: /^^(\w+ ?)*$/,
    milestoneMonthFormat: '^([a-zA-z]{3})\+-([2]{1})([0]{1})([1-9]{1})([0-9]{1})\$',
    decimalPrecisionTwoWithNaturalNumbers: /^((?!\.)[1-9][0-9]*([.]{1}[0-9]{0,2})?)$/
  };

  static cookies =
    {
      sessionId: 'SessionId'
    };

  static requestHeader =
    {
      authorization: 'Authorization',
      sessionId: 'SessionId',
      bearer: 'Bearer',
      accept: 'Accept',
      contentType: 'Content-Type'
    };

  static apiToken = {
    refreshToken: 'grant_type=refresh_token&client_id=web&refresh_token='
  };

  static contentType =
    {
      json: 'application/json; charset=utf-8',
      formUrlEncoded: 'application/x-www-form-urlencoded',
      multiPart: 'multipart/form-data'
    };

  static uiRoutes = {
    empty: '',
    default: ConfigurationSettings.defaultRoutePrefix,
    login: 'login',
    resetpassword: 'resetpassword',
    dashboard: 'dashboard',
    projects: 'projects',
    sample: 'samples',
    versions: 'versions',
    administration: 'administration',
    userRolesAndPermission: 'userroles',
    staff: 'Staff',
    margin: 'Margin',
    estimate: 'Estimate',
    summary: 'Summary',
    reports: 'reports',
    masters: 'masters',
    defineFinancePoc: 'defineFinancePoc',
    defineAdmin: 'defineAdmin',
    defineReadOnly: 'defineReadOnly',
    pipCheckIn: 'pipCheckIn',
    addNewUser: 'addNewUser',
    listUsers: 'userList',
    approver: 'approver',
    sharePIP: 'sharePIP',
    sharePIPList: 'sharepiplist',
    routeParams: {
      projectId: 'projectId',
      pipSheetId: 'pipSheetId',
      accountId: 'accountId',
      dashboardId: 'dashboardId'
    }
  };

  static breadcrumbLabels = {
    dashboard: 'Dashboard',
    projects: 'Projects',
    administration: 'Admin',
    masters: 'Masters',
    staff: 'Staff',
    margin: 'Margin',
    estimate: 'Estimate',
    summary: 'Summary',
    reports: 'Reports',
    versions: 'Versions',
  };

  static webApis = {
    login: environment.apiUrl + 'auth/login',
    logout: environment.apiUrl + 'auth/logout',
    getUserData: environment.apiUrl + 'user',
    getSharedData: environment.apiUrl + 'pipSheet/{pipSheetId}/sharedData',
    getProjectHeaderData: environment.apiUrl + 'projectHeader/project/{projectId}/pipSheet/{pipSheetId}',
    saveProjectHeaderData: environment.apiUrl + 'projectHeader',
    getAllProjectBillingByDelivery: environment.apiUrl + 'master/projectDelivery/{projectDeliveryId}/billing',
    getAllProjectBillingData: environment.apiUrl + 'master/projectBilling',
    getAllProjectDeliveryData: environment.apiUrl + 'master/projectDelivery',
    getAllServiceLines: environment.apiUrl + 'master/serviceLines',
    getAllServicePortfolios: environment.apiUrl + 'master/servicePortfolios',
    getAllContractingEntities: environment.apiUrl + 'master/contractingEntities',
    getProjectsList: environment.apiUrl + 'project/list',
    getAllLocations: environment.apiUrl + 'master/project/{projectId}/pipSheet/{pipSheetId}/locations',
    getAllProjectControlData: environment.apiUrl + 'pipSheet/{pipSheetId}/projectControl',
    saveProjectControlData: environment.apiUrl + 'pipSheet/projectControl',
    getAllResourceLocations: environment.apiUrl + 'pipSheet/{pipSheetId}/locations',
    getAllResourceOptionalPhase: environment.apiUrl + 'pipSheet/{pipSheetId}/milestones',
    saveResourcePlanningData: environment.apiUrl + 'resourcePlanning',
    getResourcePlanningData: environment.apiUrl + 'resourcePlanning/{pipSheetId}',
    getAllProjectMilestonesGroups: environment.apiUrl + 'master/milestoneGroups',
    getHeader1Data: environment.apiUrl + 'project/{projectId}/pipSheet/{pipSheetId}/header1Data',
    getCurrencyConversionDetailsByCountryId: environment.apiUrl + 'currency/country/{countryId}',
    saveOrUpdatePIPSheetCurrency: environment.apiUrl + 'pipSheet/{pipSheetId}',
    getAllPipVersions: environment.apiUrl + 'project/{projectId}/pipSheet/versions',
    createNewPipSheetVersion: environment.apiUrl + 'pipSheet/version/',
    saveEbitdaData: environment.apiUrl + 'ebitda/',
    getEbitdaData: environment.apiUrl + 'ebitda/{pipSheetId}',
    saveLaborPricingData: environment.apiUrl + 'laborPricing',
    getLaborPricingData: environment.apiUrl + 'laborPricing/',
    getBackgroundCalculations: environment.apiUrl + 'laborPricing/backgroundFields',
    getVacationAbsence: environment.apiUrl + 'vacationAbsences/',
    saveVacationAbsence: environment.apiUrl + 'vacationAbsences/',
    getPriceAdjustmentData: environment.apiUrl + 'priceAdjustment/',
    savePriceAdjustmentData: environment.apiUrl + 'priceAdjustment/',
    getExpenseAndAsset: environment.apiUrl + 'expenseAndAsset/{pipSheetId}',
    saveExpenseAndAsset: environment.apiUrl + 'expenseAndAsset/',
    getPartnerCostRevenue: environment.apiUrl + 'partnerCostAndRevenue/{pipSheetId}',
    savePartnerCostRevenue: environment.apiUrl + 'partnerCostAndRevenue/',
    getOtherPriceAdjustment: environment.apiUrl + 'otherPriceAdjustment/{pipSheetId}',
    saveOtherPriceAdjustment: environment.apiUrl + 'otherPriceAdjustment/',
    getReimbursementAndSales: environment.apiUrl + 'reimbursementAndSales/{pipSheetId}',
    saveReimbursementAndSales: environment.apiUrl + 'reimbursementAndSales/',
    getRiskManagementData: environment.apiUrl + 'riskManagement/',
    getFixedBidData: environment.apiUrl + 'fixBidAndMargin/',
    saveRiskManagementData: environment.apiUrl + 'riskManagement',
    getTotalClientPrice: environment.apiUrl + 'clientPrice/{pipSheetId}',
    saveTotalClientPrice: environment.apiUrl + 'clientPrice/',
    getCapitalCharge: environment.apiUrl + 'capitalCharge/{pipSheetId}',
    saveCapitalCharge: environment.apiUrl + 'capitalCharge/',
    getSummaryData: environment.apiUrl + 'summary/{pipSheetId}',
    getGrossProfit: environment.apiUrl + 'summary/{pipSheetId}/grossProfit',
    getInvestment: environment.apiUrl + 'summary/{pipSheetId}/investmentView',
    saveInvestment: environment.apiUrl + 'summary/investmentView',
    getEffortSummary: environment.apiUrl + 'summary/{pipSheetId}/effort',
    getBillingSchedule: environment.apiUrl + 'summary/{pipSheetId}/billingSchedule',
    getPLForecast: environment.apiUrl + 'summary/{pipSheetId}/plForecast',
    getYearComparison: environment.apiUrl + 'summary/{pipSheetId}/yoyComparison',
    getMasterList: environment.apiUrl + 'master/listNames/',
    submitPipSheet: environment.apiUrl + 'pipSheet',
    getCurrencyConversionData: environment.apiUrl + 'pipSheet/{pipSheetId}/currencyConversion',
    saveNewLocation: environment.apiUrl + 'adminMaster/location',
    getLocations: environment.apiUrl + 'adminMaster/locations',
    getPastLocationVersions: environment.apiUrl + 'adminMaster/location/{locationId}/pastVersions',
    deleteRejectedLocation: environment.apiUrl + 'adminMaster/location/{locationId}/version',
    getInActiveLocationVersions: environment.apiUrl + 'adminMaster/location/{locationId}/inactiveVersion',
    getAdmins: environment.apiUrl + 'account/{accountId}/admins',
    getUsers: environment.apiUrl + 'user/admin',
    deleteUserRole: environment.apiUrl + 'user/{userId}/account/{accountId}/fromAdminScreen/{fromAdminScreen}',
    assignAdminRole: environment.apiUrl + 'admin/role',
    getUserRoles: environment.apiUrl + 'account/{accountId}/usersAndRoles',
    saveUserRoles: environment.apiUrl + 'admin/role/nonAdmin',
    saveUserData: environment.apiUrl + 'user',
    getWorkflowStatusAccountRole: environment.apiUrl +
      'projectHeader/pipSheet/{pipSheetId}/account/{accountId}/project/{projectId}/workflowStatusAccountRole',
    getApproversData: environment.apiUrl + 'user/approver',
    updatePIPSheetCheckIn: environment.apiUrl + 'pipSheet/checkIn',
    saveSharedPipRole: environment.apiUrl + 'admin/role/sharedPip',
    getPIPCheckInCheckOutStatus: environment.apiUrl + 'pipSheet/{pipSheetId}/checkInCheckOut',
    getDashboardFile: environment.apiUrl + 'dashboard/file',
    deletePipSheet: environment.apiUrl + 'pipSheet/{pipSheetId}/project/{projectId}',
    getPipVersionSummaryDetails: environment.apiUrl + 'pipSheet/{pipSheetId}/versionSummary',
    getUserRoleForAllAccounts: environment.apiUrl + 'account/userRoles',
    getOverrideNotificationStatus: environment.apiUrl + 'pipSheet/{pipSheetId}/overrideNotificationStatus',
    getReadOnlyUsers: environment.apiUrl + 'user/readOnly',
    assignReadOnlyRole: environment.apiUrl + 'user/role/setReadOnly',
    getAllUsersAndAssociatedRoles: environment.apiUrl + 'admin/allUsersAndRoles',
    deleteReadOnlyRole: environment.apiUrl + 'user/{userId}/role/readOnly',
    getSharePipListData: environment.apiUrl + 'sharePipSheet/project/',
    updateSharePipListData: environment.apiUrl + 'sharePipSheet/',
    deleteSharePipListData: environment.apiUrl + 'sharePipSheet/{pipSheetId}/role/{roleId}/account/{accountId}/user/{sharedWithUserId}',
    getSharePipVersionData: environment.apiUrl + 'sharePipSheet/{projectId}/version',
    saveSharedPipData: environment.apiUrl + 'sharePipSheet/',
    getPipSheetStatus: environment.apiUrl + 'pipSheet/status',
    getAutoGeneratedProjectId: environment.apiUrl + 'account/{accountId}/accountCode/{accountCode}/autoGeneratedProjectId',
    createReplicatePipSheet: environment.apiUrl + 'pipSheet/replicate',
    getProjectListBasedOnAccountId: environment.apiUrl + 'account/{accountId}/projects/notSubmitted',
    getPIPSheetComments: environment.apiUrl + 'comments/{pipSheetId}',
    savePIPSheetComment: environment.apiUrl + 'comments',
    deletePIPSheetComment: environment.apiUrl + 'comments/{pipSheetCommentId}/project/{projectId}',
    getProjects: environment.apiUrl + 'account/{accountId}/projects',
    getCheckedOutVersions: environment.apiUrl + 'project/{projectId}/versions/checkedOut',
    saveCheckedInVersions: environment.apiUrl + 'admin/pipVersion',
    getAddMultipleUserTemplate: environment.apiUrl + 'account/GetAddMultipleUserTemplate',
    uploadMultipleUserData: environment.apiUrl + 'user/uploadMultipleUsers/',
    getPipOverrides: environment.apiUrl + 'pipSheet/{pipSheetId}/pipOverrides',
    savePlForeCastData: environment.apiUrl + 'summary/{pipSheetId}/plForecastData/',
    getProjectListForAccount: environment.apiUrl + 'report/account/projects/',
    getLocationWiseDetails: environment.apiUrl + 'summary/{pipSheetId}/locationWiseDetails/',
    getKeyPerformanceIndicators: environment.apiUrl + 'summary/{pipSheetId}/kpi',
    getReportKPIList: environment.apiUrl + 'report/customReportKPI',
    generateProjectSummaryReport: environment.apiUrl + 'report/project',
    getTotalDealFinancials: environment.apiUrl + 'summary/{pipSheetId}/totalDealFinancials/',
    getAuthorizedAccounts: environment.apiUrl + 'report/account/authorized',
    assignFinancePOCRole: environment.apiUrl + 'admin/role/financePOC'
  };

  static businessExceptions = {
    SessionExpired: 'SessionExpired',
    SessionKilled: 'SessionKilled',
    ErrorCode: 'ErrorCode',
    MessageCode: 'MessageCode'
  };

  static queryString = {
    SessionExpired: 'SessionExpired=true',
    SessionKilled: 'SessionKilled=true'
  };

  static localStorageKeys = {
    userName: 'userName',
    apiToken: 'apiToken',
    isLoggedIn: 'isLoggedIn',
    sessionId: 'sessionId',
    userId: 'userId',
    accessToken: 'accessToken',
    msalToken: 'msal.idtoken'
  };

  static imageExtension =
    {
      jpeg: '.jpeg',
      jpg: '.jpg'
    };

  static headerConstant =
    {
      xpLogo: 'xpTopRightLogo.png',
    };

  static splitChars = {
    comma: ','
  };

  static projectTabMenuItems = {
    staff: 'Staff',
    margin: 'Margin',
    summary: 'Summary',
    estimate: 'Estimate',
  };
  static userTabMenuItems = {
    addUser: 'Add New User',
    userList: 'Users List',
  };

  static HolidayOptionLabel = {
    select: '---Select---',
    on: 'ON : Holidays are time off',
    off: 'OFF : *no Holidays * work 5 days/wk'
  };

  static CostHoursEquivalent = 160;

  static calendar = {
    startYear: 2017,
    endYear: 2030,
    startMonth: 1,
    endMonth: 12,
    startDay: 1,
    endDay: 31
  };

  static selectComboItem: ComboItems = {
    label: '--- select ---',
    value: { id: -1, code: '--select---', name: 'select' }
  };

  static assetMaxDayCharge = 1200;

  static InvestMentView = {
    totalClientPrice: 'TOTALCLIENTPRICE',
    corporateTarget: 'CORPORATETARGET',
    netInvestment: 'NETINVESTMENT'
  };

  static effortSummary = {
    months: 'months',
    weeks: 'weeks',
    vacationAbsence: 'VACATIONABSENCE',
    totalQualifyingDiscounts: 'TOTALQUALIFYINGDISCOUNTS',
    projectduration: 'PROJECTDURATION',
    startDate: 'STARTDATE',
    endDate: 'ENDDATE'
  };

  static resourceContractorFlag = 'C*';
  static resourcePermanentFlag = '*';
  static milestoneGroupId = 4;
  static contractingEntityId = 25;
}
