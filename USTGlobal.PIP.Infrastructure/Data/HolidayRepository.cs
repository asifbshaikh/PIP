using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class HolidayRepository : IHolidayRepository
    {
        public readonly PipContext pipContext;

        public HolidayRepository(PipContext context, IMasterRepository masterRepository)
        {
            this.pipContext = context;
        }

        public async Task<HolidayDTO> GetHolidayData(int holidayId)
        {
            return await this.pipContext.Holiday
                    .Where(holiday => holiday.HolidayId == holidayId)
                    .Select(holiday => new HolidayDTO
                    {
                        Id = holiday.HolidayId,
                        HolidayName = holiday.HolidayName,
                        Date = holiday.Date
                    }).SingleOrDefaultAsync();
        }

        public async Task<List<HolidayDTO>> GetHolidays()
        {
            return await (from h in this.pipContext.Holiday
            join l in this.pipContext.Location on h.LocationId equals l.LocationId
            select new HolidayDTO()
            {
                Id = h.HolidayId,
                HolidayName = h.HolidayName,
                Date = h.Date,
                LocationId = h.LocationId,
                LocationName = l.LocationName,
                MasterVersionId = h.MasterVersionId
            }).ToListAsync();
        }
    }
}
