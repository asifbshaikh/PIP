import { PipCheckInComponent } from './administration-module/administration/pip-check-in/pip-check-in.component';
import { ModuleWithProviders, Component } from '@angular/core';
import {
  Routes,
  RouterModule
} from '@angular/router';
import { PageNotFoundComponent } from '@core';
import { AuthGuardService } from '@global';
import { Constants } from '@shared';
import { ProjectsComponent } from './planning-module/projects/projects.component';
import { PipVersionComponent } from './planning-module/projects/pip-versions/pip-version.component';
import { LoginComponent } from './login';
import { MasterComponent } from './planning-module/master/master.component';
import { DashboardComponent } from './dashboard-module/dashboard.component';
import { AdministrationComponent } from './administration-module/administration/administration.component';
import { MastersComponent } from './administration-module/masters/masters.component';
import { ProjectsStaffComponent } from './planning-module/projects/projects-staff/projects-staff.component';
import { ProjectsMarginComponent } from './planning-module/projects/projects-margin/projects-margin.component';
import { ProjectsSummaryComponent } from './planning-module/projects/projects-summary/projects-summary.component';
import { ReportsComponent } from './report-module/reports/reports.component';
import { ProjectsEstimateComponent } from './planning-module/projects/projects-estimate/projects-estimate.component';
import { DefineFinancePocComponent } from './administration-module/administration/define-finance-poc/define-finance-poc.component';
import { DefineAdminComponent } from './administration-module/administration/define-admin/define-admin.component';
import { UserRolesComponent } from './administration-module/userroles/userroles.component';
import { AddNewUserComponent } from './administration-module/administration/add-new-user/add-new-user.component';
import { ApproverComponent } from './approver-module/approver/approver.component';
import { DefineReadOnlyComponent } from './administration-module/administration/define-read-only/define-read-only.component';
import { AccountRolesComponent } from './administration-module/account-roles/account-roles.component';
import { SharePipComponent } from './planning-module/projects/share-pip/share-pip.component';
import { SharedPipListComponent } from './planning-module/projects/shared-pip-list/shared-pip-list.component';
import { UnauthorisedComponent } from '@shared/unauthorised/unauthorised.component';
const projectBreadcrumb = {
  label: Constants.breadcrumbLabels.projects,
  url: Constants.uiRoutes.projects
};

const adminBreadcrumb = {
  label: Constants.breadcrumbLabels.administration + '/Masters',
  url: Constants.uiRoutes.administration
};

