import { IBase } from './base';

export interface IAsset extends IBase {
    projectAssetId: number;
    pipSheetId: number;
    basicAssetId?: number;
    description: string;
    costToProject: number;
    count: number;
    amount: number;
    isDeleted: boolean;
    createdOn?: Date;
    updatedOn?: Date;
    uId?: number;
}
