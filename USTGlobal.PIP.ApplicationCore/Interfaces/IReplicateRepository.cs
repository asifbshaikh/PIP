using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IReplicateRepository
    {
        Task<RouteParamDTO> ReplicatePIPSheet(string userName, ReplicatePIPSheetDTO replicate);
    }
}
