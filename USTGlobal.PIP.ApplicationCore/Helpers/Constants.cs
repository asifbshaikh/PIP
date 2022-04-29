namespace USTGlobal.PIP.ApplicationCore.Helpers
{
    public static class Constants
    {
        public static readonly string FlatFeeMonthly = "Flat Fee Monthly";
        public static readonly string MonthlyFixedHours = "Monthly fixed hours";
        public static readonly decimal costHoursEquivalent = 160;
        public static readonly string First = "First";
        public static readonly string Last = "Last";
        public static readonly int[] ContractorMarkUpIds = { 2, 3, 6, 7, 10, 11 };
        public static readonly string strStatusApprovalPending = "Approval Pending";
        public static readonly string StatusWaitingforApproval = "Waiting for Approval";
        public static readonly int OPAFeeBeforeAdjustment = 1;
        public static readonly int OPAFlagAdjustmentEntry = 2;
        public static readonly int OPAEditableFields = 3;
        public static readonly string VersionString = "Version ";
        public static readonly int StatusNotSubmitted = 1;
        public static readonly int StatusApprovalPending = 2;
        public static readonly int StatusApproved = 3;
        public static readonly string Projects = "projects";
        public static readonly string Administration = "administration";
        public static readonly string Approver = "approver";
        public static readonly string Staff = "Staff";
        public static readonly int ReadOnly = 3;
        public static readonly string Slash = "/";
        public static readonly string SuccessMessage = "Success";
        public static readonly bool IsOverride = false;
        public static readonly string Approved = "Approved";
        public static readonly string Shared = "Shared";

        // Report Constants
        public static readonly string ReportWorksheetName = "Report";
        public static readonly string ReportTemplateName = "Report";
        public static readonly string ReportOperation = "Report";
        public static readonly string ProjectSummaryViewReport = "Key KPIs Report - Project Level";
        public static readonly string ProjectResourceLevelReport = "All KPIs Report - Role & Project Level";
        public static readonly string ProjectDetailedLevelReport = "All KPIs Report - Project Level";
        public static readonly string CustomReport = "Key KPIs Report - Role Level";
        public static readonly string USDCurrency = "USD";
        public static readonly string PipCurrency = "Pip Currency";
        public static readonly string ReportTotalInvoicedKPI = "Total Invoiced";
        public static readonly string ReportNetCashFlowKPI = "Net CashFlow";
        public static readonly string ReportCumulativeCashFlowKPI = "Cumulative CashFlow";
    }
}
