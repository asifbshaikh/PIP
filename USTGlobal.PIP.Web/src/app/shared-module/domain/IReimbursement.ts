import { IBase } from '.';
import { IReimbursementPeriod } from './IReimbursementPeriod';

export interface IReimbursement extends IBase {

    uId: number;
    reimbursementId: number;
    pipSheetId: number;
    milestoneId: number;
    description: string;
    reimbursedExpense: number;
    isDirectExpenseReimbursable: boolean;
    directExpensePercent: number;
    isDirectExpenseMilestone: boolean;
    isDeleted: boolean;
    reimbursementPeriods: IReimbursementPeriod[];
}
