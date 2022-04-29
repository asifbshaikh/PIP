export interface ICurrency {
    currencyId: number;
    countryId: number;
    symbol: string;
    factors: number;
    usdToLocal: number;
    localToUSD: number;
    masterVersionId: number;
}
