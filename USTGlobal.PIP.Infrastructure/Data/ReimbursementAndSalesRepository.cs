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
    public class ReimbursementAndSalesRepository : IReimbursementAndSalesRepository
    {
        private readonly PipContext pipContext;

        public ReimbursementAndSalesRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<ReimbursementAndSalesDTO> GetReimbursementAndSalesDetails(int pipSheetId)
        {
            ReimbursementAndSalesDTO reimbursementAndSalesDTO = new ReimbursementAndSalesDTO();
            List<ReimbursementPeriodDTO> reimbursementPeriodDTO = new List<ReimbursementPeriodDTO>();
            List<SalesDiscountPeriodDTO> salesDiscountPeriodDTO = new List<SalesDiscountPeriodDTO>();


            await pipContext.LoadStoredProc("dbo.sp_GetReimbursementAndSalesData")
                .WithSqlParam("@PIPSheetId", pipSheetId)
                .ExecuteStoredProcAsync((resultSet) =>
                {
                    reimbursementAndSalesDTO.Reimbursements = resultSet.ReadToList<ReimbursementParentDTO>().ToList();
                    resultSet.NextResult();


                    reimbursementPeriodDTO = resultSet.ReadToList<ReimbursementPeriodDTO>().ToList();
                    resultSet.NextResult();


                    reimbursementAndSalesDTO.SalesDiscounts = resultSet.ReadToList<SalesDiscountParentDTO>().ToList();
                    resultSet.NextResult();


                    salesDiscountPeriodDTO = resultSet.ReadToList<SalesDiscountPeriodDTO>().ToList();
                    resultSet.NextResult();

                    reimbursementAndSalesDTO.ProjectPeriods = resultSet.ReadToList<ProjectPeriodDTO>().ToList();
                    resultSet.NextResult();

                    reimbursementAndSalesDTO.ProjectMilestones = resultSet.ReadToList<ProjectMilestoneDTO>().ToList();
                    resultSet.NextResult();

                });


            foreach (ReimbursementParentDTO rParent in reimbursementAndSalesDTO.Reimbursements)
            {
                List<ReimbursementPeriodDTO> reimbursementPeriodDTOs = new List<ReimbursementPeriodDTO>();
                reimbursementPeriodDTOs.AddRange(reimbursementPeriodDTO.Where(period => period.ReimbursementId == rParent.ReimbursementId).ToList());
                rParent.ReimbursementPeriods = reimbursementPeriodDTOs;
            }


            foreach (SalesDiscountParentDTO sParent in reimbursementAndSalesDTO.SalesDiscounts)
            {
                List<SalesDiscountPeriodDTO> salesPeriodDTOs = new List<SalesDiscountPeriodDTO>();
                salesPeriodDTOs.AddRange(salesDiscountPeriodDTO.Where(period => period.SalesDiscountId == sParent.SalesDiscountId).ToList());
                sParent.SalesDiscountPeriods = salesPeriodDTOs;
            }

            return reimbursementAndSalesDTO;
        }

        public async Task SaveReimbursementAndSalesDetails(string userName, IList<ReimbursementPeriodDTO> reimbursementPeriods, IList<ReimbursementDTO> reimbursements, IList<SalesDiscountPeriodDTO> salesDiscountPeriods, IList<SalesDiscountDTO> salesDiscounts)
        {
            await pipContext.Database.ExecuteSqlCommandAsync("exec sp_SaveReimbursementAndSalesDiscounts {0}, {1}, {2}, {3}, {4}",
                userName,
                new SqlParameter("@InputReimbursement", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(reimbursements),
                    TypeName = "dbo.Reimbursement"
                },
                new SqlParameter("@InputReimbursementPeriodDetail", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(reimbursementPeriods),
                    TypeName = "dbo.ReimbursementPeriodDetail"
                },
                new SqlParameter("@InputSalesDiscount", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(salesDiscounts),
                    TypeName = "dbo.SalesDiscount"
                },
                new SqlParameter("@InputSalesDiscountPeriodDetail", SqlDbType.Structured)
                {
                    Value = IListToDataTableHelper.ToDataTables(salesDiscountPeriods),
                    TypeName = "dbo.SalesDiscountPeriodDetail"
                });

            await pipContext.SaveChangesAsync();
        }
    }
}
