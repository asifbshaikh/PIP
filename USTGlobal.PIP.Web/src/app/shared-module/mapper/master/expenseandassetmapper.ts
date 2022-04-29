import { IPeriodLabourRevenue } from './../../domain/IPeriodLabourRevenue';
import { IExpenseAndAsset } from '@shared/domain/expenseandasset';
import { IAsset } from '@shared/domain/asset';
import { IDirectExpense } from '@shared/domain/directexpense';
import { ProjectPeriod } from '@shared/domain/projectperiod';
import { IProjectMilestone } from '@shared';

export class ExpensesAndAssetsMapper implements IExpenseAndAsset {
  assetDTO: IAsset[];
  directExpenseDTO: IDirectExpense[];
  projectPeriodDTO: ProjectPeriod[];
  projectMilestoneDTO: IProjectMilestone[];
  periodLaborRevenueDTO: IPeriodLabourRevenue[];

  mapper(input: any): this {
    Object.assign(this, input);
    return this;
  }
}
