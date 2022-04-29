export interface ISubmitPipSheet {
  pipSheetId: number;
  projectId: number;
  versionNumber: number;
  currencyId: number;
  pipSheetStatusId: number;
  comments: string;
  approverComments: string;
  resendComments: string;
  isCheckedOut: boolean;
  isSubmit: boolean;
  isApprove: boolean;
  isResend: boolean;
  isRevise: boolean;
  isEdit: boolean;
}
