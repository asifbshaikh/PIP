import { ISelectedAccount } from './ISelectedAccount';
import { ISelectedProject } from './ISelectedProject';
import { ISelectedKPI } from './ISelectedKPI';

export interface IGenerateReport {
    startDate: string;
    endDate: string;
    isUSDCurrency: boolean;
    reportType: number;
    selectedKPIs: ISelectedKPI[];
    selectedAccounts: ISelectedAccount[];
    selectedProjects: ISelectedProject[];



}
