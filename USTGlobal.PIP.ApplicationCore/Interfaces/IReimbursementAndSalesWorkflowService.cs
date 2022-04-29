using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IReimbursementAndSalesWorkflowService
    {
        Task ProcessReimbursementAndSalesSaving(string userName, ReimbursementAndSalesDTO reimbursementAndSalesDTO);
    }
}
