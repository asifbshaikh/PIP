using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class EbitdaRepository : IEbitdaRepository
    {
        private readonly PipContext pipContext;

        public EbitdaRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<List<EbitdaDTO>> GetEbitdaAndStandardOverhead(int pipSheetId)
        {
            List<EbitdaDTO> ebitdaDTOList = new List<EbitdaDTO>();
            await pipContext.LoadStoredProc("dbo.sp_GetEbitdaAndStandardOverhead")
               .WithSqlParam("@PipSheetId", pipSheetId)
               .ExecuteStoredProcAsync((result) =>
               {
                   ebitdaDTOList = result.ReadToList<EbitdaDTO>().ToList();
               });
            return ebitdaDTOList;
        }

        public async Task UpdateEbitda(string userName, List<EbitdaDTO> ebitdadata, bool isOverridenValueLessThanRefUSD)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_UpdateEbitdaData {0}, {1}, {2}",
                userName,
                isOverridenValueLessThanRefUSD,
                new SqlParameter("@InputEbitdaData", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(ebitdadata),
                    TypeName = "dbo.Ebitda"
                });
            await pipContext.SaveChangesAsync();
        }
    }
}
