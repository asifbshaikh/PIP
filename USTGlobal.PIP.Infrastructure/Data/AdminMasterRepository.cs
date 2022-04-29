using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Entities;
using USTGlobal.PIP.ApplicationCore.Interfaces;
using System.Linq;
using System.Data.SqlClient;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class AdminMasterRepository : IAdminMasterRepository
    {
        private readonly PipContext pipContext;
        public AdminMasterRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<List<LocationDTO>> GetLocations()
        {
            List<LocationDTO> locationDTOs = new List<LocationDTO>();
            await pipContext.LoadStoredProc("dbo.sp_GetAdminMasterLocations")
                .ExecuteStoredProcAsync((resultSet) =>
                {
                    locationDTOs = resultSet.ReadToList<LocationDTO>().ToList();
                });

            return locationDTOs;
        }

        public async Task<List<LocationDTO>> GetPastLocationVersions(int locationId)
        {
            return await this.pipContext.Location
                .Where(x => x.LocationId == locationId && x.IsActive == false && x.EndDate != null && x.Status == 2)
                .OrderByDescending(x => x.StartDate)
                .ProjectToType<LocationDTO>()
                .ToListAsync();
        }

        public async Task<int> SaveLocation(LocationDTO locationDTO)
        {
            SqlParameter[] inputParams = new SqlParameter[9];
            inputParams[0] = new SqlParameter("@LocationId", locationDTO.LocationId);
            inputParams[0].Direction = System.Data.ParameterDirection.InputOutput;
            inputParams[1] = new SqlParameter("@LocationName", locationDTO.LocationName);
            inputParams[2] = new SqlParameter("@HoursPerDay", locationDTO.HoursPerDay.HasValue ? (object)locationDTO.HoursPerDay : DBNull.Value);
            inputParams[3] = new SqlParameter("@HoursPerMonth", locationDTO.HoursPerMonth.HasValue ? (object)locationDTO.HoursPerMonth : DBNull.Value);
            inputParams[4] = new SqlParameter("@IsActive", locationDTO.IsActive);
            inputParams[5] = new SqlParameter("@StartDate", locationDTO.StartDate.HasValue ? (object)locationDTO.StartDate : DBNull.Value);
            inputParams[6] = new SqlParameter("@EndDate", locationDTO.EndDate.HasValue ? (object)locationDTO.EndDate : DBNull.Value);
            inputParams[7] = new SqlParameter("@Comments", locationDTO.Comments != null ? locationDTO.Comments : string.Empty);
            inputParams[8] = new SqlParameter("@Status", locationDTO.Status);
            var sqlQuery = "exec dbo.sp_SaveAdminMasterLocation @LocationId output, @LocationName, @HoursPerDay, @HoursPerMonth, @IsActive, @StartDate, @EndDate, @Comments, @Status";
            await pipContext.Database.ExecuteSqlCommandAsync(sqlQuery, inputParams);

            return Convert.ToInt32(inputParams[0].Value);
        }

        public async Task<LocationDTO> GetInactiveLocationVersion(int locationId)
        {
            return await this.pipContext.Location
                .Where(x => x.LocationId == locationId && x.IsActive == false && x.StartDate == null)
                .ProjectToType<LocationDTO>()
                .SingleOrDefaultAsync();
        }

        public async Task DiscardLocationVersion(int locationId)
        {
            Location locationToBeDeleted = await (from l in this.pipContext.Location
                                                  where l.LocationId == locationId && l.Status == 3 && l.IsActive == false
                                                  select new Location
                                                  {
                                                      SerialId = l.SerialId,
                                                      LocationId = l.LocationId,
                                                      LocationName = l.LocationName,
                                                      HoursPerDay = l.HoursPerDay,
                                                      HoursPerMonth = l.HoursPerMonth,
                                                      CountryId = l.CountryId,
                                                      Comments = l.Comments,
                                                      EbitdaSeatCost = l.EbitdaSeatCost,
                                                      EndDate = l.EndDate,
                                                      StartDate = l.StartDate,
                                                      InflationRate = l.InflationRate,
                                                      IsActive = l.IsActive,
                                                      RefUSD = l.RefUSD,
                                                      Status = l.Status
                                                  }).SingleOrDefaultAsync();

            if (locationToBeDeleted != null)
            {
                this.pipContext.Location.Remove(locationToBeDeleted);
            }
            await this.pipContext.SaveChangesAsync();
        }

    }
}
