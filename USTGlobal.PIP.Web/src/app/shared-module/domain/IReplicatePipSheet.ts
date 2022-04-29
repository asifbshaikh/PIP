export interface IReplicatePipSheet {
  sourcePIPSheetId: number;
  sourceProjectId: number;
  sfProjectId: string;
  accountId: number;
  projectNamePerSf: string;
  paymentLag: number;
  isDummy: boolean;
  replicateType: number;
  versionNumber: number;
}

