export enum BusinessRulesErrorCodes {
  PasswordExpired = <any>'BR104'
}

export enum SortOrder {
  Asc = 1,
  Desc = 2
}

export enum RegExType {
  negativedecimalPrecisionFour = <any>'negativedecimalPrecisionFour',
  decimalPrecisionFour = <any>'decimalPrecisionFour'
}

export enum ResourceBillingType {
  FlatFeeMonthly = 'Flat Fee Monthly',
  MonthlyfixedHrs = 'Monthly fixed hrs'
}

export enum StaffSteps {
  ProjectHeader,
  ProjectControl,
  ResourcePlanning,
  PartnerCosts,
  Assumptions,
  Summary
}

export enum MarginSteps {
  LaborPricing,
  VacationAbsences,
  PriceAdjustmentYOY,
  PartnerCost,
  Reimbursements,
  SalesDiscounts,
  FixedBidMargin,
  OtherPricingAdj,
  TotalClientPrice,
  DirectExpenses,
  AssetPlanning,
  RisksManagement,
  CapitalCharges,
  Summary
}

export enum AllowedEbidaSeatCostLocationsOverride {
  India_general = 11,
  India_Bangalore = 12,
  India_Bhopal = 13,
  India_Chennai = 14,
  India_Gurgaon = 16,
  India_Trivandrum_Cochin = 17,
  Manila_Phillippines = 22,
  Mexico = 23,
  Malaysia = 21
}

export enum CellType {
  nil,
  text,
  dropdown,
  datepicker
}

export enum FixedBidMarginLabels {
  autoCalculated = 1,
  margin = 2,
  riskMarginRevenues = 3,
  currentPrice = 4
}

export enum MarginType {
  ebitda = 1,
  netRevenue = 2,
  price = 3
}

