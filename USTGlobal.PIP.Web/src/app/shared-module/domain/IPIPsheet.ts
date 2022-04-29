import { IBase } from './base';

export interface IPIPSheet extends IBase {
    projectId: number;
    versionNumber: number;
    pipSheetId: number;
    startDate: string;
    endDate: string;
    holidayOption: boolean;
    milestoneGroupId: number;
    currencyId: number;
    createdOn: Date;
    updatedOn: Date;
    pipSheetStatusId: number;
    comments: string;
}
