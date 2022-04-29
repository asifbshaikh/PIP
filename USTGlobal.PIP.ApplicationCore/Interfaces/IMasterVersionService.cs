using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IMasterVersionService
    {
        Task<MasterVersionDTO> GetMasterVersionData(int masterVersionId);
    }
}
