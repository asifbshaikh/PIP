import { IAsset } from './asset';
import { IDirectExpense } from './directexpense';
import { ProjectPeriod } from './projectperiod';
import { IProjectMilestone } from './IProjectMilestone';

export interface IExpenseAndAsset {
    assetDTO: IAsset[];
    directExpenseDTO: IDirectExpense[];
    projectPeriodDTO: ProjectPeriod[];
    projectMilestoneDTO: IProjectMilestone[];
}
