using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IAreaRepository
    {
        Task<AreaDTO> GetArea(int areaId);
    }
}
