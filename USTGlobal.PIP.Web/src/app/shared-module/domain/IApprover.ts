export interface IApprover {
  projectId: number;
  sfProjectId: string;
  projectName: string;
  pipSheetId: number;
  versionNumber: number;
  accountId: number;
  accountName: string;
  submittedBy: string;
  submittedOn: Date;
  comments: string;
  approverComments: string;
  pipSheetStatus: string;
  approvedOn: Date;
  approvedBy: string;
  resendComments: string;
  resendOn: Date;
  resendBy: string;
  resendOnString: string;
  approvedOnString: string;
}
