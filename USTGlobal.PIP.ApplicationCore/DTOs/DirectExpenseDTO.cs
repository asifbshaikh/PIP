namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class DirectExpenseDTO : BaseDTO
    {
        public int UId { get; set; }
        public int DirectExpenseId { get; set; }
        public int PipSheetId { get; set; }
        public int? MilestoneId { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal? PercentRevenue { get; set; }
        public bool IsReimbursable { get; set; }
        public bool IsDeleted { get; set; }
    }
}
