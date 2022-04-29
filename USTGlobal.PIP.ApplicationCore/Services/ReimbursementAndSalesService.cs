using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class ReimbursementAndSalesService : IReimbursementAndSalesService
    {
        private readonly IReimbursementAndSalesRepository reimbursementAndSalesRepository;

        public ReimbursementAndSalesService(IReimbursementAndSalesRepository reimbursementAndSalesRepository)
        {
            this.reimbursementAndSalesRepository = reimbursementAndSalesRepository;
        }

        public async Task<ReimbursementAndSalesDTO> GetReimbursementAndSalesDetails(int pipSheetId)
        {
            return await this.reimbursementAndSalesRepository.GetReimbursementAndSalesDetails(pipSheetId);
        }

        public async Task SaveReimbursementAndSalesDetails(string userName, ReimbursementAndSalesDTO reimbursementAndSalesDTO)
        {
            // reimbursement 

            IList<ReimbursementPeriodDTO> reimbursementPeriods = reimbursementAndSalesDTO.Reimbursements.SelectMany(x => x.ReimbursementPeriods).ToList();
            IList<ReimbursementDTO> reimbursement = reimbursementAndSalesDTO.Reimbursements.Select(x => new ReimbursementDTO
            {
                UId = x.UId,
                ReimbursementId = x.ReimbursementId,
                PipSheetId = x.PipSheetId,
                Description = x.Description,
                MilestoneId = x.MilestoneId == -1 ? null : x.MilestoneId,
                isDeleted = x.isDeleted,
                DirectExpensePercent = x.DirectExpensePercent,
                IsDirectExpenseReimbursable = x.IsDirectExpenseReimbursable,
                CreatedOn = DateTime.Now,
                ReimbursedExpense = x.ReimbursedExpense,
                UpdatedOn = DateTime.Now
            }).ToList();

            //sales Discount

            IList<SalesDiscountPeriodDTO> salesDiscountPeriods = reimbursementAndSalesDTO.SalesDiscounts.SelectMany(x => x.SalesDiscountPeriods).ToList();
            IList<SalesDiscountDTO> salesDiscounts = reimbursementAndSalesDTO.SalesDiscounts.Select(x => new SalesDiscountDTO
            {
                UId = x.UId,
                SalesDiscountId = x.SalesDiscountId,
                PipSheetId = x.PipSheetId,
                Description = x.Description,
                MilestoneId = x.MilestoneId == -1 ? null : x.MilestoneId,
                isDeleted = x.isDeleted,
                CreatedOn = DateTime.Now,
                Discount = x.Discount,
                UpdatedOn = DateTime.Now
            }).ToList();


            await this.reimbursementAndSalesRepository.SaveReimbursementAndSalesDetails(userName, reimbursementPeriods, reimbursement, salesDiscountPeriods, salesDiscounts);
        }

        public ReimbursementAndSalesDTO ReAssignUIds(ReimbursementAndSalesDTO reimbursementAndSales)
        {
            // Re-Assigning UIds to Reimbursement object
            for (int i = 0; i < reimbursementAndSales.Reimbursements.Count; i++)
            {
                reimbursementAndSales.Reimbursements[i].UId = i + 1;
                foreach (var r in reimbursementAndSales.Reimbursements[i].ReimbursementPeriods)
                {
                    r.UId = i + 1;
                }
            }

            // Re-Assigning UIds to Sales Discount object
            for (int i = 0; i < reimbursementAndSales.SalesDiscounts.Count; i++)
            {
                reimbursementAndSales.SalesDiscounts[i].UId = i + 1;
                foreach (var p in reimbursementAndSales.SalesDiscounts[i].SalesDiscountPeriods)
                {
                    p.UId = i + 1;
                }
            }
            return reimbursementAndSales;
        }
    }
}
