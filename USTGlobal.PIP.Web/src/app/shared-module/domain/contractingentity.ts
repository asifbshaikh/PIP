import { IBase } from './base';

export interface IContractingEntity extends IBase {
    contractingEntityId: number;
    code: string;
    name: string;
    masterVersionId: number;

}
