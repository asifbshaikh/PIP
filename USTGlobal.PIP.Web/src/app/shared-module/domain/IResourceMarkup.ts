import { IBase } from './base';

export interface IResourceMarkup extends IBase {
    markupId: number;
    name: string;
    percent: number;
    masterVersionId: number;
}
