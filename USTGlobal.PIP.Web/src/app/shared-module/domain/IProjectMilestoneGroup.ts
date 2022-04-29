import { IBase } from './base';

export interface IProjectMilestoneGroup extends IBase {
    milestoneGroupId: number;
    groupName: string;
    masterVersionId: number;
    createdOn: Date;
    updatedOn: Date;
}

