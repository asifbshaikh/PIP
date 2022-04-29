import { IReimbursement } from './IReimbursement';
import { ISalesDiscount } from './ISalesDiscount';
import { ProjectPeriod } from './projectperiod';
import { IProjectMilestone } from './IProjectMilestone';

export interface IReimbursementAndSales {
    reimbursements: IReimbursement[];
    salesDiscounts: ISalesDiscount[];
    projectPeriods: ProjectPeriod[];
    projectMilestones: IProjectMilestone[];
}
