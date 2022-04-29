using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IMasterVersionRepository
    {
        Task<MasterVersionDTO> GetMasterVersionData(int masterVersionId);
    }
}
