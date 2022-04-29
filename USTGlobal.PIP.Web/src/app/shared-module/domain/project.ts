export interface IProject {
  projectId: number;
  sfProjectId: string;
  projectName: string;
  pipSheetStatus: string;
  accountId: number;
  accountName: string;
  serviceLine: string;
  deliveryType: string;
  billingType: string;
  pipSheetId?: number;
  isDummy: boolean;
  currencyId?: number;
}
