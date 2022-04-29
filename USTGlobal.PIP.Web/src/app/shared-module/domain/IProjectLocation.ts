import { IBase } from './base';

export interface IProjectLocation extends IBase {
    locationId: number;
    pipSheetId: number;
    hoursPerDay: number;
    hoursPerMonth: number;
    isOverride: boolean;
    createdOn: Date;
    updatedOn: Date;
}
