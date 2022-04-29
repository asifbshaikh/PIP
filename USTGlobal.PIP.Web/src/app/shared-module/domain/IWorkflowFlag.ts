
export interface IWorkflowFlag {
  isApproved: boolean;
  isApprovalPending: boolean;
  isNotSubmitted: boolean;
  isCheckedOut: boolean;
  checkedInOutBy: number;
  isApproverApproved: boolean;
  isApproversApprovalPending: boolean;
  isApproverNotSubmitted: boolean;
  checkNull: boolean;
}
