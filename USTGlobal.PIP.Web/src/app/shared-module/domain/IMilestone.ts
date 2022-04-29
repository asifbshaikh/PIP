import { IBase } from './base';

export interface IMilestone extends IBase {
    milestoneId: number;
    milestoneName: string;
    milestoneGroupId: number;
    masterVersionId: number;
    createdOn: Date;
    updatedOn: Date;
}
