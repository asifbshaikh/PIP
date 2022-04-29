import { ILocationWisePeriodDetails } from './ILocationWisePeriodDetails';

export interface ILocationWiseDetails {
    descriptionId: number;
    total: number;
    locationWiseDetailsLabel?: string;
    summaryLocationDTO: ILocationWisePeriodDetails[];
}
