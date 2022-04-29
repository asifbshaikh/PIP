
export interface IPipSheetWorkflowStatus {
  pipSheetStatusId: number;
  pipSheetStatusName: string;
  approverStatusId: number;
  approverStatusName: string;
  isCheckedOut: boolean;
  checkedInOutBy: number;
}
