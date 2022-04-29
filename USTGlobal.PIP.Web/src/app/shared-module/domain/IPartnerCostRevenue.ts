import { IPartnerCost, IPartnerRevenue } from '@shared';
import { ProjectPeriod } from './projectperiod';
import { IProjectMilestone } from './IProjectMilestone';
export interface IPartnerCostRevenue {
    partnerCost: IPartnerCost[];
    partnerRevenue: IPartnerRevenue[];
    projectPeriod: ProjectPeriod[];
    projectMilestone: IProjectMilestone[];
}
