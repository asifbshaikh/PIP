import { ICurrency } from './ICurrency';
import { IProjectHeader } from './projectheader';
import { IBase } from './base';

export interface IProjectCurrencyHeader extends IBase {
  projectHeader: IProjectHeader;
  currency: ICurrency;
  totalVersionsPresent: number;
}
