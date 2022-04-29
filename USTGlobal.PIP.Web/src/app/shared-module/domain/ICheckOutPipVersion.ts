import { IProject } from '@shared';
export interface ICheckOutPipVersion {
  pipSheetId: number;
  versionNumber: number;
  checkedOutByName: string;
  checkedOutByUID: string;
}
