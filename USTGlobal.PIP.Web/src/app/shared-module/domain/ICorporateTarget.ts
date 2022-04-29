import { IBase } from './base';

export interface ICorporateTarget extends IBase {
    corporateTargetId: number;
    percent: number;
    description: string;
    masterVersionId: number;
    createdOn: Date;
    updatedOn: Date;
}