const appRoutes: Routes = [
  {
    path: Constants.uiRoutes.empty,
    component: LoginComponent
  },
  {
    path: Constants.uiRoutes.login,
    component: LoginComponent
  },
  {
    path: Constants.uiRoutes.dashboard,
    component: DashboardComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: Constants.uiRoutes.projects,
    component: ProjectsComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: Constants.uiRoutes.sample,
    component: ProjectsComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: Constants.uiRoutes.projects
      + '/:' + Constants.uiRoutes.routeParams.projectId
      + '/:' + Constants.uiRoutes.routeParams.pipSheetId
      + '/:' + Constants.uiRoutes.routeParams.accountId
      + '/:' + Constants.uiRoutes.routeParams.dashboardId
      + '/' + Constants.uiRoutes.staff,
    component: ProjectsStaffComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.staff,
      breadcrumb: [projectBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.projects
      + '/:' + Constants.uiRoutes.routeParams.projectId
      + '/:' + Constants.uiRoutes.routeParams.pipSheetId
      + '/:' + Constants.uiRoutes.routeParams.accountId
      + '/:' + Constants.uiRoutes.routeParams.dashboardId
      + '/' + Constants.uiRoutes.margin,
    component: ProjectsMarginComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.margin,
      breadcrumb: [projectBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.projects
      + '/:' + Constants.uiRoutes.routeParams.projectId
      + '/:' + Constants.uiRoutes.routeParams.pipSheetId
      + '/:' + Constants.uiRoutes.routeParams.accountId
      + '/:' + Constants.uiRoutes.routeParams.dashboardId
      + '/' + Constants.uiRoutes.estimate,
    component: ProjectsEstimateComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.estimate,
      breadcrumb: [projectBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.projects
      + '/:' + Constants.uiRoutes.routeParams.projectId
      + '/:' + Constants.uiRoutes.routeParams.pipSheetId
      + '/:' + Constants.uiRoutes.routeParams.accountId
      + '/:' + Constants.uiRoutes.routeParams.dashboardId
      + '/' + Constants.uiRoutes.summary,
    component: ProjectsSummaryComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.summary,
      breadcrumb: [projectBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.projects
      + '/:' + Constants.uiRoutes.routeParams.projectId
      + '/:' + Constants.uiRoutes.versions,
    component: PipVersionComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.versions,
      breadcrumb: [projectBreadcrumb]
    },
  },

  //  sample paths
  {
    path: Constants.uiRoutes.sample
      + '/:' + Constants.uiRoutes.routeParams.projectId
      + '/:' + Constants.uiRoutes.routeParams.pipSheetId
      + '/:' + Constants.uiRoutes.routeParams.accountId
      + '/:' + Constants.uiRoutes.routeParams.dashboardId
      + '/' + Constants.uiRoutes.staff,
    component: ProjectsStaffComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.staff,
      breadcrumb: [projectBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.sample
      + '/:' + Constants.uiRoutes.routeParams.projectId
      + '/:' + Constants.uiRoutes.routeParams.pipSheetId
      + '/:' + Constants.uiRoutes.routeParams.accountId
      + '/:' + Constants.uiRoutes.routeParams.dashboardId
      + '/' + Constants.uiRoutes.margin,
    component: ProjectsMarginComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.margin,
      breadcrumb: [projectBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.sample
      + '/:' + Constants.uiRoutes.routeParams.projectId
      + '/:' + Constants.uiRoutes.routeParams.pipSheetId
      + '/:' + Constants.uiRoutes.routeParams.accountId
      + '/:' + Constants.uiRoutes.routeParams.dashboardId
      + '/' + Constants.uiRoutes.estimate,
    component: ProjectsEstimateComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.estimate,
      breadcrumb: [projectBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.sample
      + '/:' + Constants.uiRoutes.routeParams.projectId
      + '/:' + Constants.uiRoutes.routeParams.pipSheetId
      + '/:' + Constants.uiRoutes.routeParams.accountId
      + '/:' + Constants.uiRoutes.routeParams.dashboardId
      + '/' + Constants.uiRoutes.summary,
    component: ProjectsSummaryComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.summary,
      breadcrumb: [projectBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.sample
      + '/:' + Constants.uiRoutes.routeParams.projectId
      + '/:' + Constants.uiRoutes.versions,
    component: PipVersionComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.versions,
      breadcrumb: [projectBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.sample
      + '/:' + Constants.uiRoutes.routeParams.projectId
      + '/:' + Constants.uiRoutes.versions
      + '/:' + Constants.uiRoutes.sharePIP,
    component: SharePipComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.versions,
      breadcrumb: [projectBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.sample
      + '/:' + Constants.uiRoutes.routeParams.projectId
      + '/:' + Constants.uiRoutes.versions
      + '/:' + Constants.uiRoutes.sharePIPList,
    component: SharedPipListComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.versions,
      breadcrumb: [projectBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.projects
      + '/:' + Constants.uiRoutes.routeParams.projectId
      + '/:' + Constants.uiRoutes.versions
      + '/:' + Constants.uiRoutes.sharePIP,
    component: SharePipComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.versions,
      breadcrumb: [projectBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.projects
      + '/:' + Constants.uiRoutes.routeParams.projectId
      + '/:' + Constants.uiRoutes.versions
      + '/:' + Constants.uiRoutes.sharePIPList,
    component: SharedPipListComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.versions,
      breadcrumb: [projectBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.administration,
    component: AdministrationComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: Constants.uiRoutes.administration
      + '/' + Constants.uiRoutes.masters,
    component: MastersComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.masters,
      breadcrumb: [adminBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.administration
      + '/'
      + Constants.uiRoutes.defineFinancePoc,
    component: DefineFinancePocComponent,
    canActivate: [AuthGuardService],
  },
  {
    path: Constants.uiRoutes.administration
      + '/'
      + Constants.uiRoutes.defineAdmin,
    component: DefineAdminComponent,
    canActivate: [AuthGuardService],
  },
  {
    path: Constants.uiRoutes.administration
      + '/'
      + Constants.uiRoutes.defineReadOnly,
    component: DefineReadOnlyComponent,
    canActivate: [AuthGuardService],
  },
  {
    path: Constants.uiRoutes.administration
      + '/'
      + Constants.uiRoutes.userRolesAndPermission,
    component: UserRolesComponent,
    canActivate: [AuthGuardService],
    data: {
      title: Constants.breadcrumbLabels.masters,
      breadcrumb: [adminBreadcrumb]
    },
  },
  {
    path: Constants.uiRoutes.administration
      + '/'
      + Constants.uiRoutes.addNewUser,
    component: AddNewUserComponent,
    canActivate: [AuthGuardService],
  },
  {
    path: Constants.uiRoutes.administration
      + '/'
      + Constants.uiRoutes.listUsers,
    component: AccountRolesComponent,
    canActivate: [AuthGuardService],
  },
  {
    path: Constants.uiRoutes.administration
      + '/'
      + Constants.uiRoutes.pipCheckIn,
    component: PipCheckInComponent,
    canActivate: [AuthGuardService],
  },
  {
    path: Constants.uiRoutes.reports,
    component: ReportsComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: Constants.uiRoutes.approver,
    component: ApproverComponent,
    canActivate: [AuthGuardService],
  },
  {
    path: 'unauthorised',
    component: UnauthorisedComponent
  },
  {
    path: '**',
    component: PageNotFoundComponent,
    canActivate: [AuthGuardService]
  }
];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);
