import { LocationHoliday } from './locationholiday';

export interface MonthlyData {
  workingDays: number;
  totalDays: number;
  actualWorkingDays: number;
  locationHolidays: LocationHoliday[];
}
