import { Resource } from './resource';
import { ProjectPeriod } from './projectperiod';
import { ResourcePlanMasterData } from './resourceplanmasterdata';

export interface ResourcePlanning {
    resources: Resource[];
    projectPeriods: ProjectPeriod[];
    resourcePlanMasterData: ResourcePlanMasterData[];
}
