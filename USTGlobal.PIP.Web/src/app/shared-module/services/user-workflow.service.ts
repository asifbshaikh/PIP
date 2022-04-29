import { IRoleAndAccount } from '@shared/domain/IRoleAndAccount';
import { IPipSheetWorkflowStatus } from './../domain/IPipSheetWorkflowStatus';
import { ICheckRole } from './../domain/ICheckRole';
import { Injectable } from '@angular/core';
import { IWorkflowFlag } from '@shared/domain/IWorkflowFlag';
import { OverrideNotificationStatus } from '@shared/domain/override-notification-status';
import { HttpClient } from '@angular/common/http';
import { Constants } from '@shared/infrastructure';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class UserWorkflowService {
  constructor(private httpClient: HttpClient) { }

  isFormDisabled(checkRole: ICheckRole, workflowFlag: IWorkflowFlag, loggedInUserId: number, dashboardId: number): boolean {
    // If pipsheet status is 1 - Not Submitted and approver status is null and request will be visible only on project list grid
    if (workflowFlag.isNotSubmitted && workflowFlag.checkNull) {
      // If pipsheet is CheckedOut and by same user as logged in user
      if (!workflowFlag.isCheckedOut && loggedInUserId === workflowFlag.checkedInOutBy || workflowFlag.isCheckedOut) {
        return this.getFormStatus(checkRole);
      }
      // If pipsheet is CheckedOut and not by same user as logged in user
      else if (!workflowFlag.isCheckedOut && loggedInUserId !== workflowFlag.checkedInOutBy) {
        return true;
      }
    }
    // If pipsheet status is 3 - approval pending and approver status is 3 - approval pending or
    // If pipsheet status is 2 - approved  and approver status is 2 - approved
    else if ((workflowFlag.isApprovalPending && workflowFlag.isApproversApprovalPending)
      || (workflowFlag.isApproved && workflowFlag.isApproverApproved)) {
      return true;
    }
    // If pipsheet status is 1 - Not Submitted  and approver status is 1 - Not Submitted and route id = 1(Project list dashboard)
    // Request will be editable by editor
    else if (workflowFlag.isNotSubmitted && workflowFlag.isApproverNotSubmitted && dashboardId === 1) {
      // If pipsheet is CheckedOut and by same user as logged in user
      if (!workflowFlag.isCheckedOut && loggedInUserId === workflowFlag.checkedInOutBy || workflowFlag.isCheckedOut) {
        return this.getFormStatus(checkRole);
      }
      // If pipsheet is CheckedOut and not by same user as logged in user
      else if (!workflowFlag.isCheckedOut && loggedInUserId !== workflowFlag.checkedInOutBy) {
        return true;
      }
    }
    // If pipsheet status is 1 - Not Submitted  and approver status is 1 - Not Submitted and route id = 2(approver list dashboard)
    // Request will be non editable for every role
    else if (workflowFlag.isNotSubmitted && workflowFlag.isApproverNotSubmitted && dashboardId === 2) {
      return true;
    }
  }
  getWorkflowFlag(pipSheetWorkflowStatus: IPipSheetWorkflowStatus, wfStatus: any): IWorkflowFlag {
    const workflowFlag: IWorkflowFlag = {
      isApproved: false, isApprovalPending: false, isNotSubmitted: false,
      isCheckedOut: false, checkedInOutBy: 0, isApproverApproved: false,
      isApproverNotSubmitted: false, isApproversApprovalPending: false, checkNull: false
    };

    if (pipSheetWorkflowStatus.pipSheetStatusName === wfStatus.ApprovalPending) { workflowFlag.isApprovalPending = true; }
    else if (pipSheetWorkflowStatus.pipSheetStatusName === wfStatus.Approved) { workflowFlag.isApproved = true; }
    else if (pipSheetWorkflowStatus.pipSheetStatusName === wfStatus.NotSubmitted) { workflowFlag.isNotSubmitted = true; }
    workflowFlag.isCheckedOut = pipSheetWorkflowStatus.isCheckedOut;
    workflowFlag.checkedInOutBy = pipSheetWorkflowStatus.checkedInOutBy;
    if (pipSheetWorkflowStatus.approverStatusName === wfStatus.ApprovalPending) { workflowFlag.isApproversApprovalPending = true; }
    else if (pipSheetWorkflowStatus.approverStatusName === wfStatus.Approved) { workflowFlag.isApproverApproved = true; }
    else if (pipSheetWorkflowStatus.approverStatusName === wfStatus.NotSubmitted) { workflowFlag.isApproverNotSubmitted = true; }
    else if (pipSheetWorkflowStatus.approverStatusName === null) { workflowFlag.checkNull = true; }

    return workflowFlag;
  }
  getUserSpecificRoles(roleAndAccount: IRoleAndAccount[]): ICheckRole {
    const checkRole: ICheckRole = {
      isAdmin: false, isEditor: false, isFinanaceApprover: false,
      isReviewer: false, isReadOnly: false
    };

    roleAndAccount.forEach(roleAcc => {

      if (roleAcc.roleName === 'Admin') {
        checkRole.isAdmin = true;
      }
      else if (roleAcc.roleName === 'Editor') {
        checkRole.isEditor = true;
      }
      else if (roleAcc.roleName === 'Finance Approver') {
        checkRole.isFinanaceApprover = true;
      }
      else if (roleAcc.roleName === 'Reviewer') {
        checkRole.isReviewer = true;
      }
      else if (roleAcc.roleName === 'Readonly') {
        checkRole.isReadOnly = true;
      }
    });
    return checkRole;
  }
  // Get Form Status when:-
  // Pipsheet status - Not Submitted and Approver Status - Null
  // Pipsheet status - Not Submitted and Approver Status - Not Submitted
  getFormStatus(checkRole: ICheckRole): boolean {
    // Check if editor
    if (checkRole.isEditor) {
      if (checkRole.isAdmin) {
        if (checkRole.isFinanaceApprover) {
          if (checkRole.isReadOnly) {
            if (checkRole.isReviewer) {
              return false;
            }
            return false;
          }
          else if (checkRole.isReviewer) {
            return false;
          }
          return false;
        }
        else if (checkRole.isReviewer) {
          if (checkRole.isReadOnly) {
            return false;
          }
          return false;
        }
        else if (checkRole.isReadOnly) {
          return false;
        }
        return false;
      }
      else if (checkRole.isReadOnly) {
        if (checkRole.isFinanaceApprover) {
          if (checkRole.isReviewer) {
            return false;
          }
          return false;
        }
        else if (checkRole.isReviewer) {
          return false;
        }
        return false;
      }
      else if (checkRole.isReviewer) {
        if (checkRole.isFinanaceApprover) {
          return false;
        }
        return false;
      }
      else if (checkRole.isFinanaceApprover) {
        return false;
      }
      return false;
    }
    // Check if read only
    else if (checkRole.isReadOnly) {
      if (checkRole.isAdmin) {
        if (checkRole.isReviewer && checkRole.isFinanaceApprover) {
          return true;
        }
        else if (checkRole.isFinanaceApprover) {
          return true;
        }
        else if (checkRole.isReviewer) {
          return true;
        }
        return true;
      }
      else if (checkRole.isFinanaceApprover) {
        if (checkRole.isReviewer) {
          return true;
        }
        return true;
      }
      else if (checkRole.isReviewer) {
        return true;
      }
      return true;
    }
    // Check if finance poc
    else if (checkRole.isFinanaceApprover) {
      if (checkRole.isAdmin) {
        if (checkRole.isReviewer) {
          return true;
        }
        return true;
      }
      else if (checkRole.isReviewer) {
        return true;
      }
      return true;
    }
    // Check if reviewer
    else if (checkRole.isReviewer) {
      if (checkRole.isAdmin) {
        return true;
      }
      return true;
    }
    // Check if admin
    else if (checkRole.isAdmin) {
      return true;
    }
  }

  // Get tooltip message for editor role, if pipsheet is checkout by user other than logged in user
  getToolTipMessageForEditor(checkedInOutByName: string, checkedInOutBy: number, loggedInUserId: number): string {
    let toolTipMessage = '';
    if (checkedInOutBy !== loggedInUserId) {
      toolTipMessage = 'PIP version is checked out by ' + checkedInOutByName + '. Your access limited to Read Only.';
    }
    return toolTipMessage;
  }

  getToolTipMessageForReadonly(checkedInOutByName: string, checkedInOutBy: number, loggedInUserId: number): string {
    let toolTipMessage = '';
    if (checkedInOutBy !== loggedInUserId) {
      toolTipMessage = 'This PIP version is currently checked out by ' + checkedInOutByName + ' for editing.' +
        ' The values are likely to change.';
    }
    return toolTipMessage;
  }

  returnRoleCheckForEditor(roleName: string[]): boolean {
    const value = roleName.indexOf('Editor') >= 0 ? true : false;
    return value;
  }

  returnRoleCheckForCheckinCheckOutIcon(roleName: string[]): boolean {
    const value = roleName.indexOf('Editor') >= 0 || roleName.indexOf('Readonly') >= 0 ? true : false;
    return value;
  }
  getOverrideNotificationStatus(pipSheetId: any): Observable<any> {
    return this.httpClient.get(Constants.webApis.getOverrideNotificationStatus.replace('{pipSheetId}', pipSheetId));
  }
}
