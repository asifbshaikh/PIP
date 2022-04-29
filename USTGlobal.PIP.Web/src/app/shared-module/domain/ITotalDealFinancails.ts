import { ITotalDealFinancialsYearList } from './ITotalDealFinancialsYearList';

export interface ITotalDealFinancials {
    descriptionId: number;
    totalUSD: number;
    totalLocal: number;
    rowSectionId: number;
    totalFinLabel?: string;
    totalDealFinancialsYearList: ITotalDealFinancialsYearList[];
}
