import { IBase } from './base';

export interface IProjectMilestone extends IBase {
    uId: number;
    projectMilestoneId: number;
    pipSheetId: number;
    milestoneId: number;
    milestoneName: string;
    milestoneGroupId: number;
    isChecked: boolean;
    createdOn: Date;
    updatedOn: Date;
    originalValue: string;
    overrideValue: string;
    invoiceAmount: number;
    milestoneMonth: string;
}
