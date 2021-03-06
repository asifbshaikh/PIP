using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IHolidayRepository
    {
        Task<HolidayDTO> GetHolidayData(int holidayId);
        Task<List<HolidayDTO>> GetHolidays();
    }
}
