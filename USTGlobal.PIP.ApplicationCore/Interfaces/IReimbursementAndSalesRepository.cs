using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IReimbursementAndSalesRepository
    {
        Task<ReimbursementAndSalesDTO> GetReimbursementAndSalesDetails(int pipSheetId);

        Task SaveReimbursementAndSalesDetails(string userName, IList<ReimbursementPeriodDTO> reimbursementPeriods, IList<ReimbursementDTO> reimbursements, IList<SalesDiscountPeriodDTO>
            salesDiscountPeriods, IList<SalesDiscountDTO> salesDiscounts);



    }
}
