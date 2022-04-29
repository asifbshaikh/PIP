import { TotalClientPrice } from './TotalClientPrice';
import { IBase } from './base';

export interface IHeader1 extends IBase {
    sfAccountName: string;
    startDate: string;
    endDate: string;
    projectName: string;
    versionNumber: number;
    sfProjectId: string;
    currency: string;
    totalClientPrice: number;
    totalVersionsPresent: number;
}
