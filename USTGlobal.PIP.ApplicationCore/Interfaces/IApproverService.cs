using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IApproverService
    {
        Task<List<ApproverDTO>> GetApproversData(string userName);
    }
}
