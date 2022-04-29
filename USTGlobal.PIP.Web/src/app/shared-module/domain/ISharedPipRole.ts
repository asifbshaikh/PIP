import { IRole } from './IRole';

export interface ISharedPipRole {
    userId: number;
    uid: string;
    pipSheetId: number;
    isEditor: boolean;
    isReadOnly: boolean;
}
