using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ReimbursementDTO : BaseDTO
    {
        public int UId { get; set; }
        public int ReimbursementId { get; set; }
        public int PipSheetId { get; set; }
        public int? MilestoneId { get; set; }
        public string Description { get; set; }
        public decimal? ReimbursedExpense { get; set; }
        public bool IsDirectExpenseReimbursable { get; set; }
        public decimal? DirectExpensePercent { get; set; }
        public bool IsDirectExpenseMilestone { get; set; }
        public bool isDeleted { get; set; }
    }
}
