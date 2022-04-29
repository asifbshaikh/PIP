using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IEbitdaWorkflowService
    {
        Task ProcessEbitdaSaving(string userName, List<EbitdaDTO> ebitdadata);
    }
}
