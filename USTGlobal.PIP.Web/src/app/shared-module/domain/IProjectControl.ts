import { IProjectLocation } from './IProjectLocation';
import { IProjectMilestone } from './IProjectMilestone';
import { IPIPSheet } from './IPIPSheet';


export interface IProjectControl {
    pipSheetListDTO: IPIPSheet[];
    projectLocationListDTO: IProjectLocation[];
    projectMilestoneListDTO: IProjectMilestone[];
}
