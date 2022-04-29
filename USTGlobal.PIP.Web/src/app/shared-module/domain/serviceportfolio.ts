import { IBase } from './base';

export interface IServicePortfolio extends IBase {
    servicePortfolioId: number;
    portfolioName: string;
    masterVersionId: number;
}
