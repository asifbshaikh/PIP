import { IBase } from './base';
import { IDirectExpensesPeriod } from './directexpensesperiod';

export interface IDirectExpense extends IBase {
    uId: number;
    directExpenseId: number;
    pipSheetId: number;
    milestoneId: number;
    label: string;
    description: string;
    totalExpense: number;
    isDeleted: boolean;
    percentRevenue: number;
    isReimbursable: boolean;
    directExpensePeriodDTO: IDirectExpensesPeriod[];
}
