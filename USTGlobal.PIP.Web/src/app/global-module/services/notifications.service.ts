import { version } from 'process';
import { ICheckRole } from './../../shared-module/domain/ICheckRole';
import { IWorkflowFlag } from '@shared/domain/IWorkflowFlag';
import { Injectable } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { LoggerService } from '@core';
import { DialogService } from 'primeng/api';
import { OverrideNotificationDialogComponent } from '@shared/override-notification-dialog/override-notification-dialog.component';

@Injectable()
export class NotificationService {

  onProjectsListClickNotification: Subject<number> = new Subject<number>();
  productAddedToCartNotification: Subject<null> = new Subject<null>();
  disableUINotification: Subject<null> = new Subject<null>();
  durationExistsNotification: Subject<string> = new Subject<string>();
  projectNameExistsNotification: Subject<string> = new Subject<string>();
  sfProjectIdExistsNotification: Subject<string> = new Subject<string>();
  currencyDataChangeNotification: Subject<string> = new Subject<string>();
  totalClientPriceExistsNotification: Subject<string> = new Subject<string>();
  percentEbitdaExistsNotification: Subject<string> = new Subject<string>();
  totalVersionExistsNotification: Subject<number> = new Subject<number>();
  workflowFlagExistsNotification: Subject<IWorkflowFlag> = new Subject<IWorkflowFlag>();
  roleExistsNotification: Subject<ICheckRole> = new Subject<ICheckRole>();


  // to multicase
  isFormDirty: Subject<boolean> = new Subject<boolean>();
  displayDialog: Subject<boolean> = new Subject<boolean>();
  displayNotificationDialog: Subject<boolean> = new Subject<boolean>();

  submitClickNotification: Subject<boolean> = new Subject<boolean>();
  resendClickNotification: Subject<boolean> = new Subject<boolean>();
  hasAccountLevelAccessNotification: Subject<boolean> = new Subject<boolean>();

  projectApprovalPending: Subject<boolean> = new Subject<boolean>();
  projectApproved: Subject<boolean> = new Subject<boolean>();
  projectNotSubmitted: Subject<boolean> = new Subject<boolean>();
  isAnyVersionApproved: Subject<boolean> = new Subject<boolean>();

  constructor(
    private logger: LoggerService,
    private dialogService: DialogService,
  ) {
    this.logger.info('NotificationService : constructor');
  }

  notifyOnProjectsListClick(currencyId: number) {
    this.logger.info('NotificationService : notifyOnProjectsListClick');
    this.onProjectsListClickNotification.next(currencyId);
  }

  notifyDurationExists(projectDuration: string) {
    this.logger.info('NotificationService : notifyDurationExists');
    this.durationExistsNotification.next(projectDuration);
  }

  notifyProjectNameExists(accountName: string) {
    this.logger.info('NotificationService : notifyProjectNameExists');
    this.projectNameExistsNotification.next(accountName);
  }

  notifyCurrencyChange(currencyData: string) {
    this.logger.info('NotificationService : notifyCurrencyChange');
    this.currencyDataChangeNotification.next(currencyData);
  }

  notifySFProjectIdExists(projectId: string) {
    this.logger.info('NotificationService : notifySFProjectIdExists');
    this.sfProjectIdExistsNotification.next(projectId);
  }

  notifyTotalClientPriceExists(totalClientPrice: string) {
    this.logger.info('NotificationService : notifyDurationExists');
    this.totalClientPriceExistsNotification.next(totalClientPrice);
  }

  notifyPercentEbitdaExists(percentEbitda: string) {
    this.logger.info('NotificationService : notifyPercentEbitdaChange');
    this.percentEbitdaExistsNotification.next(percentEbitda);
  }
  notifyTotalVersionExists(totalVersion: number) {
    this.logger.info('NotificationService : notifyTotalVersionChange');
    this.totalVersionExistsNotification.next(totalVersion);
  }
  notifyWorkflowFlagExists(workflowFlag: IWorkflowFlag) {
    this.logger.info('NotificationService : notifyWorkflowFlagChange');
    this.workflowFlagExistsNotification.next(workflowFlag);
  }
  notifyRoleExists(checkRole: ICheckRole) {
    this.logger.info('NotificationService : notifyRoleChange');
    this.roleExistsNotification.next(checkRole);
  }

  notifyVersionApproved(versionApproved: boolean) {
    this.logger.info('NotificationService : notifyVersionApproved');
    this.isAnyVersionApproved.next(versionApproved);
  }

  notifyFormDirty(isDirty: boolean) {
    this.isFormDirty.next(isDirty);
  }

  showDialog() {
    return this.displayDialog.next(true);
  }

  // displayDialogBox() {
  //   return this.displayDialog.asObservable();
  // }

  showNotificationDialog(pipSheetId) {
    this.dialogService.open(OverrideNotificationDialogComponent, {
      data: {
        pipSheetId: pipSheetId
      },
      header: 'Notification',
      height: '15%',
      width: '30%',
    });
  }
  notifySubmitClick(submitClick: boolean) {
    this.logger.info('NotificationService : notifySubmitClick');
    this.submitClickNotification.next(submitClick);
  }
  notifyResendClick(resendClick: boolean) {
    this.logger.info('NotificationService : notifyResendClick');
    this.resendClickNotification.next(resendClick);
  }
  notifyHasAccountLevelAccess(hasAccountLevelAccess: boolean) {
    this.logger.info('NotificationService : notifyHasAccountLevelAccess');
    this.hasAccountLevelAccessNotification.next(hasAccountLevelAccess);
  }
  //

  notifyProjectApproved(isProjectApproved: boolean) {
    this.projectApproved.next(isProjectApproved);
  }

  notifyProjectApprovalPending(isProjectApprovalPending: boolean) {
    this.projectApprovalPending.next(isProjectApprovalPending);
  }

  notifyProjectNotSubmitted(isProjectNotSubmitted: boolean) {
    this.projectNotSubmitted.next(isProjectNotSubmitted);
  }
}
