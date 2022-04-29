using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ReimbursementAndSalesDTO
    {
        public IList<ReimbursementParentDTO> Reimbursements { get; set; }
        public IList<SalesDiscountParentDTO> SalesDiscounts { get; set; }
        public List<ProjectPeriodDTO> ProjectPeriods { get; set; }
        public List<ProjectMilestoneDTO> ProjectMilestones { get; set; }


    }
}
