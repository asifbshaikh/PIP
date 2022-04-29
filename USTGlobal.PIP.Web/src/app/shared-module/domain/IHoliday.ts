import { IBase } from './base';

export interface IHoliday extends IBase {
    id: number;
    holidayName: string;
    locationName: string;
    date: string;
    locationId: number;
    masterVersionId: number;
}
