import { CompileShallowModuleMetadata } from '@angular/compiler';
import { MasterColumn } from './mastercolumn';

export interface IMasterDetails {
    masterId: number;
    masterName: string;
    cols: MasterColumn[];
    data: any;
    isEdit: boolean;
    isView: boolean;
    isCreate: boolean;
    isSelected: boolean;
}
