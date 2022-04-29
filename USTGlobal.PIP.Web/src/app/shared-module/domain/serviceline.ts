import { IBase } from './base';

export interface IServiceLine extends IBase {
    serviceLineId: number;
    serviceLineName: string;
    servicePortfolioId: number;
    masterVersionId: number;
    portfolioName: string;
}
