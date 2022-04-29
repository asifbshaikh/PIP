using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IAreaService
    {
        Task<AreaDTO> GetArea(int areaId);
    }
}
