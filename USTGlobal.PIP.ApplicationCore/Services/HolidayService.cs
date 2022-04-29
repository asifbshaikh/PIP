using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository holidayRepository;

        public HolidayService(IHolidayRepository holidayRepository)
        {
            this.holidayRepository = holidayRepository;
        }

        public async Task<HolidayDTO> GetHolidayData(int holidayId)
        {
            return await this.holidayRepository.GetHolidayData(holidayId);
        }

        public async Task<List<HolidayDTO>> GetHolidays()
        {
            return await this.holidayRepository.GetHolidays();
        }
    }
}
