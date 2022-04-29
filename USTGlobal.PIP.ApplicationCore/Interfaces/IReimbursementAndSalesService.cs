using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IReimbursementAndSalesService
    {
        Task<ReimbursementAndSalesDTO> GetReimbursementAndSalesDetails(int pipSheetId);
        Task SaveReimbursementAndSalesDetails(string userName, ReimbursementAndSalesDTO reimbursementAndSalesDTO);
        ReimbursementAndSalesDTO ReAssignUIds(ReimbursementAndSalesDTO reimbursementAndSales);
    }
}
