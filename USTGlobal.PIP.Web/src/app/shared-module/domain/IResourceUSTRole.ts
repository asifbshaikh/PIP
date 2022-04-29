import { IBase } from './base';

export interface IResourceUSTRole extends IBase {
    resourceId: number;
    name: string;
    resourceGroupId: number;
    grade: string;
    oldName: string;
    masterVersionId: number;
    resourceServiceLineId: number;
}
