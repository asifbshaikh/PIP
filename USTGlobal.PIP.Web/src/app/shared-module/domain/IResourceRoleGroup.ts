import { IBase } from './base';

export interface IResourceRoleGroup extends IBase {
    resourceGroupId: number;
    groupName: string;
    locationId: number;
    masterVersionId: number;
}
