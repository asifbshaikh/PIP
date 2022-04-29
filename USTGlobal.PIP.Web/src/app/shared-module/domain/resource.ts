import { ResourcePeriod } from './resourceperiod';
import { State } from './state.enum';
import { IBase } from './base';

export interface Resource extends IBase {
    uId: number;
    id: number;
    pipSheetId: number;
    projectResourceId: number;
    alias: string;
    locationId: number;
    resourceGroupId: number;
    resourceId: number;
    technologyId: number;
    utilizationType: boolean;
    milestoneId: number;
    contractorFlag: string;
    markupId: number;
    totalhoursPerResource: number;
    costHrsPerResource: number;
    grade: string;
    projectPeriod: ResourcePeriod[];
    isDeleted: boolean;
    totalFTE: number;
    resourceServiceLineId: number;
    clientRole: string;
    // state: State;
}





